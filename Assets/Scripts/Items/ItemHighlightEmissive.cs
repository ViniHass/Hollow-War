using UnityEngine;

/// <summary>
/// Versão avançada do ItemHighlight que usa propriedades de emissão
/// para criar um brilho mais intenso e visível.
/// 
/// IMPORTANTE: Requer um material com shader que suporte emissão
/// (como Sprites/Default ou um shader customizado).
/// 
/// Para usar: 
/// 1. Adicione este script ao prefab do item
/// 2. Certifique-se de que o SpriteRenderer tem um material apropriado
/// </summary>
public class ItemHighlightEmissive : MonoBehaviour
{
    [Header("Configurações de Brilho Emissivo")]
    [Tooltip("Cor do brilho emissivo.")]
    [SerializeField] private Color emissionColor = new Color(1f, 0.9f, 0f); // Amarelo dourado
    
    [Tooltip("Intensidade mínima da emissão.")]
    [SerializeField] private float minEmissionIntensity = 0.5f;
    
    [Tooltip("Intensidade máxima da emissão.")]
    [SerializeField] private float maxEmissionIntensity = 2f;
    
    [Tooltip("Velocidade da pulsação do brilho.")]
    [SerializeField] private float emissionPulseSpeed = 2f;

    [Header("Configurações de Pulsação de Escala")]
    [Tooltip("Escala mínima durante a pulsação.")]
    [Range(0.8f, 1f)]
    [SerializeField] private float minScale = 0.95f;
    
    [Tooltip("Escala máxima durante a pulsação.")]
    [Range(1f, 1.3f)]
    [SerializeField] private float maxScale = 1.15f;
    
    [Tooltip("Velocidade da pulsação de escala.")]
    [SerializeField] private float scalePulseSpeed = 1.5f;

    [Header("Configurações de Rotação (Opcional)")]
    [Tooltip("Se marcado, o item terá uma leve rotação contínua.")]
    [SerializeField] private bool enableRotation = false;
    
    [Tooltip("Velocidade da rotação em graus por segundo.")]
    [SerializeField] private float rotationSpeed = 30f;

    // Componentes
    private SpriteRenderer spriteRenderer;
    private Material itemMaterial;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    
    // Controle de tempo
    private float emissionTime = 0f;
    private float scaleTime = 0f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"ItemHighlightEmissive: SpriteRenderer não encontrado em {gameObject.name}!");
            enabled = false;
            return;
        }

        originalScale = transform.localScale;
        originalRotation = transform.rotation;
        
        SetupMaterial();
    }

    void SetupMaterial()
    {
        // Cria uma instância única do material
        itemMaterial = new Material(spriteRenderer.material);
        spriteRenderer.material = itemMaterial;
        
        // Se o material suportar emissão, ativa ela
        if (itemMaterial.HasProperty("_EmissionColor"))
        {
            itemMaterial.EnableKeyword("_EMISSION");
        }
    }

    void Update()
    {
        UpdateEmission();
        UpdateScale();
        
        if (enableRotation)
        {
            UpdateRotation();
        }
    }

    void UpdateEmission()
    {
        emissionTime += Time.deltaTime * emissionPulseSpeed;
        
        // Cria uma pulsação suave
        float pulse = Mathf.Sin(emissionTime) * 0.5f + 0.5f;
        float currentIntensity = Mathf.Lerp(minEmissionIntensity, maxEmissionIntensity, pulse);
        
        // Aplica a cor emissiva
        Color finalEmission = emissionColor * currentIntensity;
        
        if (itemMaterial != null)
        {
            // Tenta aplicar emissão se o shader suportar
            if (itemMaterial.HasProperty("_EmissionColor"))
            {
                itemMaterial.SetColor("_EmissionColor", finalEmission);
            }
            
            // Também aplica cor base para um efeito visual adicional
            Color tintColor = Color.Lerp(Color.white, emissionColor, pulse * 0.3f);
            itemMaterial.SetColor("_Color", tintColor);
        }
    }

    void UpdateScale()
    {
        scaleTime += Time.deltaTime * scalePulseSpeed;
        
        float pulse = Mathf.Sin(scaleTime) * 0.5f + 0.5f;
        float currentScale = Mathf.Lerp(minScale, maxScale, pulse);
        
        transform.localScale = originalScale * currentScale;
    }

    void UpdateRotation()
    {
        // Rotação suave no eixo Z
        float rotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotation);
    }

    void OnDestroy()
    {
        if (itemMaterial != null)
        {
            Destroy(itemMaterial);
        }
    }

    // Métodos públicos para controle externo
    public void EnableHighlight()
    {
        enabled = true;
    }

    public void DisableHighlight()
    {
        enabled = false;
        transform.localScale = originalScale;
        transform.rotation = originalRotation;
        
        if (itemMaterial != null)
        {
            itemMaterial.SetColor("_Color", Color.white);
            if (itemMaterial.HasProperty("_EmissionColor"))
            {
                itemMaterial.SetColor("_EmissionColor", Color.black);
            }
        }
    }

    public void SetGlowColor(Color newColor)
    {
        emissionColor = newColor;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = emissionColor;
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}
