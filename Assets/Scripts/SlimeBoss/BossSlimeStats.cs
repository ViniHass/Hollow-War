using UnityEngine;

[CreateAssetMenu(fileName = "NewBossSlimeStats", menuName = "Stats/Boss Slime Stats")]
public class BossSlimeStats : ScriptableObject
{
    [Header("Configuração Geral")]
    public int maxHealth = 5;

    [Header("Detecção")]
    [Tooltip("O raio de distância para o player 'acordar' o slime e começar a atacar.")]
    // AJUSTE: Mudei de 10f para 5f para ser igual ao maxAttackRadius
    public float detectionRange = 5f; 

    [Header("Ataque de Espinhos (360°)")]
    [Tooltip("Dano do ataque de espinhos.")]
    public int attackDamage = 2;

    [Tooltip("O tempo (em segundos) da animação de 'aviso' antes do ataque realmente causar dano.")]
    public float attackCastTime = 0.8f;

    [Tooltip("O tempo (em segundos) que os espinhos levam para crescer do centro até o raio máximo.")]
    public float attackActiveDuration = 0.5f;
    
    [Tooltip("O raio máximo (em unidades do Unity) que os espinhos atingirão.")]
    public float maxAttackRadius = 5f; // O detectionRange agora é igual a este valor

    [Tooltip("O tempo (em segundos) de espera entre o fim de um ataque e o início de outro.")]
    public float attackCooldown = 3f;
}