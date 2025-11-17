using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    [Tooltip("Velocidade padrão ao resetar")]
    public float defaultMoveSpeed = 5f;

    [Header("Health")]
    public int maxHealth = 20;
    [Tooltip("Vida máxima padrão ao resetar")]
    public int defaultMaxHealth = 20;

    [Header("Combat")]
    public int attackDamage = 1;
    [Tooltip("Dano padrão ao resetar")]
    public int defaultAttackDamage = 1;
    
    [Tooltip("O delay em segundos desde o início do ataque até a hitbox ser ativada.")]
    public float attackHitboxDelay = 0.1f;
    [Tooltip("Por quanto tempo a hitbox permanecerá ativa em segundos.")]
    public float attackHitboxActiveTime = 0.15f;
    [Tooltip("A duração total da animação de ataque. Deve ser maior que a soma do delay e do tempo ativo.")]
    public float attackAnimationDuration = 0.36f;

    [Header("Dagger Skill")]
    [Tooltip("Dano causado pela adaga lançada.")]
    public int daggerDamage = 2;
    [Tooltip("Dano padrão da adaga ao resetar")]
    public int defaultDaggerDamage = 2;
    
    [Tooltip("Velocidade de movimento da adaga em unidades/segundo.")]
    public float daggerSpeed = 10f;
    
    [Tooltip("Tempo de vida da adaga em segundos antes de desaparecer.")]
    public float daggerLifetime = 3f;
    
    [Tooltip("Cooldown em segundos para lançar outra adaga.")]
    public float daggerCooldown = 2f;
    [Tooltip("Cooldown padrão da adaga ao resetar")]
    public float defaultDaggerCooldown = 2f;

    [Header("Decoy Skill")]
    [Tooltip("Por quantos segundos o decoy permanece ativo.")]
    public float decoyDuration = 5f;
    [Tooltip("Duração padrão do decoy ao resetar")]
    public float defaultDecoyDuration = 5f;
    
    [Tooltip("O tempo em segundos até que a skill possa ser usada novamente.")]
    public float decoyCooldown = 90f;
    [Tooltip("Cooldown padrão do decoy ao resetar")]
    public float defaultDecoyCooldown = 90f;
    
    [Tooltip("Duração da animação de destruição do decoy (em segundos).")]
    public float decoyDestructionAnimTime = 1f;

    /// <summary>
    /// Restaura todos os stats aos valores padrão
    /// </summary>
    public void ResetToDefaults()
    {
        moveSpeed = defaultMoveSpeed;
        maxHealth = defaultMaxHealth;
        attackDamage = defaultAttackDamage;
        daggerDamage = defaultDaggerDamage;
        daggerCooldown = defaultDaggerCooldown;
        decoyDuration = defaultDecoyDuration;
        decoyCooldown = defaultDecoyCooldown;
        
        Debug.Log($"🔄 PlayerStats resetado:\n" +
                 $"  • Velocidade: {moveSpeed}\n" +
                 $"  • Vida Máxima: {maxHealth}\n" +
                 $"  • Dano: {attackDamage}\n" +
                 $"  • Dano Adaga: {daggerDamage}\n" +
                 $"  • Cooldown Adaga: {daggerCooldown}s\n" +
                 $"  • Cooldown Decoy: {decoyCooldown}s\n" +
                 $"  • Duração Decoy: {decoyDuration}s");
    }
}