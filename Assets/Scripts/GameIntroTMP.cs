using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Importa o TextMeshPro
using System.Collections;

public class GameIntroTMP : MonoBehaviour
{

    public void IniciarJogo()
    {
        SceneManager.LoadScene("Overworld");

        // Carrega a segunda cena sem descarregar a primeira
        SceneManager.LoadScene("Pradaria", LoadSceneMode.Additive);
    }

    public void SairDoJogo()
    {
        Application.Quit();
    }

    [Header("Referências UI (TextMeshPro)")]
    public TMP_Text textoInicial;
    public TMP_Text textoPrologo;
    public GameObject painelFinal; // contém imagem e botões
    public AudioSource audioIntro;

    [Header("Tempos (segundos)")]
    public float tempoTextoInicial = 3f;
    public float tempoPrologo = 5f;

    void Start()
    {
        // Estado inicial
        textoInicial.gameObject.SetActive(true);
        textoPrologo.gameObject.SetActive(false);
        painelFinal.SetActive(false);

        // Começa a sequência de abertura
        StartCoroutine(SequenciaIntro());
    }

    IEnumerator SequenciaIntro()
    {
        // 1️⃣ Mostra “Desenvolvido por StackOverGames”
        yield return new WaitForSeconds(tempoTextoInicial);
        textoInicial.gameObject.SetActive(false);

        // 2️⃣ Mostra o prólogo
        textoPrologo.gameObject.SetActive(true);
        yield return new WaitForSeconds(tempoPrologo);
        textoPrologo.gameObject.SetActive(false);

        // 3️⃣ Mostra painel final (imagem + botões) e toca som
        painelFinal.SetActive(true);
        if (audioIntro != null) audioIntro.Play();
    }
}
