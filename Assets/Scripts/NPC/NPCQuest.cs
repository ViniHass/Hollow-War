using Unity.VisualScripting;
using UnityEngine;

public class NPCQuest : MonoBehaviour, IInteractable {

    [Header("Referência ao Sistema de Diálogo")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Diálogos baseados no estado")]
    [SerializeField] private DialogueData dialogueNotStarted;       // Primeira conversa
    [SerializeField] private DialogueData dialogueNoItem;           // Jogador não tem o item ainda
    [SerializeField] private DialogueData dialogueCompleted;        // Jogador entrega o item (primeira finalização)
    [SerializeField] private DialogueData dialogueCompletedNoItem;  // Conversa depois de já ter finalizado

    private enum QuestState { NotStarted, Started, CompletedNoItem, Completed }
    private QuestState state = QuestState.NotStarted;

    [Header("Item Necessário para Completar")]
    [SerializeField] private ItemData requiredItem;

    [Header("Recompensa (Opcional)")]
    [SerializeField] private ItemData rewardItem;

    void Start() {
        if (dialogueSystem == null)
            Debug.LogError("NPCQuest: DialogueSystem não atribuído!");
        if (requiredItem == null)
            Debug.LogWarning("NPCQuest: RequiredItem não atribuído!");
    }

    public string GetPromptMessage() {
        switch (state) {
            case QuestState.NotStarted:
                return "Falar";
            case QuestState.Started:
                return "Entregar Item";
            case QuestState.CompletedNoItem:
            case QuestState.Completed:
                return "Conversar";
            default:
                return "Interagir";
        }
    }

    public void Interact(Inventory inventory) {
        if (dialogueSystem == null) {
            Debug.LogError("NPCQuest: DialogueSystem não configurado!");
            return;
        }

        // Evita reiniciar o diálogo se já está rolando
        if (dialogueSystem.IsActive()) return;

        switch (state) {

            case QuestState.NotStarted:
                StartQuest();
                break;

            case QuestState.Started:
                CheckQuestCompletion(inventory);
                break;

            case QuestState.Completed:
                ShowCompletionDialogue();
               
                break;

            case QuestState.CompletedNoItem:
                ShowPostCompletionDialogue();
                break;
        }
    }

    void StartQuest() {
        if (dialogueNotStarted != null) {
            dialogueSystem.SetDialogue(dialogueNotStarted);
            dialogueSystem.StartDialogue();
            state = QuestState.Started;
        } else {
            Debug.LogWarning("NPCQuest: dialogueNotStarted não atribuído!");
        }
    }

    void CheckQuestCompletion(Inventory inventory) {
        if (inventory == null) {
            Debug.LogError("NPCQuest: Inventory está null!");
            return;
        }

        if (requiredItem == null) {
            Debug.LogError("NPCQuest: RequiredItem não configurado!");
            return;
        }

        if (inventory.HasItem(requiredItem) && state == QuestState.Started) {
            // O jogador tem o item → entrega
            inventory.RemoveItem(requiredItem);
            state = QuestState.CompletedNoItem;

            if (rewardItem != null)
                inventory.AddItem(rewardItem);

            // Mostra o diálogo de finalização pela primeira vez
            dialogueSystem.SetDialogue(dialogueCompleted);
            dialogueSystem.StartDialogue();

          
        } 
        else  {
            // Jogador ainda não encontrou o item
            dialogueSystem.SetDialogue(dialogueNoItem);
            dialogueSystem.StartDialogue();
        }
    }

    void ShowCompletionDialogue() {
        dialogueSystem.SetDialogue(dialogueCompleted);
        dialogueSystem.StartDialogue();
        
    }

    void ShowPostCompletionDialogue() {
        if (dialogueCompletedNoItem != null) {
            dialogueSystem.SetDialogue(dialogueCompletedNoItem);
            dialogueSystem.StartDialogue();
        }
    }

    // Método útil para testar
    [ContextMenu("Reset Quest")]
    public void ResetQuest() {
        state = QuestState.NotStarted;
    }
}
