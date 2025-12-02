using UnityEngine;
using System.Collections; // Importante para usar o IEnumerator

public class VictoryPanelController : MonoBehaviour
{
    // Variável para arrastar o Painel de Vitória (VictoryPanel)
    [SerializeField]
    private GameObject victoryPanel;

    // Tempo que o painel ficará visível em segundos
    [SerializeField]
    private float displayDuration = 4f;

    // Método que você chamará quando o jogador vencer.
    public void ShowVictoryPanel()
    {
        // 1. Garante que o painel comece desativado (se ainda não estiver).
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            // 2. Inicia a Corrotina para esperar e esconder o painel.
            StartCoroutine(HidePanelAfterDelay());
        }
    }

    // Corrotina (método que permite esperar por um tempo no Unity)
    private IEnumerator HidePanelAfterDelay()
    {
        // Espera pela duração definida (4 segundos)
        yield return new WaitForSeconds(displayDuration);

        // Esconde o painel após o tempo de espera
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
    }

    // Opcional: Para testar no editor, você pode chamar isso no Start()
    // public void Start()
    // {
    //     ShowVictoryPanel();
    // }
}