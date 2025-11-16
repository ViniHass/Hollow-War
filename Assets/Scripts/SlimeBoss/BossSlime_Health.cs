using UnityEngine;
using System.Collections;

public class BossSlime_Health : MonoBehaviour
{
    [HideInInspector] public int maxHealth; // Será preenchido pelo script de IA
    private int currentHealth;

    // Referência específica para a IA do Slime
    private BossSlime_AI slimeAI; 
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake()
    {
        // Pega a IA específica do Slime
        slimeAI = GetComponent<BossSlime_AI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Esta função será chamada pelo Hitbox.cs do seu PLAYER
    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (currentHealth <= 0) return; // Já está morto

        currentHealth -= damage;
        
        // Pisca em vermelho
        if(flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        if(spriteRenderer == null) yield break;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
    }

    private void Die()
    {
        // Avisa a IA do Slime para morrer
        if(slimeAI != null)
        {
            slimeAI.TriggerDeath();
        }
        else
        {
            // Fallback
            Destroy(gameObject);
        }
    }
}