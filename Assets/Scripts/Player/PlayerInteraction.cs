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
            // Duração alta para manter o prompt na tela (3s é suficiente)
            UIManager.Instance.ShowGlobalMessage(message, 3.0f); 
        }
        else
        {
            // Limpa a mensagem imediatamente
            UIManager.Instance.HideGlobalMessage();
        }
    }

    void Update()
    {
        // Lógica de interação com 'E'
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            // Limpa o prompt ANTES da interação
            UpdateInteractionPrompt(false);
            
            // O IInteractable chama Interact(inventory).
            // Se o item for coletado, ele será destruído e automaticamente
            // sairá do trigger, limpando a referência
            currentInteractable.Interact(inventory);
            
            // Limpa a referência local também
            currentInteractable = null;
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
            
            // Exibe a mensagem de interação na tela
            string prompt = currentInteractable.GetPromptMessage();
            UpdateInteractionPrompt(true, prompt); 
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<IInteractable>() == currentInteractable)
        {
            currentInteractable = null;
            
            // Limpa a mensagem de interação da tela
            UpdateInteractionPrompt(false);
        }
    }
}