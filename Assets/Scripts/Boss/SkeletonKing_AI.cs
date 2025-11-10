using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonKing_AI : MonoBehaviour
{
    [Header("Configurações Essenciais")]
    public BossStats stats; 
    public Transform player;
    public LayerMask obstacleLayer; 

    [Header("Referências de Ataque")]
    public Transform attackOrigin; 
    public GameObject orbPrefab; 
    public GameObject swordPrefab; 

    // --- Variáveis Internas ---
    private enum BossState { Idle, Activated, Attacking, Teleporting }
    private BossState currentState;

    private Animator animator;
    private float lastAttackTime;
    private bool isPlayerDetected = false;
    private int currentAttackIndex; 
    
    // "Bandeira" para o teleporte por dano
    private bool teleportOnDamageQueued = false; 

    void Start()
    {
        animator = GetComponent<Animator>();
        currentState = BossState.Idle;
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        // Se o player não existe, não foi detectado, OU JÁ ESTÁ OCUPADO (atacando ou teleportando)
        // não fazemos nada. As corrotinas/eventos cuidarão de liberar o estado.
        if (player == null || !isPlayerDetected || currentState == BossState.Attacking || currentState == BossState.Teleporting)
        {
            return;
        }
        
        // Se estamos livres (currentState == Activated), podemos pensar.
        UpdateAnimatorDirection();
        HandleActivatedState(); // Executa a lógica de decisão
    }

    // Lógica de decisão principal
    void HandleActivatedState()
    {
        // PRIORIDADE 1: Fomos atingidos? (Teleporte por Dano)
        if (teleportOnDamageQueued)
        {
            teleportOnDamageQueued = false; // "Baixa a bandeira"
            TriggerTeleport(); // Dispara o teleporte
            return; // Não faz mais nada (não ataca)
        }

        // PRIORIDADE 2: O player fugiu? (Teleporte por Fuga)
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > stats.maxPlayerDistance)
        {
            TriggerTeleport(); // Dispara o teleporte
        }
        // PRIORIDADE 3: Estamos prontos para atacar?
        // (Só executa se NÃO tivermos que teleportar)
        else if (Time.time > lastAttackTime + stats.attackCooldown)
        {
            ChooseAndExecuteAttack();
        }
    }

    void UpdateAnimatorDirection()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);
    }

    void ChooseAndExecuteAttack()
    {
        // Trava a IA, o estado "Attacking" agora cobre
        // tanto o "cast" quanto a execução do ataque.
        currentState = BossState.Attacking; 
        lastAttackTime = Time.time; 
        currentAttackIndex = Random.Range(0, 2); 
        
        switch (currentAttackIndex)
        {
            case 0:
                animator.SetTrigger("CastOrb");
                break;
            case 1:
                animator.SetTrigger("CastSword");
                break;
        }
    }

    #region Teleport Logic
    
    // Chamada pelo Health script
    public void QueueDamageTeleport()
    {
        teleportOnDamageQueued = true;
    }
    
    // Chamada pela lógica de fuga ou pela fila de dano
    public void TriggerTeleport()
    {
        // Não precisamos mais checar o estado aqui,
        // pois HandleActivatedState já faz isso.
        if (currentState == BossState.Teleporting)
        {
            return;
        }
        
        StartCoroutine(TeleportSequence());
    }

    IEnumerator TeleportSequence()
    {
        currentState = BossState.Teleporting;
        animator.SetTrigger("Teleport"); 
        yield return new WaitForSeconds(stats.teleportOutAnimationDuration);
        GetComponent<SpriteRenderer>().enabled = false; 
        Vector2 newPosition = FindSafeTeleportLocation();
        transform.position = newPosition;
        yield return new WaitForSeconds(stats.teleportReappearDelay);
        GetComponent<SpriteRenderer>().enabled = true;
        
        // Libera a IA e reseta o cooldown
        currentState = BossState.Activated;
        lastAttackTime = Time.time - stats.attackCooldown + 1.0f;
    }

    Vector2 FindSafeTeleportLocation()
    {
        for (int i = 0; i < 10; i++)
        {
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(stats.teleportMinDistance, stats.teleportMaxDistance);
            Vector2 targetPosition = (Vector2)player.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
            Collider2D hit = Physics2D.OverlapCircle(targetPosition, 1f, obstacleLayer);
            if (hit == null) { return targetPosition; }
        }
        return (Vector2)player.position + (Random.insideUnitCircle.normalized * stats.teleportMinDistance);
    }
    #endregion

    #region Boss Activation
    public void ActivateBoss()
    {
        if (isPlayerDetected) return;
        Debug.Log("BOSS ATIVADO!");
        isPlayerDetected = true;
        currentState = BossState.Activated;
        lastAttackTime = Time.time - stats.attackCooldown + 1.0f; 
    }
    #endregion

    #region Animation Events
    
    // --- CORREÇÃO PRINCIPAL (BUG DA FUGA) ---
    // Este evento agora SÓ dispara o ataque.
    // Ele NÃO libera mais a IA.
    public void EVENT_AttackAndFinish()
    {
        switch (currentAttackIndex)
        {
            case 0:
                LaunchOrbAttack();
                break;
            case 1:
                LaunchSwordAttack();
                break;
        }
    }
    #endregion

    #region Attack Logic
    
    // --- ATAQUE 1: ORBE ---
    void LaunchOrbAttack()
    {
        if (orbPrefab == null) 
        {
            // Se o prefab não existe, libera a IA para não travar
            currentState = BossState.Activated;
            animator.SetTrigger("AttackFinished");
            return;
        }
        StartCoroutine(OrbAttackCoroutine());
    }

    IEnumerator OrbAttackCoroutine()
    {
        Vector2 spawnPosition = (attackOrigin != null) ? attackOrigin.position : transform.position;
        List<BossOrbProjectile> spawnedOrbs = new List<BossOrbProjectile>();
        
        int numberOfOrbs = 24;
        float angleStep = 360f / numberOfOrbs; 

        for (int i = 0; i < numberOfOrbs; i++)
        {
            float currentAngle = (angleStep * i) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle));
            GameObject orbGO = Instantiate(orbPrefab, spawnPosition, Quaternion.identity);
            BossOrbProjectile orbScript = orbGO.GetComponent<BossOrbProjectile>();
            if (orbScript != null)
            {
                orbScript.Setup( stats.orbSpeed, stats.orbLifetime, (int)stats.damagePerAttack, dir );
                spawnedOrbs.Add(orbScript);
            }
        }
        
        yield return new WaitForSeconds(stats.castDelayOrb);
        
        foreach (BossOrbProjectile orb in spawnedOrbs) {
            if (orb != null) { orb.Launch(); }
        }

        // --- CORREÇÃO PRINCIPAL (BUG DA FUGA) ---
        // A IA SÓ é liberada DEPOIS que as orbes são disparadas.
        currentState = BossState.Activated; 
        animator.SetTrigger("AttackFinished");
    }


    // --- ATAQUE 2: ESPADA ---
    void LaunchSwordAttack()
    {
        if (swordPrefab == null)
        {
            // Se o prefab não existe, libera a IA para não travar
            currentState = BossState.Activated;
            animator.SetTrigger("AttackFinished");
            return;
        }
        StartCoroutine(SwordAttackCoroutine());
    }

    IEnumerator SwordAttackCoroutine()
    {
        yield return new WaitForSeconds(stats.castDelaySword);
        
        GameObject swordGO = Instantiate(swordPrefab, attackOrigin.position, attackOrigin.rotation, attackOrigin);
        BossHitbox swordHitbox = swordGO.GetComponent<BossHitbox>();
        if(swordHitbox != null) { swordHitbox.damage = (int)stats.damagePerAttack; }

        Vector2 directionToPlayer = (player.position - attackOrigin.position).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        float startAngle = targetAngle - 90f;
        float endAngle = targetAngle + 90f;
        
        float timer = 0f;
        while (timer < stats.swordArcDuration)
        {
            float currentAngle = Mathf.LerpAngle(startAngle, endAngle, timer / stats.swordArcDuration);
            attackOrigin.rotation = Quaternion.Euler(0, 0, currentAngle);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(swordGO, stats.swordLifetime - stats.swordArcDuration);
        attackOrigin.localRotation = Quaternion.identity;

        // --- CORREÇÃO PRINCIPAL (BUG DA FUGA) ---
        // A IA SÓ é liberada DEPOIS que o giro da espada termina.
        currentState = BossState.Activated; 
        animator.SetTrigger("AttackFinished");
    }
    #endregion
}