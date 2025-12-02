# ?? GUIA DE CONFIGURAÇÃO - Sistema de Adagas/Pedras

## ? O QUE FOI IMPLEMENTADO

O sistema de lançamento de adagas/pedras foi modificado para **CONSUMIR** um item do inventário a cada uso.

### Mudanças no PlayerController:
- ? Adicionada referência ao componente `Inventory`
- ? Adicionado campo `requiredStoneItem` (ItemData da pedra necessária)
- ? Verificação de inventário antes de lançar
- ? Remoção automática da pedra ao lançar
- ? Mensagem de feedback ao jogador quando não tem pedras

---

## ?? PASSOS PARA CONFIGURAR NO UNITY

### 1?? CRIAR O ITEM "PEDRA1"

1. No Unity Editor, clique com **botão direito** na pasta `Assets/Items` (ou crie essa pasta)
2. Navegue para: **Create > Inventory > Item Data**
3. Nomeie o arquivo como **"Pedra1"**
4. Clique no arquivo e configure no Inspector:
   - **Item Name:** `pedra1`
   - **Description:** `Uma pedra que pode ser lançada contra inimigos`

---

### 2?? CONFIGURAR O PLAYERCONTROLLER

1. Selecione o **GameObject do Player** na hierarquia
2. Localize o componente **PlayerController** no Inspector
3. Encontre o campo **"Required Stone Item"** (na seção Skill: Dagger/Pedra)
4. **Arraste** o asset **"Pedra1"** (criado no passo 1) para este campo

---

### 3?? CRIAR PREFAB DE PEDRA COLETÁVEL

1. Crie um **GameObject vazio** na hierarquia (clique direito > Create Empty)
2. Renomeie para **"StonePickup"**
3. Adicione os seguintes componentes:

   **a) SpriteRenderer:**
   - Arraste a sprite da pedra no campo **"Sprite"**
   - Configure **Order in Layer** conforme necessário

   **b) Collider2D (BoxCollider2D ou CircleCollider2D):**
   - ? Marque **"Is Trigger"**
   - Ajuste o tamanho para cobrir a sprite

   **c) ItemPickup (Script):**
   - Clique em **"Add Component"**
   - Procure por **"ItemPickup"**
   - No campo **"Item To Give"**, arraste o asset **"Pedra1"**

4. Salve como **Prefab:**
   - Arraste o GameObject da hierarquia para a pasta `Assets/Prefabs/Items`
   - Agora você pode deletar da hierarquia e usar o prefab

---

### 4?? ESPALHAR PEDRAS PELO MAPA

**Opção A - Manual:**
1. Arraste o prefab **"StonePickup"** para a cena
2. Posicione onde desejar
3. Repita para criar múltiplas pedras

**Opção B - Organizado:**
1. Crie um GameObject vazio chamado **"--- ITEM PICKUPS ---"**
2. Arraste os StonePickup como filhos deste objeto
3. Facilita organização e visualização

---

## ?? COMO FUNCIONA NO JOGO

### Coletar Pedra:
1. Jogador se aproxima da pedra
2. Aparece mensagem: **"Pressione E para pegar item"**
3. Pressiona **E**
4. Pedra é adicionada ao inventário
5. GameObject da pedra desaparece do mundo

### Lançar Pedra:
1. Jogador pressiona **Botão Direito do Mouse**
2. Sistema verifica:
   - ? Tem pedra no inventário? ? Lança
   - ? Não tem pedra? ? Mostra mensagem "Você precisa de uma pedra para lançar!"
3. Se lançar com sucesso:
   - Pedra é **removida** do inventário
   - Projétil é criado na direção do mouse
   - Cooldown é iniciado

---

## ?? CONFIGURAÇÕES OPCIONAIS

### Ajustar Cooldown da Pedra:
1. Abra o **PlayerStats** (ScriptableObject)
2. Modifique **"Dagger Cooldown"**

### Ajustar Dano da Pedra:
1. Abra o **PlayerStats**
2. Modifique **"Dagger Damage"**

### Velocidade do Projétil:
1. Abra o **PlayerStats**
2. Modifique **"Dagger Speed"**

### Tempo de Vida do Projétil:
1. Abra o **PlayerStats**
2. Modifique **"Dagger Lifetime"**

---

## ?? TESTANDO

1. Inicie o jogo
2. Colete uma pedra (E)
3. Abra o inventário (I) - deve aparecer "pedra1" no log
4. Lance com **Botão Direito do Mouse**
5. Tente lançar novamente sem ter pedra - deve aparecer mensagem de erro
6. Colete outra pedra e teste novamente

---

## ?? CHECKLIST DE VERIFICAÇÃO

- [ ] Item "Pedra1" criado no Unity
- [ ] PlayerController tem referência ao "Pedra1"
- [ ] Prefab StonePickup criado com todos os componentes
- [ ] Collider2D marcado como "Is Trigger"
- [ ] ItemPickup configurado com "Pedra1"
- [ ] Pedras espalhadas pelo mapa
- [ ] Player tem componente Inventory
- [ ] Testado: coleta funciona
- [ ] Testado: lançamento funciona
- [ ] Testado: mensagem de erro funciona

---

## ?? TROUBLESHOOTING

**Problema:** Não consigo lançar pedras mesmo com elas no inventário
- ? Verifique se o campo "Required Stone Item" no PlayerController está preenchido
- ? Verifique se o nome do item é exatamente "pedra1" (case sensitive)
- ? Olhe no console se há erros

**Problema:** Não consigo coletar pedras
- ? Verifique se o Collider2D está marcado como "Is Trigger"
- ? Verifique se o Player tem a tag "Player"
- ? Verifique se o ItemPickup tem o item "Pedra1" atribuído

**Problema:** Pedras desaparecem mas não vão para o inventário
- ? Verifique se o Player tem o componente "Inventory"
- ? Verifique no console se há mensagens de erro

---

## ?? PRÓXIMOS PASSOS SUGERIDOS

1. **UI do Inventário:** Mostrar quantas pedras o jogador tem
2. **Diferentes Tipos:** Criar "Pedra2", "Pedra3" com efeitos diferentes
3. **Limite de Inventário:** Máximo de X pedras por vez
4. **Pedras Especiais:** Pedra de fogo, gelo, etc.
5. **Recarga Automática:** Pedras reaparecem após X segundos

---

## ?? NOTAS DO DESENVOLVEDOR

- O sistema usa o mesmo script `DaggerProjectile.cs` para o projétil
- Mantivemos o nome "dagger" no código por compatibilidade
- O item pode ter qualquer nome no ItemData (não precisa ser "pedra1")
- O sistema suporta múltiplas pedras no inventário (não há limite)
- Cada lançamento consome apenas 1 pedra
