using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Ferramenta de Editor para aplicar ItemHighlight automaticamente
/// a todos os prefabs de itens na pasta especificada.
/// 
/// Uso: No menu do Unity, vá em Tools > Item Highlight > Aplicar a Todos os Itens
/// </summary>
public class ItemHighlightEditorTool : EditorWindow
{
    private string itemsPrefabPath = "Assets/Prefabs/Items";
    private bool useEmissiveVersion = false;
    private bool overwriteExisting = false;
    
    // Configurações padrão
    private Color glowColor = new Color(1f, 1f, 0f, 1f); // Amarelo
    private float glowIntensity = 0.5f;
    private float glowPulseSpeed = 2f;
    private float minScale = 0.9f;
    private float maxScale = 1.1f;
    private float scalePulseSpeed = 1.5f;

    [MenuItem("Tools/Item Highlight/Configurar Destaque de Itens")]
    public static void ShowWindow()
    {
        GetWindow<ItemHighlightEditorTool>("Item Highlight Tool");
    }

    void OnGUI()
    {
        GUILayout.Label("?? Ferramenta de Destaque de Itens", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Configurações de pasta
        GUILayout.Label("?? Configuração de Pasta", EditorStyles.boldLabel);
        itemsPrefabPath = EditorGUILayout.TextField("Pasta dos Prefabs:", itemsPrefabPath);
        
        if (GUILayout.Button("?? Escolher Pasta"))
        {
            string path = EditorUtility.OpenFolderPanel("Selecione a pasta dos prefabs", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                itemsPrefabPath = "Assets" + path.Substring(Application.dataPath.Length);
            }
        }

        EditorGUILayout.Space();

        // Tipo de script
        GUILayout.Label("?? Tipo de Efeito", EditorStyles.boldLabel);
        useEmissiveVersion = EditorGUILayout.Toggle("Usar Versão Emissiva", useEmissiveVersion);
        overwriteExisting = EditorGUILayout.Toggle("Substituir Existentes", overwriteExisting);

        EditorGUILayout.Space();

        // Configurações visuais
        GUILayout.Label("? Configurações Visuais", EditorStyles.boldLabel);
        glowColor = EditorGUILayout.ColorField("Cor do Brilho:", glowColor);
        glowIntensity = EditorGUILayout.Slider("Intensidade:", glowIntensity, 0f, 1f);
        glowPulseSpeed = EditorGUILayout.Slider("Velocidade do Brilho:", glowPulseSpeed, 0.5f, 5f);
        
        EditorGUILayout.Space();
        
        minScale = EditorGUILayout.Slider("Escala Mínima:", minScale, 0.5f, 1f);
        maxScale = EditorGUILayout.Slider("Escala Máxima:", maxScale, 1f, 1.5f);
        scalePulseSpeed = EditorGUILayout.Slider("Velocidade da Escala:", scalePulseSpeed, 0.5f, 5f);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Botões de ação
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("? Aplicar a Todos os Prefabs", GUILayout.Height(40)))
        {
            ApplyToAllPrefabs();
        }

        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("?? Listar Prefabs de Itens", GUILayout.Height(30)))
        {
            ListItemPrefabs();
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("? Remover de Todos os Prefabs", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Confirmar Remoção",
                "Tem certeza que deseja remover o ItemHighlight de todos os prefabs?",
                "Sim, Remover", "Cancelar"))
            {
                RemoveFromAllPrefabs();
            }
        }

        GUI.backgroundColor = Color.white;
    }

    void ApplyToAllPrefabs()
    {
        if (!AssetDatabase.IsValidFolder(itemsPrefabPath))
        {
            EditorUtility.DisplayDialog("Erro", $"A pasta '{itemsPrefabPath}' não existe!", "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { itemsPrefabPath });
        int addedCount = 0;
        int skippedCount = 0;
        List<string> processedItems = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) continue;

            // Verifica se é um item (tem ItemPickup ou ItemData)
            bool isItem = prefab.GetComponent<ItemPickup>() != null;

            if (isItem)
            {
                bool hasHighlight = prefab.GetComponent<ItemHighlight>() != null || 
                                  prefab.GetComponent<ItemHighlightEmissive>() != null;

                if (hasHighlight && !overwriteExisting)
                {
                    skippedCount++;
                    continue;
                }

                // Carrega o conteúdo do prefab para edição
                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);

                // Remove componentes existentes se overwrite estiver ativo
                if (overwriteExisting)
                {
                    ItemHighlight oldSimple = instance.GetComponent<ItemHighlight>();
                    if (oldSimple) DestroyImmediate(oldSimple, true);

                    ItemHighlightEmissive oldEmissive = instance.GetComponent<ItemHighlightEmissive>();
                    if (oldEmissive) DestroyImmediate(oldEmissive, true);
                }

                // Adiciona o novo componente
                if (useEmissiveVersion)
                {
                    ItemHighlightEmissive highlight = instance.AddComponent<ItemHighlightEmissive>();
                    ConfigureEmissiveComponent(highlight);
                }
                else
                {
                    ItemHighlight highlight = instance.AddComponent<ItemHighlight>();
                    ConfigureSimpleComponent(highlight);
                }

                // Salva o prefab
                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                PrefabUtility.UnloadPrefabContents(instance);

                addedCount++;
                processedItems.Add(prefab.name);
            }
        }

        AssetDatabase.SaveAssets();

        string message = $"? Processo concluído!\n\n" +
                        $"• Prefabs processados: {addedCount}\n" +
                        $"• Prefabs ignorados: {skippedCount}\n\n" +
                        $"Itens processados:\n" + string.Join("\n", processedItems);

        Debug.Log(message);
        EditorUtility.DisplayDialog("Concluído", message, "OK");
    }

    void ConfigureSimpleComponent(ItemHighlight highlight)
    {
        SerializedObject so = new SerializedObject(highlight);
        
        so.FindProperty("glowColor").colorValue = glowColor;
        so.FindProperty("glowIntensity").floatValue = glowIntensity;
        so.FindProperty("glowPulseSpeed").floatValue = glowPulseSpeed;
        so.FindProperty("minScale").floatValue = minScale;
        so.FindProperty("maxScale").floatValue = maxScale;
        so.FindProperty("scalePulseSpeed").floatValue = scalePulseSpeed;
        
        so.ApplyModifiedProperties();
    }

    void ConfigureEmissiveComponent(ItemHighlightEmissive highlight)
    {
        SerializedObject so = new SerializedObject(highlight);
        
        so.FindProperty("emissionColor").colorValue = glowColor;
        so.FindProperty("minEmissionIntensity").floatValue = glowIntensity * 0.5f;
        so.FindProperty("maxEmissionIntensity").floatValue = glowIntensity * 2f;
        so.FindProperty("emissionPulseSpeed").floatValue = glowPulseSpeed;
        so.FindProperty("minScale").floatValue = minScale;
        so.FindProperty("maxScale").floatValue = maxScale;
        so.FindProperty("scalePulseSpeed").floatValue = scalePulseSpeed;
        
        so.ApplyModifiedProperties();
    }

    void ListItemPrefabs()
    {
        if (!AssetDatabase.IsValidFolder(itemsPrefabPath))
        {
            EditorUtility.DisplayDialog("Erro", $"A pasta '{itemsPrefabPath}' não existe!", "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { itemsPrefabPath });
        List<string> items = new List<string>();
        List<string> itemsWithHighlight = new List<string>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null && prefab.GetComponent<ItemPickup>() != null)
            {
                items.Add(prefab.name);
                
                bool hasHighlight = prefab.GetComponent<ItemHighlight>() != null || 
                                  prefab.GetComponent<ItemHighlightEmissive>() != null;
                
                if (hasHighlight)
                {
                    itemsWithHighlight.Add(prefab.name);
                }
            }
        }

        string message = $"?? Prefabs de Itens Encontrados: {items.Count}\n\n" +
                        $"? Com ItemHighlight: {itemsWithHighlight.Count}\n\n" +
                        $"Lista de itens:\n" + string.Join("\n", items);

        Debug.Log(message);
        EditorUtility.DisplayDialog("Lista de Prefabs", message, "OK");
    }

    void RemoveFromAllPrefabs()
    {
        if (!AssetDatabase.IsValidFolder(itemsPrefabPath))
        {
            EditorUtility.DisplayDialog("Erro", $"A pasta '{itemsPrefabPath}' não existe!", "OK");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { itemsPrefabPath });
        int removedCount = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) continue;

            bool hasHighlight = prefab.GetComponent<ItemHighlight>() != null || 
                              prefab.GetComponent<ItemHighlightEmissive>() != null;

            if (hasHighlight)
            {
                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                GameObject instance = PrefabUtility.LoadPrefabContents(prefabPath);

                ItemHighlight simple = instance.GetComponent<ItemHighlight>();
                if (simple) DestroyImmediate(simple, true);

                ItemHighlightEmissive emissive = instance.GetComponent<ItemHighlightEmissive>();
                if (emissive) DestroyImmediate(emissive, true);

                PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
                PrefabUtility.UnloadPrefabContents(instance);

                removedCount++;
            }
        }

        AssetDatabase.SaveAssets();

        string message = $"? ItemHighlight removido de {removedCount} prefabs.";
        Debug.Log(message);
        EditorUtility.DisplayDialog("Concluído", message, "OK");
    }
}
