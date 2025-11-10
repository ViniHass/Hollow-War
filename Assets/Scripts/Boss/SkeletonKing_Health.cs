using System.Collections;
using UnityEngine;

public class SkeletonKing_Health : MonoBehaviour
{
    public BossStats stats; 
    public GameObject barrierVisual; 

    private int currentHealth;
    private bool isInvincible = false;

    private SkeletonKing_AI aiScript;
    private Animator animator;

    void Start()
    {
        currentHealth = stats.maxHealth;
        aiScript = GetComponent<SkeletonKing_AI>();
        animator = GetComponent<Animator>();
        
        if (barrierVisual != null)
        {
            barrierVisual.SetActive(false);
        }
    }

    public void TakeDamage(int damage, Vector2 knockbackDirection)
    {
        if (isInvincible || currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;
        // (Ignoramos knockbackDirection de propósito)

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(ActivateBarrier());
            
            // --- CORREÇÃO DE ESTADO ---
            // Em vez de forçar um teleporte, nós "enfileiramos" um.
            // A IA vai executá-lo assim que terminar a ação atual.
            aiScript.QueueDamageTeleport(); 
        }
    }

    IEnumerator ActivateBarrier()
    {
        isInvincible = true;
        if (barrierVisual != null) { barrierVisual.SetActive(true); }

        yield return new WaitForSeconds(stats.barrierDuration);

        isInvincible = false;
        if (barrierVisual != null) { barrierVisual.SetActive(false); }
    }

    void Die()
    {
        Debug.Log("Boss Morreu!");
        animator.SetTrigger("Die");
        aiScript.enabled = false; 
        GetComponent<CapsuleCollider2D>().enabled = false; 
        
        Transform hurtbox = transform.Find("Hurtbox");
        if(hurtbox != null)
        {
            hurtbox.gameObject.SetActive(false);
        }
        
        Destroy(gameObject, 5.0f); 
    }
}