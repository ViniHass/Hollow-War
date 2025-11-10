using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se colidiu com um Hurtbox com a tag "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // 1. Tenta achar o script de vida de um inimigo normal
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                // Inimigo normal encontrado!
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                enemyHealth.TakeDamage(damage, knockbackDirection);
            }
            else
            {
                // 2. Se falhar, tenta achar o script de vida do BOSS
                SkeletonKing_Health bossHealth = other.GetComponentInParent<SkeletonKing_Health>();
                if (bossHealth != null)
                {
                    // Boss encontrado!
                    // O boss não usa knockback, mas a função precisa dele
                    Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
                    bossHealth.TakeDamage(damage, knockbackDirection);
                }
            }
        }
    }
}