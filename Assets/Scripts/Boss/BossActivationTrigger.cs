using UnityEngine;

public class BossActivationTrigger : MonoBehaviour
{
    // Arraste o "SkeletonKing" (o pai) aqui no Inspector
    public SkeletonKing_AI bossAI;

    private void Start()
    {
        // Configura o raio do trigger usando o BossStats
        if (bossAI != null && bossAI.stats != null)
        {
            GetComponent<CircleCollider2D>().radius = bossAI.stats.maxDetectionRange;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossAI.ActivateBoss();
            gameObject.SetActive(false); // Desativa para não ativar de novo
        }
    }
}