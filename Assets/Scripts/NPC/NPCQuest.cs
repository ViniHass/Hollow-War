using UnityEngine;

public class NPCQuest : MonoBehaviour, IInteractable 
{
    [Header("Referência ao Sistema de Diálogo")]
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Diálogos baseados no estado")]
    [SerializeField] private DialogueData dialogueNotStarted;       
    [SerializeField] private DialogueData dialogueNoItem;           
    [SerializeField] private DialogueData dialogueCompleted;        
    [SerializeField] private DialogueData dialogueCompletedNoItem;  

    private enum QuestState { NotStarted, Started, CompletedNoItem, Completed }
    [SerializeField] private QuestState state = QuestState.NotStarted; 

    [Header("Item Necessário para Completar")]
    [SerializeField] private ItemData requiredItem;

    [Header("Recompensa (Opcional)")]
    [SerializeField] private ItemData rewardItem;

    [Header("Identificador Único do NPC")]
    [Tooltip("ID único para salvar o estado da quest.")]
    [SerializeField] private string npcId;

    void OnEnable() 
    {
        GameManager.OnGameOver += ResetQuestOnGameOver;
    }

    void OnDisable() 
    {
        GameManager.OnGameOver -= ResetQuestOnGameOver;
    }

    void Start() 
    {
        if (string.IsNullOrEmpty(npcId))
        {
            npcId = gameObject.name;
        }

        LoadQuestStateFromGameManager();
    }
    
    // Método auxiliar para usar o UIManager
    private void ShowGlobalQuestMessage(string message)
    {
        if (UIManager.Instance != null)
        {
            // Mensagens de quest com 3 segundos de duração.
            UIManager.Instance.ShowGlobalMessage(message, 3.0f); 
        }
        else
        {
            Debug.Log(message);
        }
    }

    void ResetQuestOnGameOver() 
    {
        state = QuestState.NotStarted;
        SaveQuestStateToGameManager();
        Debug.Log($"Quest do NPC {gameObject.name} resetada devido ao Game Over.");
    }


    void LoadQuestStateFromGameManager()
    {
        if (GameManager.Instance == null) return;

        int savedState = GameManager.Instance.LoadQuestState(npcId);
        
        if (savedState != -1)
        {
            state = (QuestState)savedState;
            Debug.Log($"Quest de '{npcId}' carregada com estado: {state}");
        }
    }

    void SaveQuestStateToGameManager()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SaveQuestState(npcId, (int)state);
        }
    }

    public string GetPromptMessage() 
    {
        switch (state) 
        {
            case QuestState.NotStarted: return "Falar";
            case QuestState.Started: return "Entregar Item";
            case QuestState.CompletedNoItem:
            case QuestState.Completed: return "Conversar";
            default: return "Interagir";
        }
    }

    public void Interact(Inventory inventory) 
    {
        if (dialogueSystem == null || dialogueSystem.IsActive()) return;

        switch (state) 
        {
            case QuestState.NotStarted: StartQuest(); break;
            case QuestState.Started: CheckQuestCompletion(inventory); break;
            case QuestState.Completed: ShowCompletionDialogue(); break;
            case QuestState.CompletedNoItem: ShowPostCompletionDialogue(); break;
        }
    }

    void StartQuest() 
    {
        if (dialogueNotStarted != null) 
        {
            dialogueSystem.SetDialogue(dialogueNotStarted);
            dialogueSystem.StartDialogue();
            state = QuestState.Started;
            SaveQuestStateToGameManager();
            
            // 🌟 INSTRUÇÃO: O que fazer após iniciar a quest
            ShowGlobalQuestMessage($"INSTRUÇÃO: Você precisa encontrar '{requiredItem.itemName}'.");
        } 
        else 
        {
            Debug.LogWarning("NPCQuest: dialogueNotStarted não atribuído!");
        }
    }

    void CheckQuestCompletion(Inventory inventory) 
    {
        if (inventory.HasItem(requiredItem) && state == QuestState.Started) 
        {
            inventory.RemoveItem(requiredItem);
            state = QuestState.CompletedNoItem;

            string rewardMessage = "";
            if (rewardItem != null) 
            {
                inventory.AddItem(rewardItem);
                rewardMessage = $" Recompensa: {rewardItem.itemName}!";
            }
            
            // 🌟 INSTRUÇÃO: Feedback de sucesso
            ShowGlobalQuestMessage($"Quest CONCLUÍDA!{rewardMessage}");

            dialogueSystem.SetDialogue(dialogueCompleted);
            dialogueSystem.StartDialogue();
            SaveQuestStateToGameManager();
        } 
        else 
        {
            // 🌟 INSTRUÇÃO: Feedback de item faltando
            ShowGlobalQuestMessage($"INSTRUÇÃO: Eu ainda estou esperando pelo '{requiredItem.itemName}'.");
            
            dialogueSystem.SetDialogue(dialogueNoItem);
            dialogueSystem.StartDialogue();
        }
    }

    void ShowCompletionDialogue() 
    {
        dialogueSystem.SetDialogue(dialogueCompleted);
        dialogueSystem.StartDialogue();
        
        // 🌟 INSTRUÇÃO: Quest completa
        ShowGlobalQuestMessage("Quest concluída. Não há mais tarefas aqui.");
    }

    void ShowPostCompletionDialogue() 
    {
        if (dialogueCompletedNoItem != null) 
        {
            dialogueSystem.SetDialogue(dialogueCompletedNoItem);
            dialogueSystem.StartDialogue();
            
            // 🌟 INSTRUÇÃO: Quest finalizada
            ShowGlobalQuestMessage("Missão finalizada. Siga para a próxima aventura!");
        }
    }

    [ContextMenu("Reset Quest")]
    public void ResetQuest() 
    {
        state = QuestState.NotStarted;
        SaveQuestStateToGameManager();
        Debug.Log($"Quest '{npcId}' resetada para NotStarted.");
    }
}