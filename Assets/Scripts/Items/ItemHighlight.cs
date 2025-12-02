using UnityEngine;

/// <summary>
/// Adiciona um efeito visual de destaque aos itens:
/// - Brilho amarelo pulsante
/// - Escala que cresce e diminui (pulsação)
/// 
/// Para usar: Basta adicionar este script ao prefab do item.
/// </summary>
public class ItemHighlight : MonoBehaviour
{
    [Header("Configurações de Brilho")]
    [Tooltip("Cor do brilho. Amarelo por padrão.")]
    [SerializeField] private Color glowColor = new Color(1f, 1f, 0f, 1f); // Amarelo
    
    [Tooltip("Intensidade do brilho (0 a 1).")]
    [Range(0f, 1f)]
    [SerializeField] private float glowIntensity = 0.5f;
    
    [Tooltip("Velocidade da pulsação do brilho.")]
    [SerializeField] private float glowPulseSpeed = 2f;

    [Header("Configurações de Pulsação de Escala")]
    [Tooltip("Escala mínima do item durante a pulsação (ex: 0.9 = 90% do tamanho original).")]
    [Range(0.5f, 1f)]
    [SerializeField] private float minScale = 0.9f;
    
    [Tooltip("Escala máxima do item durante a pulsação (ex: 1.1 = 110% do tamanho original).")]
    [Range(1f, 1.5f)]
    [SerializeField] private float maxScale = 1.1f;
    
    [Tooltip("Velocidade da pulsação de escala.")]
    [SerializeField] private float scalePulseSpeed = 1.5f;

    // Componentes
    private SpriteRenderer spriteRenderer;
    private Material glowMaterial;
    private Vector3 originalScale;
    
    // Controle de tempo
    private float glowTime = 0f;
    private float scaleTime = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"ItemHighlight: Nenhum SpriteRenderer encontrado em {gameObject.name}. O efeito não funcionará!");
            enabled = false;
            return;
        }

        // Salva a escala original
        originalScale = transform.localScale;
        
        // Cria um material único para este item (para não afetar outros itens)
        CreateGlowMaterial();
    }

    void CreateGlowMaterial()
    {
        // Cria uma cópia do material para este sprite específico
        glowMaterial = new Material(spriteRenderer.material);
        spriteRenderer.material = glowMaterial;
        
        // Configura o shader para suportar cor adicional (glow)
        // Se estiver usando o shader padrão do Unity, podemos usar a propriedade _Color
        glowMaterial.SetColor("_Color", Color.white);
    }

    void Update()
    {
        // Atualiza o brilho
        UpdateGlow();
        
        // Atualiza a escala (pulsação)
        UpdateScale();
    }

    void UpdateGlow()
    {
        glowTime += Time.deltaTime * glowPulseSpeed;
        
        // Calcula a intensidade do brilho usando uma onda senoidal (oscila suavemente)
        float pulse = Mathf.Sin(glowTime) * 0.5f + 0.5f; // Varia de 0 a 1
        float currentIntensity = Mathf.Lerp(0.3f, glowIntensity, pulse);
        
        // Interpola entre a cor branca e a cor do brilho
        Color finalColor = Color.Lerp(Color.white, glowColor, currentIntensity);
        
        // Aplica a cor ao material
        if (glowMaterial != null)
        {
            glowMaterial.SetColor("_Color", finalColor);
        }
    }

    void UpdateScale()
    {
        scaleTime += Time.deltaTime * scalePulseSpeed;
        
        // Calcula a escala usando uma onda senoidal
        float pulse = Mathf.Sin(scaleTime) * 0.5f + 0.5f; // Varia de 0 a 1
        float currentScale = Mathf.Lerp(minScale, maxScale, pulse);
        
        // Aplica a escala ao transform
        transform.localScale = originalScale * currentScale;
    }

    void OnDestroy()
    {
        // Limpa o material criado para evitar memory leaks
        if (glowMaterial != null)
        {
            Destroy(glowMaterial);
        }
    }

    // Método opcional para resetar o efeito
    public void ResetEffect()
    {
        glowTime = 0f;
        scaleTime = 0f;
        transform.localScale = originalScale;
        
        if (glowMaterial != null)
        {
            glowMaterial.SetColor("_Color", Color.white);
        }
    }

    // Métodos para ativar/desativar o efeito dinamicamente
    public void EnableHighlight()
    {
        enabled = true;
    }

    public void DisableHighlight()
    {
        enabled = false;
        transform.localScale = originalScale;
        
        if (glowMaterial != null)
        {
            glowMaterial.SetColor("_Color", Color.white);
        }
    }

    // Para debug: Desenha um círculo ao redor do item no editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = glowColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
