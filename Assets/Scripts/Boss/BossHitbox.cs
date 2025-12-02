using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour
{
    public int damage;
    
    // Lista para garantir que o player só tome dano UMA VEZ por "swing"
    private List<Collider2D> collidersHit;

    void OnEnable()
    {
        // Limpa a lista toda vez que o hitbox é ativado
        collidersHit = new List<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se já atingimos este collider, ignora
        if (collidersHit.Contains(other))
        {
            return;
        }

        // Verifica se é o Hurtbox do Player (tag "Player")
        if (other.CompareTag("Player"))
        {
            // Pega o script Health.cs no pai do hurtbox
            Health playerHealth = other.GetComponentInParent<Health>(); 
            
            if (playerHealth != null)
            {
                // Causa o dano
                playerHealth.TakeDamage(damage);
                
                // Adiciona na lista para não atingir de novo
                collidersHit.Add(other);
            }
        }
    }
}