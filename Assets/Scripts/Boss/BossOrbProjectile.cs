using UnityEngine;

public class BossOrbProjectile : MonoBehaviour
{
    private float speed;
    private float lifetime;
    private int damage;
    private Vector2 moveDirection;
    
    private bool isMoving = false;

    // Função "Setup" que o boss vai chamar
    public void Setup(float newSpeed, float newLifetime, int newDamage, Vector2 dir)
    {
        speed = newSpeed;
        lifetime = newLifetime;
        damage = newDamage;
        moveDirection = dir.normalized;
        
        Destroy(gameObject, lifetime);
    }

    // O boss chamará isso após o "delay"
    public void Launch()
    {
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    // Verificando a colisão (Hitbox)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isMoving) return; 

        // 1. Verificando a TAG
        // Conforme seu pedido, checamos se o que atingimos tem a tag "Player"
        // (Esta deve ser a tag da Hurtbox do seu player)
        if (other.CompareTag("Player"))
        {
            // 2. Dando o Dano
            // Usamos seu script "Health.cs".
            // Assumimos que a Hurtbox é filha do objeto principal do Player
            // que contém o script "Health".
            Health playerHealth = other.GetComponentInParent<Health>(); //
            
            if (playerHealth != null)
            {
                // Chamamos a função pública TakeDamage do seu script
                playerHealth.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("Orbe atingiu o Player, mas não achou script 'Health' no pai!");
            }
            
            // 3. Destruindo a Orbe
            Destroy(gameObject); 
        }
    }
}