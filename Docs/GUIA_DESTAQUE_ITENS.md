# ?? Sistema de Destaque para Itens - Guia de Uso

## ?? Visão Geral

Este sistema adiciona efeitos visuais de destaque aos itens no jogo:
- ? **Brilho amarelo pulsante**
- ?? **Pulsação de escala** (o item cresce e diminui)
- ?? **Rotação opcional** (versão avançada)

---

## ?? Scripts Disponíveis

### 1. `ItemHighlight.cs` (Versão Simples)
**Recomendado para:** Maioria dos casos

**Características:**
- Brilho amarelo suave
- Pulsação de escala
- Fácil de usar
- Funciona com qualquer shader de sprite

**Uso:**
1. Selecione o prefab do item no Project
2. Adicione o componente `ItemHighlight`
3. Ajuste as configurações no Inspector (opcional)
4. Pronto!

---

### 2. `ItemHighlightEmissive.cs` (Versão Avançada)
**Recomendado para:** Itens especiais ou quando você quer um brilho mais intenso

**Características:**
- Brilho emissivo mais intenso
- Pulsação de escala
- Rotação opcional
- Requer material com suporte a emissão

**Uso:**
1. Selecione o prefab do item
2. Adicione o componente `ItemHighlightEmissive`
3. (Opcional) Ajuste o material do SpriteRenderer para usar um shader com emissão
4. Configure no Inspector

---

## ??? Como Aplicar aos Prefabs

### Método 1: Aplicar Manualmente a Cada Prefab

1. **Abra a pasta de Prefabs:**
   - Navegue até onde seus prefabs de itens estão salvos
   - Exemplo: `Assets/Prefabs/Items/`

2. **Selecione o prefab do item:**
   - Clique no prefab (ex: `Key.prefab`, `Potion.prefab`)

3. **Adicione o script:**
   - No Inspector, clique em "Add Component"
   - Digite "ItemHighlight" ou "ItemHighlightEmissive"
   - Selecione o script

4. **Configure (opcional):**
   ```
   Glow Color: Amarelo (padrão) ou sua cor preferida
   Glow Intensity: 0.5 (recomendado)
   Glow Pulse Speed: 2 (recomendado)
   Min Scale: 0.9
   Max Scale: 1.1
   Scale Pulse Speed: 1.5
   ```

5. **Salve o prefab:**
   - Ctrl + S ou File > Save

---

### Método 2: Aplicar a Múltiplos Prefabs de Uma Vez

1. **Selecione múltiplos prefabs:**
   - Segure `Ctrl` (Windows) ou `Cmd` (Mac)
   - Clique em cada prefab que deseja modificar

2. **Adicione o componente:**
   - Com todos selecionados, clique em "Add Component" no Inspector
   - Adicione `ItemHighlight`

3. **Configure em lote:**
   - As alterações serão aplicadas a todos os prefabs selecionados

---

### Método 3: Aplicar via Script (Automatizado)

Se você tem muitos prefabs, pode criar um script de Editor:

```csharp
// Salve este script em Assets/Editor/ApplyItemHighlightToAll.cs
using UnityEngine;
using UnityEditor;

public class ApplyItemHighlightToAll : EditorWindow
{
    [MenuItem("Tools/Aplicar ItemHighlight a Todos os Itens")]
    static void ApplyToAllItems()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/Items" });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null && prefab.GetComponent<ItemPickup>() != null)
            {
                // Adiciona o ItemHighlight se ainda não tiver
                if (prefab.GetComponent<ItemHighlight>() == null)
                {
                    GameObject instance = PrefabUtility.LoadPrefabContents(path);
                    instance.AddComponent<ItemHighlight>();
                    PrefabUtility.SaveAsPrefabAsset(instance, path);
                    PrefabUtility.UnloadPrefabContents(instance);
                    
                    Debug.Log($"? ItemHighlight adicionado a: {prefab.name}");
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log("?? Processo concluído!");
    }
}
```

---

## ?? Configurações Detalhadas

### ItemHighlight (Versão Simples)

| Parâmetro | Descrição | Valor Recomendado |
|-----------|-----------|-------------------|
| **Glow Color** | Cor do brilho | Amarelo (1, 1, 0) |
| **Glow Intensity** | Intensidade do brilho (0-1) | 0.5 |
| **Glow Pulse Speed** | Velocidade da pulsação do brilho | 2.0 |
| **Min Scale** | Escala mínima (0.5-1) | 0.9 |
| **Max Scale** | Escala máxima (1-1.5) | 1.1 |
| **Scale Pulse Speed** | Velocidade da pulsação da escala | 1.5 |

---

### ItemHighlightEmissive (Versão Avançada)

| Parâmetro | Descrição | Valor Recomendado |
|-----------|-----------|-------------------|
| **Emission Color** | Cor do brilho emissivo | Amarelo dourado (1, 0.9, 0) |
| **Min Emission Intensity** | Intensidade mínima | 0.5 |
| **Max Emission Intensity** | Intensidade máxima | 2.0 |
| **Emission Pulse Speed** | Velocidade da pulsação | 2.0 |
| **Min Scale** | Escala mínima (0.8-1) | 0.95 |
| **Max Scale** | Escala máxima (1-1.3) | 1.15 |
| **Scale Pulse Speed** | Velocidade da escala | 1.5 |
| **Enable Rotation** | Ativa rotação contínua | false (desligado) |
| **Rotation Speed** | Velocidade de rotação (graus/s) | 30 |

---

## ?? Personalizações Comuns

### Mudar a Cor do Brilho

**Para brilho azul:**
```
Glow Color: (0, 0.5, 1)
```

**Para brilho verde:**
```
Glow Color: (0, 1, 0)
```

**Para brilho roxo:**
```
Glow Color: (0.8, 0, 1)
```

---

### Ajustar a Intensidade da Pulsação

**Pulsação suave:**
```
Min Scale: 0.95
Max Scale: 1.05
Scale Pulse Speed: 1.0
```

**Pulsação intensa:**
```
Min Scale: 0.85
Max Scale: 1.2
Scale Pulse Speed: 2.5
```

---

### Desativar a Pulsação (Só Brilho)

```
Min Scale: 1.0
Max Scale: 1.0
```

---

## ?? Controle Programático

### Ativar/Desativar o Efeito via Script

```csharp
// Obter referência ao componente
ItemHighlight highlight = GetComponent<ItemHighlight>();

// Desativar o efeito
highlight.DisableHighlight();

// Ativar o efeito
highlight.EnableHighlight();

// Resetar o efeito
highlight.ResetEffect();
```

---

### Mudar a Cor Dinamicamente (Versão Emissive)

```csharp
ItemHighlightEmissive highlight = GetComponent<ItemHighlightEmissive>();

// Mudar para vermelho
highlight.SetGlowColor(Color.red);

// Mudar para azul
highlight.SetGlowColor(new Color(0, 0.5f, 1f));
```

---

## ?? Troubleshooting

### ? O brilho não aparece

**Solução 1:** Verifique se o GameObject tem um `SpriteRenderer`

**Solução 2:** Certifique-se de que o sprite tem uma cor visível (não transparente)

---

### ? A pulsação não está suave

**Solução:** Reduza o `Scale Pulse Speed` para valores entre 1.0 e 2.0

---

### ? O efeito está muito intenso

**Solução:** Diminua o `Glow Intensity` ou `Max Emission Intensity`

---

### ? O item fica pequeno demais

**Solução:** Aumente o `Min Scale` para valores mais próximos de 1.0

---

## ?? Checklist Rápido

- [ ] Script adicionado ao prefab do item
- [ ] `SpriteRenderer` presente no GameObject
- [ ] Configurações ajustadas no Inspector
- [ ] Testado na cena
- [ ] Prefab salvo

---

## ?? Exemplos de Uso

### Item Comum (Chave, Poção)
```
ItemHighlight (versão simples)
Glow Color: Amarelo
Glow Intensity: 0.5
Scale: 0.9 - 1.1
```

---

### Item Raro (Artefato, Recompensa de Quest)
```
ItemHighlightEmissive (versão avançada)
Emission Color: Dourado
Max Emission: 2.5
Scale: 0.9 - 1.2
Enable Rotation: true
```

---

### Item Especial (Power-up, Item Mágico)
```
ItemHighlightEmissive
Emission Color: Azul brilhante (0.2, 0.8, 1)
Max Emission: 3.0
Scale: 0.85 - 1.25
Enable Rotation: true
Rotation Speed: 45
```

---

## ?? Suporte

Se tiver problemas ou dúvidas:
1. Verifique se o script está anexado ao GameObject correto
2. Confirme que há um `SpriteRenderer` no mesmo GameObject
3. Teste com valores padrão primeiro
4. Consulte os exemplos acima

---

**Criado para facilitar a aplicação de efeitos visuais aos itens do jogo! ???**
