# ?? Sistema de Destaque para Itens - Guia Rápido

## ? O que foi criado?

Três novos scripts para adicionar efeitos visuais aos itens do seu jogo:

1. **ItemHighlight.cs** - Versão simples (recomendada)
2. **ItemHighlightEmissive.cs** - Versão avançada com brilho intenso
3. **ItemHighlightEditorTool.cs** - Ferramenta para aplicar automaticamente

---

## ?? Como Usar (3 Métodos)

### Método 1: Manual (Item por Item)

1. Abra seu prefab de item no Inspector
2. Clique em **"Add Component"**
3. Digite **"ItemHighlight"**
4. Pronto! O efeito já está aplicado

**Exemplo:** Adicionar ao prefab "Key.prefab", "Potion.prefab", etc.

---

### Método 2: Ferramenta Automática (RECOMENDADO) ?

1. No Unity, vá no menu: **Tools > Item Highlight > Configurar Destaque de Itens**
2. Configure a pasta onde estão seus prefabs (ex: `Assets/Prefabs/Items`)
3. Ajuste as configurações visuais (cor, intensidade, etc.)
4. Clique em **"? Aplicar a Todos os Prefabs"**
5. Pronto! Todos os itens agora têm o efeito!

**Vantagens:**
- Aplica em todos os itens de uma vez
- Mantém configurações consistentes
- Pode remover ou atualizar facilmente

---

### Método 3: Via Código

Se você criar itens dinamicamente, adicione o script no código:

```csharp
GameObject item = Instantiate(itemPrefab);
ItemHighlight highlight = item.AddComponent<ItemHighlight>();
```

---

## ?? Configurações Principais

### Para a maioria dos itens:
```
? Glow Color: Amarelo (1, 1, 0)
?? Glow Intensity: 0.5
? Pulse Speed: 2.0
?? Min Scale: 0.9
?? Max Scale: 1.1
```

### Para itens especiais/raros:
```
? Use ItemHighlightEmissive
? Emission Color: Dourado (1, 0.9, 0)
?? Max Emission: 2.5
?? Min Scale: 0.85
?? Max Scale: 1.2
?? Enable Rotation: ?
```

---

## ?? Cores Sugeridas

| Tipo de Item | Cor Recomendada | RGB |
|--------------|----------------|-----|
| Item Comum | Amarelo | (1, 1, 0) |
| Item Raro | Dourado | (1, 0.9, 0) |
| Item Mágico | Azul | (0, 0.5, 1) |
| Item de Vida | Verde | (0, 1, 0) |
| Item Especial | Roxo | (0.8, 0, 1) |

---

## ?? Checklist de Uso

- [ ] Escolher qual script usar (ItemHighlight para maioria dos casos)
- [ ] Aplicar aos prefabs (manual ou automático)
- [ ] Testar na cena (Play Mode)
- [ ] Ajustar configurações se necessário
- [ ] Salvar os prefabs

---

## ?? Funções Úteis

### Desativar o efeito temporariamente:
```csharp
GetComponent<ItemHighlight>().DisableHighlight();
```

### Ativar novamente:
```csharp
GetComponent<ItemHighlight>().EnableHighlight();
```

### Mudar a cor (versão Emissive):
```csharp
GetComponent<ItemHighlightEmissive>().SetGlowColor(Color.red);
```

---

## ? FAQ Rápido

**P: O efeito não aparece?**
R: Verifique se o GameObject tem um `SpriteRenderer`.

**P: Posso ter cores diferentes para itens diferentes?**
R: Sim! Configure individualmente no Inspector de cada prefab.

**P: Como removo o efeito de todos os itens?**
R: Use a ferramenta: **Tools > Item Highlight > Configurar** e clique em "? Remover de Todos".

**P: Qual versão devo usar?**
R: Use `ItemHighlight` (simples) para 90% dos casos. Use `ItemHighlightEmissive` apenas para itens muito especiais.

---

## ?? Localização dos Arquivos

```
Assets/
??? Scripts/
?   ??? Items/
?       ??? ItemHighlight.cs              ? Script principal (simples)
?       ??? ItemHighlightEmissive.cs      ? Script avançado
??? Editor/
?   ??? ItemHighlightEditorTool.cs        ? Ferramenta automática
??? Docs/
    ??? GUIA_DESTAQUE_ITENS.md            ? Guia completo
    ??? QUICK_START_ITEM_HIGHLIGHT.md     ? Este arquivo
```

---

## ?? Exemplo Prático

### Cenário: Adicionar destaque a uma chave

1. Localize o prefab: `Assets/Prefabs/Items/Key.prefab`
2. Selecione-o no Project
3. No Inspector, clique em "Add Component"
4. Digite "ItemHighlight" e adicione
5. Ajuste a cor para amarelo (já é padrão)
6. Salve (Ctrl+S)
7. Entre em Play Mode e veja o resultado!

---

## ?? Dicas Profissionais

1. **Consistência**: Use as mesmas configurações para itens do mesmo tipo
2. **Destaque**: Itens raros devem ter `Max Scale` maior (1.2+)
3. **Performance**: Use `ItemHighlight` simples quando possível
4. **Teste**: Sempre teste em Play Mode antes de finalizar
5. **Backup**: Faça backup dos prefabs antes de aplicar em massa

---

## ?? Precisa de Ajuda?

Consulte o guia completo em: `Docs/GUIA_DESTAQUE_ITENS.md`

---

**Criado para o projeto Hollow War ??**
**Sistema de destaque visual para itens colecionáveis ?**
