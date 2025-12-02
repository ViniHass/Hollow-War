using UnityEngine;

public class DaggerProjectile : MonoBehaviour
{
    private Vector2 direction;
    private float speed;
    private int damage;
    private float lifetime;
    private float spawnTime;
    
    public void Initialize(Vector2 dir, float projectileSpeed, int projectileDamage, float projectileLifetime)
    {
        direction = dir.normalized;
        speed = projectileSpeed;
        damage = projectileDamage;
        lifetime = projectileLifetime;
        spawnTime = Time.time;
        
        // Rotaciona a adaga para apontar na direção do movimento
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    void Update()
    {
        // Move a adaga
        transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
        
        // Destrói após o tempo de vida
        if (Time.time >= spawnTime + lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se colidiu com um inimigo
        if (other.CompareTag("Enemy"))
        {
            // 1. Tenta achar o script de vida de um inimigo normal
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                Vector2 knockbackDirection = direction;
                enemyHealth.TakeDamage(damage, knockbackDirection);
                Destroy(gameObject);
                return;
            }
            
            // 2. Tenta achar o script de vida do BOSS
            SkeletonKing_Health bossHealth = other.GetComponentInParent<SkeletonKing_Health>();
            if (bossHealth != null)
            {
                Vector2 knockbackDirection = direction;
                bossHealth.TakeDamage(damage, knockbackDirection);
                Destroy(gameObject);
                return;
            }
            
            // 3. Tenta achar o script de vida do SLIME BOSS
            BossSlime_Health slimeBossHealth = other.GetComponentInParent<BossSlime_Health>();
            if (slimeBossHealth != null)
            {
                Vector2 knockbackDirection = direction;
                slimeBossHealth.TakeDamage(damage, knockbackDirection);
                Destroy(gameObject);
                return;
            }
        }
        
        // Verifica se colidiu com uma parede ou obstáculo
        if (other.CompareTag("Wall") || other.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
        {
            Destroy(gameObject);
        }
    }
}
