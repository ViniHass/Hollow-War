using UnityEngine;

/// <summary>
/// Classe com presets pré-configurados para diferentes tipos de itens.
/// Use os métodos estáticos para aplicar configurações específicas.
/// 
/// Exemplo de uso:
///   ItemHighlightPresets.ApplyCommonItemPreset(itemGameObject);
///   ItemHighlightPresets.ApplyRareItemPreset(itemGameObject);
/// </summary>
public static class ItemHighlightPresets
{
    // ============================================
    // PRESETS PARA ItemHighlight (Versão Simples)
    // ============================================

    /// <summary>
    /// Preset para itens comuns (chaves, poções básicas, moedas).
    /// Brilho amarelo suave com pulsação leve.
    /// </summary>
    public static void ApplyCommonItemPreset(GameObject item)
    {
        ItemHighlight highlight = item.GetComponent<ItemHighlight>();
        if (highlight == null)
            highlight = item.AddComponent<ItemHighlight>();

        // Configuração via Reflection para acessar campos privados
        SetPrivateField(highlight, "glowColor", new Color(1f, 1f, 0f, 1f)); // Amarelo
        SetPrivateField(highlight, "glowIntensity", 0.5f);
        SetPrivateField(highlight, "glowPulseSpeed", 2f);
        SetPrivateField(highlight, "minScale", 0.92f);
        SetPrivateField(highlight, "maxScale", 1.08f);
        SetPrivateField(highlight, "scalePulseSpeed", 1.5f);

        Debug.Log($"? Preset 'Item Comum' aplicado a {item.name}");
    }

    /// <summary>
    /// Preset para itens importantes da história (itens de quest).
    /// Brilho dourado com pulsação média.
    /// </summary>
    public static void ApplyQuestItemPreset(GameObject item)
    {
        ItemHighlight highlight = item.GetComponent<ItemHighlight>();
        if (highlight == null)
            highlight = item.AddComponent<ItemHighlight>();

        SetPrivateField(highlight, "glowColor", new Color(1f, 0.9f, 0.2f, 1f)); // Dourado
        SetPrivateField(highlight, "glowIntensity", 0.65f);
        SetPrivateField(highlight, "glowPulseSpeed", 2.5f);
        SetPrivateField(highlight, "minScale", 0.9f);
        SetPrivateField(highlight, "maxScale", 1.12f);
        SetPrivateField(highlight, "scalePulseSpeed", 1.8f);

        Debug.Log($"? Preset 'Item de Quest' aplicado a {item.name}");
    }

    /// <summary>
    /// Preset para itens de cura/vida.
    /// Brilho verde com pulsação suave.
    /// </summary>
    public static void ApplyHealthItemPreset(GameObject item)
    {
        ItemHighlight highlight = item.GetComponent<ItemHighlight>();
        if (highlight == null)
            highlight = item.AddComponent<ItemHighlight>();

        SetPrivateField(highlight, "glowColor", new Color(0f, 1f, 0.3f, 1f)); // Verde
        SetPrivateField(highlight, "glowIntensity", 0.55f);
        SetPrivateField(highlight, "glowPulseSpeed", 1.8f);
        SetPrivateField(highlight, "minScale", 0.93f);
        SetPrivateField(highlight, "maxScale", 1.07f);
        SetPrivateField(highlight, "scalePulseSpeed", 1.4f);

        Debug.Log($"? Preset 'Item de Cura' aplicado a {item.name}");
    }

    // ================================================
    // PRESETS PARA ItemHighlightEmissive (Avançado)
    // ================================================

    /// <summary>
    /// Preset para itens raros/épicos.
    /// Brilho roxo intenso com pulsação forte e rotação.
    /// </summary>
    public static void ApplyRareItemPreset(GameObject item)
    {
        ItemHighlightEmissive highlight = item.GetComponent<ItemHighlightEmissive>();
        if (highlight == null)
            highlight = item.AddComponent<ItemHighlightEmissive>();

        SetPrivateField(highlight, "emissionColor", new Color(0.8f, 0f, 1f, 1f)); // Roxo
        SetPrivateField(highlight, "minEmissionIntensity", 0.7f);
        SetPrivateField(highlight, "maxEmissionIntensity", 2.5f);
        SetPrivateField(highlight, "emissionPulseSpeed", 2.2f);
        SetPrivateField(highlight, "minScale", 0.88f);
        SetPrivateField(highlight, "maxScale", 1.18f);
        SetPrivateField(highlight, "scalePulseSpeed", 1.8f);
        SetPrivateField(highlight, "enableRotation", true);
        SetPrivateField(highlight, "rotationSpeed", 30f);

        Debug.Log($"? Preset 'Item Raro' aplicado a {item.name}");
    }

    /// <summary>
    /// Preset para itens lendários/únicos.
    /// Brilho dourado brilhante com pulsação intensa e rotação rápida.
    /// </summary>
    public static void ApplyLegendaryItemPreset(GameObject item)
    {
        ItemHighlightEmissive highlight = item.GetComponent<ItemHighlightEmissive>();
        if (highlight == null)
            highlight = item.AddComponent<ItemHighlightEmissive>();

        SetPrivateField(highlight, "emissionColor", new Color(1f, 0.85f, 0f, 1f)); // Dourado brilhante
        SetPrivateField(highlight, "minEmissionIntensity", 1f);
        SetPrivateField(highlight, "maxEmissionIntensity", 3f);
        SetPrivateField(highlight, "emissionPulseSpeed", 2.5f);
        SetPrivateField(highlight, "minScale", 0.85f);
        SetPrivateField(highlight, "maxScale", 1.25f);
        SetPrivateField(highlight, "scalePulseSpeed", 2f);
        SetPrivateField(highlight, "enableRotation", true);
        SetPrivateField(highlight, "rotationSpeed", 45f);

        Debug.Log($"? Preset 'Item Lendário' aplicado a {item.name}");
    }

    /// <summary>
    /// Preset para itens mágicos/arcanos.
    /// Brilho azul místico com pulsação média.
    /// </summary>
    public static void ApplyMagicItemPreset(GameObject item)
    {
        ItemHighlightEmissive highlight = item.GetComponent<ItemHighlightEmissive>();
        if (highlight == null)
            highlight = item.AddComponent<ItemHighlightEmissive>();

        SetPrivateField(highlight, "emissionColor", new Color(0.2f, 0.7f, 1f, 1f)); // Azul mágico
        SetPrivateField(highlight, "minEmissionIntensity", 0.6f);
        SetPrivateField(highlight, "maxEmissionIntensity", 2.2f);
        SetPrivateField(highlight, "emissionPulseSpeed", 2f);
        SetPrivateField(highlight, "minScale", 0.9f);
        SetPrivateField(highlight, "maxScale", 1.15f);
        SetPrivateField(highlight, "scalePulseSpeed", 1.6f);
        SetPrivateField(highlight, "enableRotation", true);
        SetPrivateField(highlight, "rotationSpeed", 20f);

        Debug.Log($"? Preset 'Item Mágico' aplicado a {item.name}");
    }

    /// <summary>
    /// Preset para power-ups/consumíveis especiais.
    /// Brilho laranja energético com pulsação rápida.
    /// </summary>
    public static void ApplyPowerUpPreset(GameObject item)
    {
        ItemHighlightEmissive highlight = item.GetComponent<ItemHighlightEmissive>();
        if (highlight == null)
            highlight = item.AddComponent<ItemHighlightEmissive>();

        SetPrivateField(highlight, "emissionColor", new Color(1f, 0.5f, 0f, 1f)); // Laranja
        SetPrivateField(highlight, "minEmissionIntensity", 0.8f);
        SetPrivateField(highlight, "maxEmissionIntensity", 2.8f);
        SetPrivateField(highlight, "emissionPulseSpeed", 3f);
        SetPrivateField(highlight, "minScale", 0.87f);
        SetPrivateField(highlight, "maxScale", 1.2f);
        SetPrivateField(highlight, "scalePulseSpeed", 2.2f);
        SetPrivateField(highlight, "enableRotation", false);

        Debug.Log($"? Preset 'Power-Up' aplicado a {item.name}");
    }

    // ====================================
    // MÉTODOS AUXILIARES
    // ====================================

    /// <summary>
    /// Método auxiliar para configurar campos privados usando Reflection.
    /// </summary>
    private static void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | 
            System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            Debug.LogWarning($"?? Campo '{fieldName}' não encontrado em {obj.GetType().Name}");
        }
    }

    /// <summary>
    /// Remove qualquer ItemHighlight do GameObject.
    /// </summary>
    public static void RemoveHighlight(GameObject item)
    {
        ItemHighlight simple = item.GetComponent<ItemHighlight>();
        if (simple != null)
        {
            Object.Destroy(simple);
            Debug.Log($"? ItemHighlight removido de {item.name}");
        }

        ItemHighlightEmissive emissive = item.GetComponent<ItemHighlightEmissive>();
        if (emissive != null)
        {
            Object.Destroy(emissive);
            Debug.Log($"? ItemHighlightEmissive removido de {item.name}");
        }
    }
}

// ============================================
// EXEMPLOS DE USO
// ============================================

/*
/// <summary>
/// Exemplo de como usar os presets em seu código.
/// </summary>
public class ItemSpawner : MonoBehaviour
{
    public GameObject keyPrefab;
    public GameObject potionPrefab;
    public GameObject swordPrefab;

    void Start()
    {
        // Spawna e aplica presets
        GameObject key = Instantiate(keyPrefab);
        ItemHighlightPresets.ApplyCommonItemPreset(key);

        GameObject potion = Instantiate(potionPrefab);
        ItemHighlightPresets.ApplyHealthItemPreset(potion);

        GameObject sword = Instantiate(swordPrefab);
        ItemHighlightPresets.ApplyRareItemPreset(sword);
    }
}
*/
