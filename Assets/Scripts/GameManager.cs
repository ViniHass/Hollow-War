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
    
    [Header("Referências")]
    public GameObject player;
    
    // Guarda a última posição válida do player (checkpoint)
    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;

    // Sistema de Persistência
    private Dictionary<string, int> questStates = new Dictionary<string, int>();
    private Dictionary<string, bool> collectedItems = new Dictionary<string, bool>();

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
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        
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
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
            Time.timeScale = 0f;
        }

        if (OnGameOver != null)
        {
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
            ReiniciarJogo();
        }
    }
}