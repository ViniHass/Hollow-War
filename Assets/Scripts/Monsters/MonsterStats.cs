using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterStats", menuName = "Stats/Monster Stats")]
public class MonsterStats : ScriptableObject
{
    [Header("Base Stats")]
    public int maxHealth = 3;
    public float moveSpeed = 2f;
    public int attackDamage = 1;

    [Header("AI Behavior")]
    [Tooltip("O raio da zona onde o inimigo detecta o player.")]
    public float detectionRadius = 8f;
    
    [Tooltip("A distância máxima que o inimigo consegue acertar o player.")]
    public float attackRadius = 1f;

    [Tooltip("A distância que o inimigo vai PARAR do player (deve ser menor ou igual ao Attack Radius).")]
    public float stopDistance = 0.8f; // <--- NOVO

    [Tooltip("Tempo de espera entre os ataques em segundos.")]
    public float attackCooldown = 2f;

    [Header("Combat Timings")]
    public float attackHitboxDelay = 0.5f;
    public float attackHitboxActiveTime = 0.3f;

    [Tooltip("Tempo que o inimigo fica parado APÓS o ataque antes de voltar a andar.")]
    public float attackRecovery = 0.8f;         // <--- ADICIONE ISSO (Sugestão: 0.7s)

    [Header("Feedback on Hit")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.3f;
    public float flashDuration = 0.2f;
}