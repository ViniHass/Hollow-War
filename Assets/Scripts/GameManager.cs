using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // Evento para notificar o Game Over (usado por NPCQuest)
    public static event System.Action OnGameOver;
    public static GameManager Instance;

    [Header("Configurações de Vidas")]
    public int vidasAtuais = 3;

    [Header("UI de Vidas")]
    public Image[] coracoesUI;
    public Sprite coracaoCheioSprite;
    public Sprite coracaoVazioSprite;

    [Header("Configurações de Game Over")]
    public GameObject painelGameOver;
    public string nomeCenaRespawn = "Overworld";

    [Header("Configurações de Respawn")]
    public Vector3 respawnOffset = new Vector3(0, 2, 0);
    public float tempoAntesRespawn = 1f;
    
    [Header("Audio de Respawn")]
    [SerializeField] private AudioClip respawnSound;
    [Range(0f, 2f)]
    [SerializeField] private float respawnVolume = 1f;
    
    [Header("Referências")]
    public GameObject player;

    [Header("📊 Stats do Jogador")]
    [Tooltip("Arraste o ScriptableObject PlayerStats aqui")]
    [SerializeField] private PlayerStats playerStats;
    
    // Guarda a última posição válida do player (checkpoint)
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;

    // Sistema de Persistência
    private Dictionary<string, int> questStates = new Dictionary<string, int>();
    private Dictionary<string, bool> collectedItems = new Dictionary<string, bool>();

    // 💾 Backup dos Stats Originais do Jogador (valores primitivos)
    private float originalMoveSpeed;
    private int originalMaxHealth;
    private int originalAttackDamage;
    private float originalAttackHitboxDelay;
    private float originalAttackHitboxActiveTime;
    private float originalAttackAnimationDuration;
    private float originalDecoyDuration;
    private float originalDecoyCooldown;
    private float originalDecoyDestructionAnimTime;
    private bool hasStatsBackup = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("🎮 GameManager.Start() iniciado");
        
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        // Fazer backup dos stats originais no início do jogo
        Debug.Log("💾 Tentando fazer backup dos stats...");
        BackupOriginalStats();
        
        AtualizarUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndRemoveCollectedItems();
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
        // Garante que a posição inicial da cena seja o primeiro checkpoint válido
        if (player != null && !hasCheckpoint)
        {
            SetCheckpoint(player.transform.position);
            Debug.Log("✓ Checkpoint inicial (posição de spawn) definido na nova cena.");
        }

        // Fazer backup dos stats se ainda não foi feito
        if (!hasStatsBackup)
        {
            BackupOriginalStats();
        }
    }

    /// <summary>
    /// Faz backup dos valores originais do PlayerStats no início do jogo
    /// </summary>
    void BackupOriginalStats()
    {
        if (hasStatsBackup)
        {
            Debug.Log("⚠️ Backup já foi feito anteriormente. Pulando...");
            return;
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogWarning("⚠️ PlayerController não encontrado para fazer backup dos stats.");
            return;
        }

        // Usar Reflection para acessar o PlayerStats (que é private)
        System.Reflection.FieldInfo statsField = typeof(PlayerController).GetField("stats", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (statsField == null)
        {
            Debug.LogWarning("⚠️ Não foi possível acessar o campo 'stats' do PlayerController!");
            return;
        }
        
        PlayerStats currentStats = statsField.GetValue(playerController) as PlayerStats;
        
        if (currentStats == null)
        {
            Debug.LogWarning("⚠️ PlayerStats não está atribuído no PlayerController!");
            return;
        }

        // Salvar os valores como primitivos (não referência ao ScriptableObject)
        originalMoveSpeed = currentStats.moveSpeed;
        originalMaxHealth = currentStats.maxHealth;
        originalAttackDamage = currentStats.attackDamage;
        originalAttackHitboxDelay = currentStats.attackHitboxDelay;
        originalAttackHitboxActiveTime = currentStats.attackHitboxActiveTime;
        originalAttackAnimationDuration = currentStats.attackAnimationDuration;
        originalDecoyDuration = currentStats.decoyDuration;
        originalDecoyCooldown = currentStats.decoyCooldown;
        originalDecoyDestructionAnimTime = currentStats.decoyDestructionAnimTime;

        hasStatsBackup = true;
        
        Debug.Log($"💾 BACKUP DOS STATS ORIGINAIS REALIZADO:\n" +
                 $"  • Velocidade: {originalMoveSpeed}\n" +
                 $"  • Vida Máxima: {originalMaxHealth}\n" +
                 $"  • Dano: {originalAttackDamage}\n" +
                 $"  • Cooldown Decoy: {originalDecoyCooldown}s\n" +
                 $"  • Duração Decoy: {originalDecoyDuration}s");
    }

    /// <summary>
    /// Restaura os stats originais do jogador
    /// </summary>
    void RestoreOriginalStats()
    {
        if (!hasStatsBackup)
        {
            Debug.LogError("❌ NÃO HÁ BACKUP DE STATS PARA RESTAURAR!");
            return;
        }

        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("❌ PlayerController não encontrado para restaurar stats.");
            return;
        }

        // Usar Reflection para acessar o PlayerStats (que é private)
        System.Reflection.FieldInfo statsField = typeof(PlayerController).GetField("stats", 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (statsField == null)
        {
            Debug.LogError("❌ Campo 'stats' não encontrado via Reflection!");
            return;
        }
        
        PlayerStats currentStats = statsField.GetValue(playerController) as PlayerStats;
        
        if (currentStats == null)
        {
            Debug.LogError("❌ PlayerStats é null!");
            return;
        }

        // Log dos valores ANTES da restauração
        Debug.Log($"📊 STATS ANTES DA RESTAURAÇÃO:\n" +
                 $"  • Velocidade: {currentStats.moveSpeed}\n" +
                 $"  • Vida Máxima: {currentStats.maxHealth}\n" +
                 $"  • Dano: {currentStats.attackDamage}\n" +
                 $"  • Cooldown Decoy: {currentStats.decoyCooldown}s\n" +
                 $"  • Duração Decoy: {currentStats.decoyDuration}s");

        // Restaurar os valores do backup (primitivos salvos)
        currentStats.moveSpeed = originalMoveSpeed;
        currentStats.maxHealth = originalMaxHealth;
        currentStats.attackDamage = originalAttackDamage;
        currentStats.attackHitboxDelay = originalAttackHitboxDelay;
        currentStats.attackHitboxActiveTime = originalAttackHitboxActiveTime;
        currentStats.attackAnimationDuration = originalAttackAnimationDuration;
        currentStats.decoyDuration = originalDecoyDuration;
        currentStats.decoyCooldown = originalDecoyCooldown;
        currentStats.decoyDestructionAnimTime = originalDecoyDestructionAnimTime;

        // Log dos valores DEPOIS da restauração
        Debug.Log($"🔄 STATS RESTAURADOS AOS VALORES ORIGINAIS:\n" +
                 $"  • Velocidade: {currentStats.moveSpeed} (era {originalMoveSpeed})\n" +
                 $"  • Vida Máxima: {currentStats.maxHealth} (era {originalMaxHealth})\n" +
                 $"  • Dano: {currentStats.attackDamage} (era {originalAttackDamage})\n" +
                 $"  • Cooldown Decoy: {currentStats.decoyCooldown}s (era {originalDecoyCooldown}s)\n" +
                 $"  • Duração Decoy: {currentStats.decoyDuration}s (era {originalDecoyDuration}s)");
    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpointPosition = position;
        hasCheckpoint = true;
        Debug.Log($"✓ Checkpoint definido em: {position}");
    }

    public void PersonagemMorreu()
    {
        vidasAtuais--;
        AtualizarUI();

        if (vidasAtuais <= 0)
        {
            GameOver();
        }
        else
        {
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) yield break;
        }
        
        // Desativa o Player imediatamente
        player.SetActive(false); 
        
        yield return new WaitForSeconds(tempoAntesRespawn);

        // Se houver um checkpoint ativo, reposiciona o Player
        if (hasCheckpoint)
        {
            Health healthComponent = player.GetComponent<Health>() ?? player.GetComponentInChildren<Health>();

            Vector3 respawnPos = lastCheckpointPosition + respawnOffset;
            
            player.transform.position = respawnPos;
            player.SetActive(true);

            // 🎵 Reproduz o som de respawn DEPOIS de reativar o player
            if (AudioManager.Instance != null && respawnSound != null)
            {
                AudioManager.Instance.PlaySound(respawnSound, player.transform.position, respawnVolume);
            }

            if (healthComponent != null)
            {
                healthComponent.RestoreHealthFull();
            }
            
            Debug.Log($"→ Respawnando no checkpoint: {lastCheckpointPosition}");
        }
        else
        {
            // 🚨 Ação sem checkpoint: Recarrega a cena de respawn (posição de spawn padrão)
            Debug.LogWarning("⚠ Nenhum checkpoint ativo. Recarregando cena de spawn.");
            SceneManager.LoadScene(nomeCenaRespawn);
        }
    }

    void AtualizarUI()
    {
        if (coracoesUI == null || coracoesUI.Length == 0) return;

        for (int i = 0; i < coracoesUI.Length; i++)
        {
            if (coracoesUI[i] == null) continue;

            coracoesUI[i].sprite = (i < vidasAtuais) ? coracaoCheioSprite : coracaoVazioSprite;
        }
    }

    void GameOver()
    {
        Debug.Log("💀 GAME OVER ACIONADO!");
        
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
            Time.timeScale = 0f;
        }

        // 🔄 Restaurar stats originais ANTES de disparar o evento
        Debug.Log("🔄 Iniciando restauração de stats...");
        RestoreOriginalStats();

        // Disparar evento de Game Over (NPCQuest irá resetar as quests)
        if (OnGameOver != null)
        {
            Debug.Log("📢 Disparando evento OnGameOver...");
            OnGameOver();
        }

        ResetAllQuests();
    }
    
    // ========== SISTEMA DE PERSISTÊNCIA ==========

    public void MarkItemAsPicked(string itemName)
    {
        if (!collectedItems.ContainsKey(itemName))
        {
            collectedItems.Add(itemName, true);
        }
        else
        {
            collectedItems[itemName] = true;
        }
    }

    void CheckAndRemoveCollectedItems()
    {
        ItemPickup[] itemsInScene = FindObjectsOfType<ItemPickup>();
        
        foreach (ItemPickup item in itemsInScene)
        {
            if (item.ItemToGive == null) continue;

            string itemName = item.ItemToGive.itemName;
            
            if (collectedItems.ContainsKey(itemName))
            {
                Destroy(item.gameObject);
            }
        }
    }

    public void SaveQuestState(string npcId, int questState)
    {
        if (questStates.ContainsKey(npcId))
        {
            questStates[npcId] = questState;
        }
        else
        {
            questStates.Add(npcId, questState);
        }
    }

    public int LoadQuestState(string npcId)
    {
        if (questStates.ContainsKey(npcId))
        {
            return questStates[npcId];
        }
        return -1;
    }

    void ResetAllQuests()
    {
        questStates.Clear();
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        vidasAtuais = 3;
        ResetAllQuests();
        collectedItems.Clear();
        hasCheckpoint = false;
        lastCheckpointPosition = Vector3.zero;

        // 🔄 Restaurar stats originais ao reiniciar
        RestoreOriginalStats();

        SceneManager.LoadScene(nomeCenaRespawn);
        
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        // Verifica se a tela de Game Over está ativa E se a tecla R foi pressionada
        // Se o painel estiver ativo, o jogo está pausado (Time.timeScale = 0f)
        if (painelGameOver != null && painelGameOver.activeSelf && Input.GetKeyDown(KeyCode.R))
        {
            Destroy(player);
            SceneManager.LoadScene("Abertura");
        }
    }
}