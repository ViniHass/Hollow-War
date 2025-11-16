using UnityEngine;
using TMPro; // Necessário para usar TextMeshPro
using System.Collections;

public class UIManager : MonoBehaviour
{
    // O Singleton para acesso fácil de qualquer script
    public static UIManager Instance;

    [Header("Configuração de Mensagens")]
    [Tooltip("Referência ao componente TextMeshPro que exibirá as mensagens.")]
    [SerializeField] private TextMeshProUGUI globalMessageText;

    [Tooltip("Duração padrão (em segundos) que a mensagem ficará visível.")]
    [SerializeField] private float defaultDisplayDuration = 2.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // Opcional: DontDestroyOnLoad(gameObject); se a UI deve persistir
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Garante que a caixinha (o Painel Pai) esteja escondida no início
        if (globalMessageText != null && globalMessageText.transform.parent != null)
        {
            globalMessageText.transform.parent.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Exibe uma mensagem na tela por um período de tempo, ativando a caixinha (Painel Pai).
    /// </summary>
    /// <param name="message">O texto a ser exibido.</param>
    /// <param name="duration">Duração da exibição. Se for 0 ou menor, usa a duração padrão.</param>
    public void ShowGlobalMessage(string message, float duration = 0f)
    {
        if (globalMessageText == null)
        {
            Debug.LogError("UIManager: globalMessageText não atribuído! A mensagem não será exibida.");
            return;
        }

        float actualDuration = (duration <= 0) ? defaultDisplayDuration : duration;

        // Interrompe qualquer coroutine anterior para exibir a nova mensagem imediatamente
        StopAllCoroutines(); 
        StartCoroutine(DisplayMessageRoutine(message, actualDuration));
    }

    IEnumerator DisplayMessageRoutine(string message, float duration)
    {
        GameObject panel = globalMessageText.transform.parent.gameObject;
        
        // 1. Configura a mensagem no componente de texto
        globalMessageText.text = message;
        
        // 2. Ativa o Painel Pai (a caixinha)
        if (panel != null)
        {
            panel.SetActive(true);
        }

        // 3. Espera pelo tempo definido
        yield return new WaitForSeconds(duration);

        // 4. Desativa o Painel Pai (a caixinha)
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }
}