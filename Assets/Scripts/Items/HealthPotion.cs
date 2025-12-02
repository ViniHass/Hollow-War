using UnityEngine;

public class HealthPotion : MonoBehaviour, IInteractable
{
    [Header("Configuração da Poção")]
    [Tooltip("Quantidade de vida que será recuperada")]
    [SerializeField] private int healAmount = 5;
    
    [Tooltip("Mensagem que aparece na tela")]
    [SerializeField] private string message = "Pressione E para beber poção";

    [Header("Audio (Opcional)")]
    [SerializeField] private AudioClip drinkSound;

    // Método exigido pela interface IInteractable
    public void Interact(Inventory inventory)
    {
        // Tentamos pegar o componente Health que está no mesmo objeto do Inventory (o Player)
        Health playerHealth = inventory.GetComponent<Health>();

        if (playerHealth != null)
        {
            // Chamamos o método Heal que já existe no seu Health.cs
            // Ele já calcula para não ultrapassar o máximo automaticamente
            playerHealth.Heal(healAmount);

            // Toca som se houver Audio Manager e clip configurado
            if (AudioManager.Instance != null && drinkSound != null)
            {
                AudioManager.Instance.PlaySound(drinkSound, transform.position, 1f);
            }

            // Destroi o objeto da poção após o uso
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("O objeto que tentou pegar a poção não tem componente Health!");
        }
    }

    // Método exigido pela interface IInteractable para mostrar mensagem na UI
    public string GetPromptMessage()
    {
        return message;
    }
}