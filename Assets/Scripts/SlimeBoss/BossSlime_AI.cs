using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BossSlime_Health))] 
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CircleCollider2D))] 
public class BossSlime_AI : MonoBehaviour
{
    [Header("Gamedata (Stats)")]
    public BossSlimeStats stats; 

    [Header("Referências de Setup")]
    public GameObject spikeAttackHitbox; 

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [Range(0f, 2f)]
    [SerializeField] private float attackVolume = 1f;

    // Componentes e Estado
    private Animator animator;
    private BossSlime_Health health;
    private Transform player;

    private SlimeHitbox spikeHitboxScript; 
    private CircleCollider2D spikeAttackCollider; 
    private CircleCollider2D detectionCollider;

    private enum State { Idle, Attacking, Dead }
    private State currentState = State.Idle;
    
    private float lastAttackTime;

    private bool isPlayerInRange = false;
    private float playerDetectionTimer; 
    private const float DETECTION_TIMEOUT = 0.1f; 

    void Awake()
    {
        animator = GetComponent<Animator>();
        health = GetComponent<BossSlime_Health>();
        
        detectionCollider = GetComponent<CircleCollider2D>();
        detectionCollider.isTrigger = true;
        // Esta linha agora vai ler 5f do Gamedata
        detectionCollider.radius = stats.detectionRange; 

        health.maxHealth = stats.maxHealth;

        if (spikeAttackHitbox == null) {
            Debug.LogError("O 'SpikeAttackHitbox' não foi definido no " + gameObject.name);
        } else {
            spikeHitboxScript = spikeAttackHitbox.GetComponent<SlimeHitbox>();
            spikeAttackCollider = spikeAttackHitbox.GetComponent<CircleCollider2D>();
            spikeAttackHitbox.SetActive(false); 
        }
    }

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            player = playerObj.transform;
        }
        
        lastAttackTime = Time.time - stats.attackCooldown; 
    }

    void Update()
    {
        if (player == null) return;
        
        if (Time.time > playerDetectionTimer + DETECTION_TIMEOUT)
        {
            isPlayerInRange = false;
        }

        if (currentState != State.Idle || !isPlayerInRange)
        {
            return;
        }
        
        if (Time.time > lastAttackTime + stats.attackCooldown)
        {
            currentState = State.Attacking;
            lastAttackTime = Time.time;
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        animator.SetTrigger("Attack"); 
        
        // 🎵 Reproduz o som de ataque do Boss Slime
        if (AudioManager.Instance != null && attackSound != null)
        {
            AudioManager.Instance.PlaySound(attackSound, transform.position, attackVolume);
        }
        
        yield return new WaitForSeconds(stats.attackCastTime);

        spikeAttackHitbox.SetActive(true); 
        spikeHitboxScript.damage = stats.attackDamage;
        spikeAttackCollider.radius = 0f; 

        float timer = 0f;
        while (timer < stats.attackActiveDuration)
        {
            // Esta linha vai ler 5f do Gamedata
            spikeAttackCollider.radius = Mathf.Lerp(0f, stats.maxAttackRadius, timer / stats.attackActiveDuration);
            timer += Time.deltaTime;
            yield return null; 
        }

        spikeAttackCollider.radius = 0f; 
        spikeAttackHitbox.SetActive(false);

        currentState = State.Idle;
    }
    
    public void TriggerDeath()
    {
        if (currentState == State.Dead) return;

        currentState = State.Dead;
        animator.SetTrigger("Die"); 
        
        detectionCollider.enabled = false; 
        
        if(spikeAttackHitbox != null) spikeAttackHitbox.SetActive(false);
        Destroy(gameObject, 3f); 
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            playerDetectionTimer = Time.time;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (stats == null) return;

        Gizmos.color = Color.yellow;
        // Esta linha agora vai desenhar um círculo de 5f
        Gizmos.DrawWireSphere(transform.position, stats.detectionRange); 

        Gizmos.color = Color.red;
        // Esta linha também vai desenhar um círculo de 5f
        Gizmos.DrawWireSphere(transform.position, stats.maxAttackRadius); 
    }
}