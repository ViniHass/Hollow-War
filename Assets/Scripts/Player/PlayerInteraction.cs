using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private Inventory inventory;
    private IInteractable currentInteractable; // O objeto com o qual podemos interagir agora

    void Awake()
    {
        inventory = GetComponent<Inventory>();
        if (inventory == null)
        {
            Debug.LogError("Componente Inventory não encontrado no PlayerInteraction.");
        }
    }

    // Método auxiliar para exibir ou limpar a mensagem de interação
    private void UpdateInteractionPrompt(bool show, string message = "")
    {
        if (UIManager.Instance == null)
        {
            // Fallback para debug
            if (show) Debug.Log("Pode interagir: " + message);
            else Debug.Log("Alvo de interação removido.");
            return;
        }

        if (show)
        {
            // Duração alta para manter o prompt na tela (999s)
            UIManager.Instance.ShowGlobalMessage(message, 999f); 
        }
       
    }

    void Update()
    {
        // Lógica de interação com 'E'
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            // 1. O IInteractable chama Interact(inventory).
            //    -> Se o item estiver faltando, o NPCQuest/BridgeRepair exibirá a mensagem de "caixinha" correta.
            currentInteractable.Interact(inventory);
            
            // 2. Limpar o prompt de Interação
            //    Após a interação (diálogo, falha na entrega, sucesso), o prompt "Pressione 'E'" deve sumir, 
            //    deixando a caixa livre para exibir apenas a mensagem de feedback da Quest.
            UpdateInteractionPrompt(false); 
        }

        // Lógica para mostrar inventário com 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory != null)
            {
                inventory.DisplayItems();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            
            // 🌟 Exibe a mensagem de interação na tela
            string prompt = currentInteractable.GetPromptMessage();
            // A mensagem completa inclui a instrução para o jogador
            UpdateInteractionPrompt(true, "Pressione 'E' para: " + prompt); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
            
            // 🌟 Limpa a mensagem de interação da tela
            UpdateInteractionPrompt(false);
        }
    }
}