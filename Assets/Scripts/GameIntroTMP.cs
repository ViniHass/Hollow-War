using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameIntroTMP : MonoBehaviour
{
    public GameObject painelComBotoes;

    [Header("Referências UI (TextMeshPro)")]
    public TMP_Text textoInicial;
    public TMP_Text textoPrologo;
    public GameObject painelFinal;
    public AudioSource audioIntro;

    [Header("Tempos (segundos)")]
    public float tempoTextoInicial = 3f;
    public float tempoPrologo = 5f;

    void Start()
    {
        if (painelComBotoes != null)
        {
            painelComBotoes.SetActive(true);
        }

        if (textoInicial != null)
        {
            textoInicial.gameObject.SetActive(false);
        }

        if (textoPrologo != null)
        {
            textoPrologo.gameObject.SetActive(false);
        }
    }

    public void IniciarJogo()
    {
        StartCoroutine(RunIntroSequence());
    }

    IEnumerator RunIntroSequence()
    {
        // 1️⃣ Sequência de textos
        if (painelComBotoes != null)
        {
            painelComBotoes.SetActive(false);
        }

        if (audioIntro != null)
        {
            audioIntro.Play();
        }

        if (textoInicial != null)
        {
            textoInicial.gameObject.SetActive(true);
            yield return new WaitForSeconds(tempoTextoInicial);
            textoInicial.gameObject.SetActive(false);
        }

        if (textoPrologo != null)
        {
            textoPrologo.gameObject.SetActive(true);
            yield return new WaitForSeconds(tempoPrologo);
            textoPrologo.gameObject.SetActive(false);
        }

        // 2️⃣ CARREGAMENTO CORRETO - SEM USAR SINGLE

        // Guarda referência da cena atual (Abertura)
        Scene aberturaScene = SceneManager.GetActiveScene();

        // Carrega Overworld de forma ADITIVA
        AsyncOperation loadOverworld = SceneManager.LoadSceneAsync("Overworld", LoadSceneMode.Additive);
        yield return loadOverworld;

        // Carrega Pradaria de forma ADITIVA
        AsyncOperation loadPradaria = SceneManager.LoadSceneAsync("Pradaria", LoadSceneMode.Additive);
        yield return loadPradaria;

        // Define Overworld como cena ativa
        Scene overworldScene = SceneManager.GetSceneByName("Overworld");
        SceneManager.SetActiveScene(overworldScene);

        // Agora descarrega a cena de Abertura
        yield return SceneManager.UnloadSceneAsync(aberturaScene);

        Debug.Log("Overworld e Pradaria carregadas. Abertura removida.");
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }
}