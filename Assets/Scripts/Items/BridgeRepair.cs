using UnityEngine;

public class BridgeRepair : MonoBehaviour, IInteractable
{
    [Header("Configuração")]
    [SerializeField] private ItemData requiredItem;
    [SerializeField] private Sprite repairedSprite;
    [SerializeField] private Collider2D wallCollider; // O colisor que bloqueia a passagem

    private bool isRepaired = false;

    private void ShowMessage(string message)
    {
        // Método auxiliar para chamar o UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGlobalMessage(message);
        }
        else
        {
            // Fallback para debug caso o UIManager não esteja na cena
            Debug.Log(message);
        }
    }

    public void Interact(Inventory inventory)
    {
        if (isRepaired) return; // Já foi reparada, não faz nada

        // Verifica se o jogador tem o item necessário
        if (inventory.HasItem(requiredItem))
        {
            inventory.RemoveItem(requiredItem); // Usa o item
            
            // Repara a ponte
            isRepaired = true;
            GetComponent<SpriteRenderer>().sprite = repairedSprite;
            
            // Desativa a barreira de colisão
            if (wallCollider != null)
            {
                wallCollider.enabled = false;
            }
            
            // 🌟 Substituição do Log pela Caixa de Mensagem
            ShowMessage("A ponte foi consertada!");
        }
        else
        {
            // 🌟 Substituição do Log pela Caixa de Mensagem
            ShowMessage("Você precisa de '" + requiredItem.itemName + "' para consertar a ponte.");
        }
    }

    public string GetPromptMessage()
    {
        if (isRepaired)
        {
            return "A ponte está consertada.";
        }
        return "Aperte E para consertar";
    }
}