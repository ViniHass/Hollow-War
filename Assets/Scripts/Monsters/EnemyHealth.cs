using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private MonsterStats stats;
    private int currentHealth;
    private EnemyAI ai;
    private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;
    [Range(0f, 2f)]
    [SerializeField] private float damageVolume = 1f;

    void Awake()
    {
        ai = GetComponent<EnemyAI>();
        animator = GetComponent<Animator>();
        currentHealth = stats.maxHealth;
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (currentHealth <= 0) return; // Já está morto

        currentHealth -= damage;
        ai.TriggerFeedback(knockbackDirection);

        // Reproduz o som de dano com o volume ajustável
        if (AudioManager.Instance != null && damageSound != null)
        {
            AudioManager.Instance.PlaySound(damageSound, transform.position, damageVolume);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Inimigo Morreu!");
        animator.SetTrigger("Die");
        ai.enabled = false; 
        
        // Desativa todos os colisores para que o inimigo morto não bloqueie o caminho
        foreach(Collider2D col in GetComponentsInChildren<Collider2D>())
        {
            col.enabled = false;
        }

        // Não desabilitamos mais o script, pois ele precisa estar ativo
        // para que a função do Animation Event seja encontrada.
    }

    // --- NOVA FUNÇÃO PARA O ANIMATION EVENT ---
    /// <summary>
    /// Esta função pública será chamada pelo Animation Event no final da animação de morte.
    /// </summary>
    public void DestroyOnDeath()
    {
        // Destrói o GameObject raiz do inimigo.
        Destroy(gameObject);
    }
}