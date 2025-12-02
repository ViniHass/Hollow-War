using UnityEngine;

/// <summary>
/// Script de exemplo/teste para demonstrar o uso do ItemHighlight.
/// 
/// Adicione este script a um item na cena para testar os efeitos.
/// Use as teclas 1, 2, 3 para testar diferentes configurações.
/// </summary>
public class TestItemHighlight : MonoBehaviour
{
    private ItemHighlight highlight;
    private ItemHighlightEmissive highlightEmissive;
    private bool usingEmissive = false;

    void Start()
    {
        // Tenta obter qual versão está sendo usada
        highlight = GetComponent<ItemHighlight>();
        highlightEmissive = GetComponent<ItemHighlightEmissive>();

        if (highlightEmissive != null)
        {
            usingEmissive = true;
            Debug.Log("?? Testador ativo! Usando ItemHighlightEmissive");
        }
        else if (highlight != null)
        {
            usingEmissive = false;
            Debug.Log("?? Testador ativo! Usando ItemHighlight simples");
        }
        else
        {
            Debug.LogError("? Nenhum ItemHighlight encontrado! Adicione o componente primeiro.");
            enabled = false;
            return;
        }

        Debug.Log("" +
            "Controles:\n" +
            "  1 - Amarelo (padrão)\n" +
            "  2 - Azul\n" +
            "  3 - Verde\n" +
            "  4 - Roxo\n" +
            "  5 - Vermelho\n" +
            "  E - Desativar efeito\n" +
            "  R - Reativar efeito");
    }

    void Update()
    {
        // Teclas para mudar a cor
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeColor(new Color(1f, 1f, 0f), "Amarelo");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeColor(new Color(0f, 0.5f, 1f), "Azul");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeColor(new Color(0f, 1f, 0f), "Verde");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeColor(new Color(0.8f, 0f, 1f), "Roxo");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ChangeColor(new Color(1f, 0f, 0f), "Vermelho");
        }

        // Ativar/Desativar
        if (Input.GetKeyDown(KeyCode.E))
        {
            DisableEffect();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            EnableEffect();
        }
    }

    void ChangeColor(Color newColor, string colorName)
    {
        if (usingEmissive && highlightEmissive != null)
        {
            highlightEmissive.SetGlowColor(newColor);
            Debug.Log($"? Cor alterada para: {colorName}");
        }
        else
        {
            Debug.Log("?? Mudança de cor dinâmica só funciona com ItemHighlightEmissive");
        }
    }

    void DisableEffect()
    {
        if (usingEmissive && highlightEmissive != null)
        {
            highlightEmissive.DisableHighlight();
        }
        else if (highlight != null)
        {
            highlight.DisableHighlight();
        }
        Debug.Log("? Efeito desativado");
    }

    void EnableEffect()
    {
        if (usingEmissive && highlightEmissive != null)
        {
            highlightEmissive.EnableHighlight();
        }
        else if (highlight != null)
        {
            highlight.EnableHighlight();
        }
        Debug.Log("? Efeito reativado");
    }

    void OnGUI()
    {
        // UI simples para mostrar os controles
        GUI.Box(new Rect(10, 10, 250, 180), "?? Testador de Item Highlight");
        
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 12;
        style.normal.textColor = Color.white;

        string version = usingEmissive ? "Emissive (Avançado)" : "Simples";
        GUI.Label(new Rect(20, 35, 230, 20), $"Versão: {version}", style);
        
        GUI.Label(new Rect(20, 60, 230, 20), "1 - Amarelo", style);
        GUI.Label(new Rect(20, 80, 230, 20), "2 - Azul", style);
        GUI.Label(new Rect(20, 100, 230, 20), "3 - Verde", style);
        GUI.Label(new Rect(20, 120, 230, 20), "4 - Roxo", style);
        GUI.Label(new Rect(20, 140, 230, 20), "E - Desativar", style);
        GUI.Label(new Rect(20, 160, 230, 20), "R - Reativar", style);
    }
}
