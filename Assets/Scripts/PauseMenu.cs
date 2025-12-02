using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // Conectados no Inspector
    public GameObject painelPausaUI;
    public GameObject painelComandosUI;

    public static bool JogoEstaPausado = false;

    void Update()
    {
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

    // Chamada pela tecla ESC
    public void PausarJogo()
    {
        painelPausaUI.SetActive(true); // Ativa o menu principal
        painelComandosUI.SetActive(false); // Garante que o sub-menu esteja escondido
        Time.timeScale = 0f; // PAUSA O JOGO
        JogoEstaPausado = true;
    }

    // Chamada pelo Botão JOGAR
    public void ContinuarJogo()
    {
        painelPausaUI.SetActive(false);
        painelComandosUI.SetActive(false);
        Time.timeScale = 1f; // RETORNA O JOGO
        JogoEstaPausado = false;

        // Se o player estiver desativado por causa da morte, reativa-o (boa prática)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && !player.activeSelf)
        {
            player.SetActive(true);
        }
    }

    // Chamada pelo Botão COMANDOS
    public void AbrirPainelComandos()
    {
        painelPausaUI.SetActive(false); // Esconde o pergaminho
        painelComandosUI.SetActive(true); // Mostra o sub-menu
    }

    // Chamada pelo Botão VOLTAR (no sub-menu)
    public void FecharPainelComandos()
    {
        painelComandosUI.SetActive(false); // Esconde o sub-menu
        painelPausaUI.SetActive(true); // Mostra o pergaminho
    }

    // Chamada pelo Botão SAIR
    public void SairDoJogo()
    {
        Time.timeScale = 1f;
#if UNITY_EDITOR
        Debug.Log("O jogo foi encerrado (no Editor)");
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}