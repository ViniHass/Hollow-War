using UnityEngine;
using System.Collections;
using System.Linq;

public class DetectionZone : MonoBehaviour
{
    private EnemyAI parentAI;
    private CircleCollider2D detectionCollider;
    private Transform currentTarget = null;

    [Header("Settings")]
    [Tooltip("Quais layers contêm o Player e o Decoy?")]
    public LayerMask targetLayers; // Configure isso no Inspector!
    
    [Tooltip("Intervalo em segundos entre as verificações de área (Otimização).")]
    public float detectionRate = 0.2f; 

    void Awake()
    {
        parentAI = GetComponentInParent<EnemyAI>();
        detectionCollider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        // Inicia a rotina de detecção otimizada
        StartCoroutine(DetectionRoutine());
    }

    // Substituímos o Update por essa Coroutine
    IEnumerator DetectionRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(detectionRate);

        while (true)
        {
            DetectTargets();
            yield return wait;
        }
    }

    private void DetectTargets()
    {
        // 1. Procura alvos usando a LayerMask configurada
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            detectionCollider.radius, 
            targetLayers
        );

        // 2. Lógica de prioridade (Decoy > Player)
        Transform bestTarget = FindBestTarget(hits);

        // 3. Atualiza a IA apenas se o alvo mudou
        if (bestTarget != currentTarget)
        {
            if (bestTarget != null)
                parentAI.SetTarget(bestTarget);
            else
                parentAI.ClearTarget();
            
            currentTarget = bestTarget;
        }
    }

    private Transform FindBestTarget(Collider2D[] hits)
    {
        if (hits.Length == 0) return null;

        // Prioridade 1: Decoy
        var decoy = hits.FirstOrDefault(h => h.CompareTag("Decoy"));
        if (decoy) return decoy.transform;

        // Prioridade 2: Player
        var player = hits.FirstOrDefault(h => h.CompareTag("Player"));
        // Assume que o colisor é o Hurtbox e pega o pai, ajuste se necessário
        if (player) return player.transform.parent != null ? player.transform.parent : player.transform;

        return null;
    }

    void OnDrawGizmosSelected()
    {
        if (detectionCollider == null) detectionCollider = GetComponent<CircleCollider2D>();
        Gizmos.color = new Color(1, 1, 0, 0.2f);
        Gizmos.DrawSphere(transform.position, detectionCollider.radius);
    }
}