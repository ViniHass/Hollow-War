using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable 
{
    [SerializeField] private ItemData itemToGive;

    // Propriedade pública para que o GameManager possa ler o nome do item
    public ItemData ItemToGive => itemToGive;

    public void Interact(Inventory inventory)
    {
        inventory.AddItem(itemToGive);
        
        // Notifica o GameManager que o item foi pego para que não reapareça
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MarkItemAsPicked(itemToGive.itemName);
        }

        Destroy(gameObject); // O item some imediatamente do mundo
    }

    public string GetPromptMessage()
    {
        if (itemToGive != null)
        {
            UIManager.Instance.ShowGlobalMessage("Pessione E para pegar item", 3.0f);
        }
        return "Apertar 'E' para pegar item";
    }
}