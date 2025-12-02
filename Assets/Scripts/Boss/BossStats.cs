using UnityEngine;

[CreateAssetMenu(fileName = "NewBossStats", menuName = "Boss/New Boss Stats")]
public class BossStats : ScriptableObject
{
    [Header("Saúde e Defesa")]
    public int maxHealth = 10;
    public float barrierDuration = 5.0f;
    public float damagePerAttack = 1.0f;

    [Header("Comportamento (IA)")]
    public float attackCooldown = 4.0f; 
    public float maxDetectionRange = 25f; 
    public float maxPlayerDistance = 20f; 

    [Header("Teleporte")]
    public float teleportOutAnimationDuration = 0.5f; 
    public float teleportReappearDelay = 0.3f;      
    public float teleportMinDistance = 6f;          
    public float teleportMaxDistance = 10f;

    [Header("Timers de 'Aviso' (Delay)")]
    public float castDelayOrb = 0.5f; 
    public float castDelaySword = 0.3f; 
    // (castDelayRock foi removido)

    [Header("Stats do Ataque (Orbe)")]
    public float orbSpeed = 8f;
    public float orbLifetime = 5f; 

    // --- ADICIONE ESTA SEÇÃO ---
    [Header("Stats do Ataque (Espada)")]
    [Tooltip("Duração do giro de 180° da espada, em segundos")]
    public float swordArcDuration = 0.5f;
    [Tooltip("Tempo que a hitbox da espada fica ativa (deve ser > ArcDuration)")]
    public float swordLifetime = 0.7f;
}