using UnityEngine;
using System.Linq; // Necessário para .FirstOrDefault e .OrderBy

public class DetectionZone : MonoBehaviour
{
    private EnemyAI parentAI;
    private CircleCollider2D detectionCollider;

    // Guarda o que a IA está mirando atualmente
    private Transform currentTarget = null;
    
    // As "tags" que este script procura
    private LayerMask detectionLayer;

    void Awake()
    {
        parentAI = GetComponentInParent<EnemyAI>();
        detectionCollider = GetComponent<CircleCollider2D>();

        // Isso cria uma "LayerMask" que só inclui as camadas (layers)
        // onde o Player e o Decoy estão.
        // IMPORTANTE: Certifique-se de que seu Player e Decoy
        // estão em layers como "Default" ou uma layer customizada "Targetable".
        // Ajuste "Default" se o seu player/decoy estiver em outra layer.
        detectionLayer = LayerMask.GetMask("Default"); 
        // Se você criou uma layer "Player" e "Decoy", use:
        // detectionLayer = LayerMask.GetMask("Player", "Decoy");
    }

    // Update é mais confiável que FixedUpdate para detecção de IA
    void Update()
    {
        // 1. Procura ATIVAMENTE por alvos
        // Isso é 1000x mais confiável que OnTrigger
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, 
            detectionCollider.radius, 
            detectionLayer
        );

        // 2. Procura o MELHOR alvo
        Transform bestTarget = FindBestTarget(hits);

        // 3. OTIMIZAÇÃO: Só avisa a IA se o alvo MUDOU
        if (bestTarget != currentTarget)
        {
            if (bestTarget != null)
            {
                parentAI.SetTarget(bestTarget);
            }
            else
            {
                parentAI.ClearTarget();
            }
            
            currentTarget = bestTarget; // Atualiza o que a IA sabe
        }
    }

    private Transform FindBestTarget(Collider2D[] hits)
    {
        if (hits.Length == 0)
        {
            return null; // Ninguém está na zona
        }

        // --- PRIORIDADE 1: DECOY ---
        // Procura por qualquer colisor com a tag "Decoy"
        Collider2D decoyHit = hits.FirstOrDefault(hit => hit.CompareTag("Decoy"));
        if (decoyHit != null)
        {
            // Achamos um Decoy, ele tem prioridade máxima
            return decoyHit.transform; // Retorna o transform do Decoy
        }

        // --- PRIORIDADE 2: PLAYER ---
        // Se não achou Decoy, procura pelo Player
        Collider2D playerHit = hits.FirstOrDefault(hit => hit.CompareTag("Player"));
        if (playerHit != null)
        {
            // Achamos o Player
            return playerHit.transform.parent; // Retorna o transform PAI do Hurtbox
        }

        // Se chegou aqui, achou colisores, mas nenhum era Player ou Decoy
        return null;
    }

    // Opcional: Desenha o raio de detecção no Editor para debugging
    void OnDrawGizmos()
    {
        if (detectionCollider == null)
            detectionCollider = GetComponent<CircleCollider2D>();
            
        Gizmos.color = new Color(1, 0, 0, 0.2f); // Vermelho transparente
        Gizmos.DrawSphere(transform.position, detectionCollider.radius);
    }
}