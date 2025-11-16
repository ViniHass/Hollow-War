using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MonsterStats stats;

    [Header("Hitbox References")]
    [SerializeField] private GameObject hitboxN;
    [SerializeField] private GameObject hitboxS;
    [SerializeField] private GameObject hitboxE;
    [SerializeField] private GameObject hitboxW;
    
    // Componentes e Variáveis
    private Transform player; // Apenas uma referência de segurança
    private Transform currentTarget; // O ALVO ATUAL (Decoy ou Player)
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastDirection = Vector2.down; 
    private Color originalColor;
    private bool canAttack = true;

    private enum State { Idle, Chasing, Attacking, Feedback }
    private State currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        
        transform.Find("DetectionZone").GetComponent<CircleCollider2D>().radius = stats.detectionRadius;
    }

    void Start()
    {
        GameObject playerHurtbox = GameObject.FindGameObjectWithTag("Player");
        if(playerHurtbox != null)
        {
            player = playerHurtbox.transform.parent;
        }
        currentState = State.Idle;
    }

    void Update()
    {
        // Se não tiver alvo, ou se estiver atacando/sofrendo dano, fica parado
        if (currentTarget == null || currentState == State.Attacking || currentState == State.Feedback)
        {
            animator.SetBool("isMoving", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // Se tem um alvo, o estado é Chasing
        currentState = State.Chasing;

        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        // Se está perto o suficiente, ataca
        if (distanceToTarget <= stats.attackRadius && canAttack)
        {
            StartCoroutine(Attack());
        }
        // Se está longe, persegue
        else if (distanceToTarget > stats.attackRadius)
        {
            ChaseTarget();
        }
    }

    // --- MÉTODOS DE CONTROLE (Chamados pela DetectionZone) ---

    // DetectionZone chama isso quando um alvo (com prioridade) entra
    public void SetTarget(Transform target)
    {
        currentTarget = target;
        currentState = State.Chasing;
    }

    // DetectionZone chama isso quando todos os alvos saem
    public void ClearTarget()
    {
        currentTarget = null;
        currentState = State.Idle;
    }
    
    // --- O RESTO DOS SEUS MÉTODOS (Sem alterações) ---

    // REMOVIDOS: OnPlayerEnterDetectionZone() e OnPlayerExitDetectionZone()
    // A DetectionZone agora cuida de tudo e chama SetTarget/ClearTarget

    private void ChaseTarget()
    {
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = direction * stats.moveSpeed;
        UpdateAnimation(direction);
    }

    private IEnumerator Attack()
    {
        currentState = State.Attacking;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;
        
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        UpdateAnimation(direction);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(stats.attackHitboxDelay);

        GameObject hitboxToActivate = GetHitboxForDirection(direction);
        hitboxToActivate.GetComponent<EnemyHitbox>().damage = stats.attackDamage;
        hitboxToActivate.SetActive(true);

        yield return new WaitForSeconds(stats.attackHitboxActiveTime);

        hitboxToActivate.SetActive(false);
        currentState = State.Chasing;

        yield return new WaitForSeconds(stats.attackCooldown);
        canAttack = true;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        animator.SetBool("isMoving", direction.magnitude > 0);
        lastDirection = direction;
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetFloat("moveX", direction.x > 0 ? 1 : -1);
            animator.SetFloat("moveY", 0);
        }
        else
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", direction.y > 0 ? 1 : -1);
        }
    }
    
    public void TriggerFeedback(Vector2 knockbackDirection)
    {
        StartCoroutine(FeedbackCoroutine(knockbackDirection));
    }

    private IEnumerator FeedbackCoroutine(Vector2 knockbackDirection)
    {
        currentState = State.Feedback;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * stats.knockbackForce, ForceMode2D.Impulse);

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(stats.flashDuration);
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(stats.knockbackDuration - stats.flashDuration);

        rb.linearVelocity = Vector2.zero; 
        
        if(currentTarget != null)
            currentState = State.Chasing;
        else
            currentState = State.Idle;
    }

    private GameObject GetHitboxForDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? hitboxE : hitboxW;
        }
        else
        {
            return direction.y > 0 ? hitboxN : hitboxS;
        }
    }
}