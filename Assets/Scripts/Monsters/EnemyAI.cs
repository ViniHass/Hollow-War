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
    
    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [Range(0f, 2f)]
    [SerializeField] private float attackVolume = 1f;
    
    // Componentes e Variáveis
    private Transform player; // Apenas uma referência de segurança
    private Transform currentTarget; // O ALVO ATUAL (Decoy ou Player)
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Estado
    private Vector2 lastDirection = Vector2.down; 
    private Color originalColor;
    private bool canAttack = true;

    private enum State { Idle, Chasing, CombatIdle, Attacking, Feedback }
    private State currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        
        // Configura o raio do colisor filho baseado nos stats
        var zone = transform.Find("DetectionZone");
        if(zone) zone.GetComponent<CircleCollider2D>().radius = stats.detectionRadius;
    }

    void Start()
    {
        currentState = State.Idle;
    }

    void Update()
    {
        // 1. Bloqueios de Estado: Se estiver atacando ou tomando dano, a física/lógica de movimento não roda.
        if (currentState == State.Attacking || currentState == State.Feedback) return;

        // 2. Sem alvo -> Idle
        if (currentTarget == null)
        {
            if (currentState != State.Idle) EnterIdleState();
            return;
        }

        // 3. Lógica de Combate e Movimento
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        // Está dentro da distância de ataque?
        if (distanceToTarget <= stats.attackRadius)
        {
            // Pode atacar?
            if (canAttack)
            {
                StartCoroutine(AttackSequence());
            }
            else
            {
                // --- CORREÇÃO DO BUG ---
                // Está perto, mas em cooldown. Fica parado encarando (CombatIdle).
                EnterCombatIdleState();
            }
        }
        else
        {
            // Está longe -> Persegue
            ChaseTarget();
        }
    }

    // --- ESTADOS E COMPORTAMENTOS ---

    private void EnterIdleState()
    {
        currentState = State.Idle;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    private void EnterCombatIdleState()
    {
        currentState = State.CombatIdle;
        rb.linearVelocity = Vector2.zero; // Garante que pare
        animator.SetBool("isMoving", false);
        
        // Opcional: Virar para o player mesmo parado
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        UpdateAnimationFacing(direction); 
    }

    private void ChaseTarget()
    {
        currentState = State.Chasing;
        
        // Calcula direção
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        
        // Move
        rb.linearVelocity = direction * stats.moveSpeed;
        
        // Anima
        animator.SetBool("isMoving", true);
        UpdateAnimationFacing(direction);
    }

    private IEnumerator AttackSequence()
    {
        currentState = State.Attacking;
        canAttack = false;
        
        // Para imediatamente o movimento
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false); // Garante que a animação de andar pare

        // Define direção do ataque
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        UpdateAnimationFacing(direction);
        animator.SetTrigger("Attack");

        // 🎵 Reproduz o som de ataque
        if (AudioManager.Instance != null && attackSound != null)
        {
            AudioManager.Instance.PlaySound(attackSound, transform.position, attackVolume);
        }

        yield return new WaitForSeconds(stats.attackHitboxDelay);

        // Ativa Hitbox
        GameObject hitboxToActivate = GetHitboxForDirection(direction);
        var hitboxScript = hitboxToActivate.GetComponent<EnemyHitbox>();
        if(hitboxScript) hitboxScript.damage = stats.attackDamage;
        
        hitboxToActivate.SetActive(true);

        // Tempo da Hitbox ativa
        yield return new WaitForSeconds(stats.attackHitboxActiveTime);

        hitboxToActivate.SetActive(false);

        // (Opcional) Pequeno delay pós-ataque para ele não "patinar" instantaneamente (Recovery)
        yield return new WaitForSeconds(0.1f);

        currentState = State.Chasing; // Libera o Update para decidir o próximo passo

        // Cooldown
        yield return new WaitForSeconds(stats.attackCooldown);
        canAttack = true;
    }

    // --- AUXILIARES ---

    private void UpdateAnimationFacing(Vector2 direction)
    {
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
    
    public void SetTarget(Transform target) { currentTarget = target; }
    public void ClearTarget() { currentTarget = null; }

    public void TriggerFeedback(Vector2 knockbackDirection)
    {
        StartCoroutine(FeedbackCoroutine(knockbackDirection));
    }

    private IEnumerator FeedbackCoroutine(Vector2 knockbackDirection)
    {
        // Salva o estado anterior se quiser voltar exatamente para ele, 
        // mas geralmente voltar para Chasing/Idle no Update é mais seguro.
        currentState = State.Feedback;
        
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * stats.knockbackForce, ForceMode2D.Impulse);

        spriteRenderer.color = Color.white; // Flash
        yield return new WaitForSeconds(stats.flashDuration);
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(stats.knockbackDuration - stats.flashDuration);

        rb.linearVelocity = Vector2.zero; 
        
        // Ao terminar o feedback, liberamos o estado para o Update decidir
        if(currentTarget != null)
            currentState = State.Chasing;
        else
            currentState = State.Idle;
    }

    private GameObject GetHitboxForDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? hitboxE : hitboxW;
        else
            return direction.y > 0 ? hitboxN : hitboxS;
    }

    // Debug visual para entender o alcance
    void OnDrawGizmosSelected()
    {
        if(stats != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.attackRadius);
        }
    }
}