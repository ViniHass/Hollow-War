using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // VARIÁVEIS PÚBLICAS (Aparecerão no Inspector)

    // GameObject é o tipo de objeto que representa um painel inteiro na sua UI.
    public GameObject painelPausaUI;
    public GameObject painelComandosUI;

    // Variável para rastrear o estado de pausa.
    public static bool JogoEstaPausado = false;

    void Update()
    {
        // Detecta a tecla Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (JogoEstaPausado)
            {
                ContinuarJogo();
            }
            else
            {
                PausarJogo();
            }
        }
    }

    public void PausarJogo()
    {
        painelPausaUI.SetActive(true); // Ativa o painel: Torna-o visível
        painelComandosUI.SetActive(false); // Garante que o painel de comandos esteja escondido
        Time.timeScale = 0f; // Pausa o tempo do jogo
        JogoEstaPausado = true;
    }

    public void ContinuarJogo()
    {
        painelPausaUI.SetActive(false); // Desativa o painel: Torna-o invisível
        painelComandosUI.SetActive(false);
        Time.timeScale = 1f; // Volta o tempo normal
        JogoEstaPausado = false;
    }

    // PauseMenu.cs

    // ... (seus métodos anteriores)

    // Chamada APENAS pelo Botão "COMANDOS"
    public void AbrirPainelComandos()
    {
        painelPausaUI.SetActive(false); // Esconde o painel principal (pergaminho)
        painelComandosUI.SetActive(true); // Mostra o painel de comandos
    }

    // Chamada APENAS pelo Botão "VOLTAR"
    public void FecharPainelComandos()
    {
        painelComandosUI.SetActive(false); // Esconde o painel de comandos
        painelPausaUI.SetActive(true); // Mostra o painel principal (pergaminho)
    }

    public void SairDoJogo()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        Debug.Log("O jogo foi encerrado (no Editor)");
        // Para simular o encerramento no Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Fecha o aplicativo no build final
            Application.Quit();
#endif
    }

}