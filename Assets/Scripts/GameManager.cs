using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;


public class GameManager : MonoBehaviour
{
    // NOVO: Evento estático para notificar outros scripts (como NPCs) que o Game Over ocorreu.
    public static event System.Action OnGameOver;

    // 1. O Padrão Singleton: Permite que outros scripts o encontrem facilmente.
    public static GameManager Instance;

    [Header("Configurações de Vidas")]
    public int vidasAtuais = 3;

    // Arrays e Sprites para a UI
    public Image[] coracoesUI;
    public Sprite coracaoCheioSprite;
    public Sprite coracaoVazioSprite;

    [Header("Configurações de Game Over")]
    public GameObject painelGameOver;
    public string nomeCenaRespawn = "Overworld";

    [Header("Sistema de Persistência")]
    // Dicionário para salvar o estado das quests (chave = nome do NPC, valor = estado)
    private Dictionary<string, int> questStates = new Dictionary<string, int>();

    // Dicionário para armazenar os itens que foram coletados (chave = nome do item)
    private Dictionary<string, bool> collectedItems = new Dictionary<string, bool>();

    
    [Header("Referências")]
    public GameObject player;

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f); // opcional, tempo para animação de morte

        // Reativa o jogador
        player.SetActive(true);

        // Calcula ponto de respawn perto da morte
        Vector3 offset = new Vector3(4, 10, 2); // pode ser alterado
        Vector3 respawnPos = Health.lastDeathPosition + offset;

        player.transform.position = respawnPos;

        // Restaura vida
        player.GetComponent<Health>().RestoreHealthFull();
    }

    


    void Awake()
    {
        // O Singleton garante que apenas uma instância exista
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Adiciona um listener para quando uma cena é carregada
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
        AtualizarUI();
    }
    
    // Método chamado toda vez que uma cena é carregada
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Verifica se os itens coletados não devem reaparecer
        CheckAndRemoveCollectedItems();
    }
    
    // Função principal chamada pelo player quando ele perde a barra de vida
    // Seu método "Die" ou "Morte" seria uma chamada a este método.
    public void PersonagemMorreu()
    {
        vidasAtuais--;

        // 1. Atualiza a UI de corações
        AtualizarUI();

        // 2. Verifica se o jogo acabou
        if (vidasAtuais <= 0)
        {
            GameOver();
        }
        else
        {
            // Se ainda há vidas, recarrega a cena para "respawnar"
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        StartCoroutine(RespawnRoutine());
    }


    void AtualizarUI()
    {
        // Percorre os corações na UI
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            if (i < vidasAtuais)
            {
                // Coração Cheio (mostra que há vida)
                coracoesUI[i].sprite = coracaoCheioSprite;
                coracoesUI[i].enabled = true; // Garante que a imagem esteja ativa
            }
            else
            {
                // Coração Vazio (mostra que a vida foi perdida)
                coracoesUI[i].sprite = coracaoVazioSprite;
                coracoesUI[i].enabled = true;
            }
        }
    }

    void GameOver()
    {
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
            Time.timeScale = 0f; // Pausa o jogo
        }

        // Dispara o evento de Game Over 
        if (OnGameOver != null)
        {
            OnGameOver();
        }

        // Reseta todas as quests (o NPCQuest irá redefinir seu estado usando o evento OnGameOver)
        ResetAllQuests();
    }
    
    // MÉTODO: Marca um item como permanentemente coletado.
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
        Debug.Log($"Item {itemName} marcado como coletado.");
    }

    // MÉTODO: Verifica a cena e destrói ItemPickups que já foram coletados.
    void CheckAndRemoveCollectedItems()
    {
        ItemPickup[] itemsInScene = FindObjectsOfType<ItemPickup>();
        
        foreach (ItemPickup item in itemsInScene)
        {
            string itemName = item.ItemToGive.itemName; 
            
            if (collectedItems.ContainsKey(itemName))
            {
                // Se o item foi coletado, remove-o imediatamente da cena
                Destroy(item.gameObject);
                Debug.Log($"Item {itemName} removido ao carregar a cena, pois já havia sido coletado.");
            }
        }
    }


    // ========== SISTEMA DE PERSISTÊNCIA DE QUESTS ==========

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
        Debug.Log($"Quest de '{npcId}' salva com estado: {questState}");
    }

    public int LoadQuestState(string npcId)
    {
        if (questStates.ContainsKey(npcId))
        {
            return questStates[npcId];
        }
        return -1; // Indica que não há estado salvo
    }

    void ResetAllQuests()
    {
        // Limpa o dicionário de persistência
        questStates.Clear(); 
        Debug.Log("Todas as quests foram resetadas!");
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f; // Despausa o jogo
        vidasAtuais = 3;
        ResetAllQuests();
        
        // Limpa os itens coletados ao reiniciar o jogo
        collectedItems.Clear();

        SceneManager.LoadScene("abertura");
        //SceneManager.LoadScene("Pradaria", LoadSceneMode.Additive);
        
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
    }

    
}

