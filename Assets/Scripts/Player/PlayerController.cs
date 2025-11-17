using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PlayerStats stats; // Seu gamedata (valores BASE)

    [Header("Hitbox References")]
    [SerializeField] private GameObject hitboxNE;
    [SerializeField] private GameObject hitboxNW;
    [SerializeField] private GameObject hitboxSW;
    [SerializeField] private GameObject hitboxSE;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip footstepLoop; 
    [Range(0f, 2f)]
    [SerializeField] private float footstepVolume = 0.5f;

    [Header("Skill: Dagger")]
    [SerializeField] private GameObject daggerPrefab;
    [SerializeField] private float daggerSpawnOffset = 0.5f;
    [SerializeField] private AudioClip daggerThrowSound;
    
    private float daggerCooldownTimer = 0f;
    private bool isDaggerOnCooldown = false;

    [Header("Skill: Decoy")]
    [SerializeField] private GameObject decoyPrefab;
    [SerializeField] private float decoySpawnOffset = 1.0f;
    
    private float decoyCooldownTimer = 0f;
    private bool isDecoyOnCooldown = false;

    // 🌟 MODIFICADORES DE STATS (aplicados sobre os valores base)
    [Header("📊 Modificadores Permanentes")]
    [SerializeField] private float moveSpeedModifier = 0f;
    [SerializeField] private int maxHealthModifier = 0;
    [SerializeField] private int attackDamageModifier = 0;
    [SerializeField] private int daggerDamageModifier = 0;
    [SerializeField] private float daggerCooldownModifier = 0f;
    [SerializeField] private float decoyCooldownModifier = 0f;
    [SerializeField] private float decoyDurationModifier = 0f;

    // Propriedades públicas para acessar os stats MODIFICADOS
    public float CurrentMoveSpeed => stats.moveSpeed + moveSpeedModifier;
    public int CurrentMaxHealth => stats.maxHealth + maxHealthModifier;
    public int CurrentAttackDamage => stats.attackDamage + attackDamageModifier;
    public int CurrentDaggerDamage => stats.daggerDamage + daggerDamageModifier;
    public float CurrentDaggerCooldown => Mathf.Max(0, stats.daggerCooldown - daggerCooldownModifier);
    public float CurrentDecoyCooldown => Mathf.Max(0, stats.decoyCooldown - decoyCooldownModifier);
    public float CurrentDecoyDuration => stats.decoyDuration + decoyDurationModifier;

    // Eventos
    public static event Action<Vector2> OnMove;
    public static event Action<Vector2> OnAttack;

    // Componentes e Variáveis
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lastDirection = new Vector2(1, 1);
    private bool isAttacking = false;
    private Hitbox activeHitbox;
    private AudioSource footstepAudioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.clip = footstepLoop;
        footstepAudioSource.loop = true; 
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.volume = footstepVolume;

        hitboxNE.SetActive(false);
        hitboxNW.SetActive(false);
        hitboxSW.SetActive(false);
        hitboxSE.SetActive(false);
    }

    void Update()
    {
        // Cooldown da adaga
        if (isDaggerOnCooldown)
        {
            daggerCooldownTimer -= Time.deltaTime;
            if (daggerCooldownTimer <= 0)
            {
                isDaggerOnCooldown = false;
                Debug.Log("🗡️ Skill Adaga pronta!");
            }
        }

        // Cooldown do decoy
        if (isDecoyOnCooldown)
        {
            decoyCooldownTimer -= Time.deltaTime;
            if (decoyCooldownTimer <= 0)
            {
                isDecoyOnCooldown = false;
                Debug.Log("Skill Decoy pronta!");
            }
        }

        if (isAttacking)
        {
            movementInput = Vector2.zero;
            OnMove?.Invoke(movementInput);
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput.magnitude > 0)
        {
            lastDirection = movementInput;
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.volume = footstepVolume;
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }

        OnMove?.Invoke(movementInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackCoroutine());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking && !isDecoyOnCooldown)
        {
            UseDecoy();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && !isDaggerOnCooldown)
        {
            ThrowDagger();
        }
    }

    void FixedUpdate()
    {
        // USA O STAT MODIFICADO
        rb.MovePosition(rb.position + movementInput * CurrentMoveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        OnAttack?.Invoke(lastDirection);

        if (AudioManager.Instance != null && attackSound != null)
        {
            AudioManager.Instance.PlaySound(attackSound, transform.position, 1f);
        }

        yield return new WaitForSeconds(stats.attackHitboxDelay);
        GameObject hitboxToActivate = GetHitboxForDirection(lastDirection);
        if (hitboxToActivate != null)
        {
            hitboxToActivate.SetActive(true);
            activeHitbox = hitboxToActivate.GetComponent<Hitbox>();
            if (activeHitbox != null)
            {
                // USA O DANO MODIFICADO
                activeHitbox.damage = CurrentAttackDamage;
            }
        }
        yield return new WaitForSeconds(stats.attackHitboxActiveTime);
        if (hitboxToActivate != null)
        {
            hitboxToActivate.SetActive(false);
        }
        float remainingTime = stats.attackAnimationDuration - stats.attackHitboxDelay - stats.attackHitboxActiveTime;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }
        isAttacking = false;
    }

    private void ThrowDagger()
    {
        if (daggerPrefab == null)
        {
            Debug.LogError("⚠️ Prefab da adaga não foi atribuído no PlayerController!");
            return;
        }

        // 🎯 Calcula a direção do mouse em relação ao jogador
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0; // Garante que o Z seja 0 (jogo 2D)
        
        Vector2 directionToMouse = (mouseWorldPosition - transform.position).normalized;

        // Se o mouse estiver muito perto do jogador (deadzone), usa a última direção
        if (Vector2.Distance(transform.position, mouseWorldPosition) < 0.5f)
        {
            directionToMouse = lastDirection;
        }

        isDaggerOnCooldown = true;
        daggerCooldownTimer = CurrentDaggerCooldown;
        Debug.Log($"🗡️ Adaga lançada na direção do mouse! Cooldown de {CurrentDaggerCooldown}s iniciado.");

        // Calcula a posição de spawn ligeiramente à frente do player na direção do mouse
        Vector3 spawnPosition = transform.position + (Vector3)directionToMouse * daggerSpawnOffset;

        // Instancia a adaga
        GameObject daggerInstance = Instantiate(daggerPrefab, spawnPosition, Quaternion.identity);

        // Inicializa o script da adaga com a direção do mouse
        DaggerProjectile daggerScript = daggerInstance.GetComponent<DaggerProjectile>();
        if (daggerScript != null)
        {
            daggerScript.Initialize(directionToMouse, stats.daggerSpeed, CurrentDaggerDamage, stats.daggerLifetime);
        }
        else
        {
            Debug.LogError("⚠️ O prefab da adaga está sem o script DaggerProjectile.cs!");
        }

        // Reproduz o som de lançamento
        if (AudioManager.Instance != null && daggerThrowSound != null)
        {
            AudioManager.Instance.PlaySound(daggerThrowSound, transform.position, 1f);
        }
    }

    private GameObject GetHitboxForDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > 22.5f && angle <= 67.5f) return hitboxNE;
        if (angle > 67.5f && angle <= 112.5f) return hitboxNE;
        if (angle > 112.5f && angle <= 157.5f) return hitboxNW;
        if (angle > 157.5f || angle <= -157.5f) return hitboxNW;
        if (angle > -157.5f && angle <= -112.5f) return hitboxSW;
        if (angle > -112.5f && angle <= -67.5f) return hitboxSW;
        if (angle > -67.5f && angle <= -22.5f) return hitboxSE;
        if (angle > -22.5f && angle <= 22.5f) return hitboxSE;

        return hitboxNE;
    }

    private void UseDecoy()
    {
        isDecoyOnCooldown = true;
        // USA O COOLDOWN MODIFICADO
        decoyCooldownTimer = CurrentDecoyCooldown;
        Debug.Log("Usou Decoy! Cooldown de " + CurrentDecoyCooldown + "s iniciado.");

        bool facingRight = lastDirection.x >= 0;
        Vector2 spawnDirection = facingRight ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = transform.position + (Vector3)spawnDirection * decoySpawnOffset;

        GameObject decoyInstance = Instantiate(decoyPrefab, spawnPosition, Quaternion.identity);

        Decoy decoyScript = decoyInstance.GetComponent<Decoy>();
        if (decoyScript != null)
        {
            // USA A DURAÇÃO MODIFICADA
            decoyScript.Initialize(CurrentDecoyDuration, stats.decoyDestructionAnimTime, facingRight);
        }
        else
        {
            Debug.LogError("O prefab 'Decoy' está sem o script Decoy.cs!");
        }
    }

    // 🌟 MÉTODOS PÚBLICOS PARA APLICAR POWER-UPS
    public void ApplyPowerUp(float moveSpeed, int maxHealth, int attackDamage, float decoyCooldownReduction, float decoyDuration)
    {
        Debug.Log($"📊 STATS ANTES DO POWER-UP:\n" +
                 $"  • Velocidade: {CurrentMoveSpeed}\n" +
                 $"  • Vida Máxima: {CurrentMaxHealth}\n" +
                 $"  • Dano: {CurrentAttackDamage}\n" +
                 $"  • Cooldown Decoy: {CurrentDecoyCooldown}s\n" +
                 $"  • Duração Decoy: {CurrentDecoyDuration}s");

        moveSpeedModifier += moveSpeed;
        maxHealthModifier += maxHealth;
        attackDamageModifier += attackDamage;
        decoyCooldownModifier += decoyCooldownReduction;
        decoyDurationModifier += decoyDuration;

        Debug.Log($"⚡ POWER-UP APLICADO COM SUCESSO!\n" +
                 $"  • Velocidade: {CurrentMoveSpeed} (+{moveSpeed})\n" +
                 $"  • Vida Máxima: {CurrentMaxHealth} (+{maxHealth})\n" +
                 $"  • Dano: {CurrentAttackDamage} (+{attackDamage})\n" +
                 $"  • Cooldown Decoy: {CurrentDecoyCooldown}s (-{decoyCooldownReduction}s)\n" +
                 $"  • Duração Decoy: {CurrentDecoyDuration}s (+{decoyDuration}s)");

        // Atualizar vida máxima se houve aumento
        if (maxHealth > 0)
        {
            Health playerHealth = GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.IncreaseMaxHealth(maxHealth);
            }
            else
            {
                Debug.LogWarning("⚠️ Componente Health do jogador não encontrado!");
            }
        }
    }

    // Método para resetar modificadores (útil para Game Over)
    public void ResetModifiers()
    {
        moveSpeedModifier = 0f;
        maxHealthModifier = 0;
        attackDamageModifier = 0;
        daggerDamageModifier = 0;
        daggerCooldownModifier = 0f;
        decoyCooldownModifier = 0f;
        decoyDurationModifier = 0f;
        Debug.Log("🔄 Modificadores de stats resetados!");
    }
}