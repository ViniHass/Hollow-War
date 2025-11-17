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

    [Header("🌟 Power-up ao Completar Quest")]
    [Tooltip("Se marcado, ao completar esta quest os stats do jogador serão aumentados.")]
    [SerializeField] private bool grantsPowerUp = false;
    
    [SerializeField] private float moveSpeedBonus = 0f;
    [SerializeField] private int maxHealthBonus = 0;
    [SerializeField] private int attackDamageBonus = 0;
    [SerializeField] private float decoyCooldownReduction = 0f;
    [SerializeField] private float decoyDurationBonus = 0f;

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
    
    private void ShowGlobalQuestMessage(string message)
    {
        if (UIManager.Instance != null)
        {
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
            case QuestState.NotStarted: return "Aperte E para Falar";
            case QuestState.Started: return "Aperte E para Entregar Item";
            case QuestState.CompletedNoItem:
            case QuestState.Completed: return "Aperte E para Conversar";
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
            
            // 🌟 APLICAR POWER-UP se esta quest concede
            if (grantsPowerUp)
            {
                ApplyPowerUp();
                rewardMessage += " ⚡ PODERES AUMENTADOS!";
            }
            
            ShowGlobalQuestMessage($"Quest CONCLUÍDA!{rewardMessage}");

            dialogueSystem.SetDialogue(dialogueCompleted);
            dialogueSystem.StartDialogue();
            SaveQuestStateToGameManager();
        } 
        else 
        {
            ShowGlobalQuestMessage($"INSTRUÇÃO: Eu ainda estou esperando pelo '{requiredItem.itemName}'.");
            
            dialogueSystem.SetDialogue(dialogueNoItem);
            dialogueSystem.StartDialogue();
        }
    }

    void ApplyPowerUp()
    {
        // Encontrar o PlayerController na cena
        PlayerController playerController = FindObjectOfType<PlayerController>();
        
        if (playerController == null)
        {
            Debug.LogError("❌ PlayerController não encontrado na cena!");
            return;
        }

        // Usar Reflection para acessar o PlayerStats (que é private)
        System.Reflection.FieldInfo statsField = typeof(PlayerController).GetField("stats", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (statsField == null)
        {
            Debug.LogError("❌ Não foi possível acessar o campo 'stats' do PlayerController!");
            return;
        }
        
        PlayerStats stats = statsField.GetValue(playerController) as PlayerStats;
        
        if (stats == null)
        {
            Debug.LogError("❌ PlayerStats não está atribuído no PlayerController!");
            return;
        }

        // Log ANTES das mudanças
        Debug.Log($"📊 STATS ANTES DO POWER-UP:\n" +
                 $"  • Velocidade: {stats.moveSpeed}\n" +
                 $"  • Vida Máxima: {stats.maxHealth}\n" +
                 $"  • Dano: {stats.attackDamage}\n" +
                 $"  • Cooldown Decoy: {stats.decoyCooldown}s\n" +
                 $"  • Duração Decoy: {stats.decoyDuration}s");

        // Aplicar bônus ao ScriptableObject
        stats.moveSpeed += moveSpeedBonus;
        stats.maxHealth += maxHealthBonus;
        stats.attackDamage += attackDamageBonus;
        stats.decoyCooldown = Mathf.Max(0, stats.decoyCooldown - decoyCooldownReduction);
        stats.decoyDuration += decoyDurationBonus;

        // Log DEPOIS das mudanças
        Debug.Log($"⚡ POWER-UP APLICADO COM SUCESSO!\n" +
                 $"  • Velocidade: {stats.moveSpeed} (+{moveSpeedBonus})\n" +
                 $"  • Vida Máxima: {stats.maxHealth} (+{maxHealthBonus})\n" +
                 $"  • Dano: {stats.attackDamage} (+{attackDamageBonus})\n" +
                 $"  • Cooldown Decoy: {stats.decoyCooldown}s (-{decoyCooldownReduction}s)\n" +
                 $"  • Duração Decoy: {stats.decoyDuration}s (+{decoyDurationBonus}s)");

        // Atualizar a vida do jogador se ganhou vida máxima
        if (maxHealthBonus > 0)
        {
            Health playerHealth = playerController.GetComponent<Health>();
            
            if (playerHealth != null)
            {
                playerHealth.IncreaseMaxHealth(maxHealthBonus);
            }
            else
            {
                Debug.LogWarning("⚠️ Componente Health do jogador não encontrado! Os stats foram aumentados, mas a vida atual não foi curada.");
            }
        }
    }

    void ShowCompletionDialogue() 
    {
        dialogueSystem.SetDialogue(dialogueCompleted);
        dialogueSystem.StartDialogue();
        
        ShowGlobalQuestMessage("Quest concluída. Não há mais tarefas aqui.");
    }

    void ShowPostCompletionDialogue() 
    {
        if (dialogueCompletedNoItem != null) 
        {
            dialogueSystem.SetDialogue(dialogueCompletedNoItem);
            dialogueSystem.StartDialogue();
            
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