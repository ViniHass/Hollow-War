using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Configurações")]
    [Tooltip("Desativa após ser usado uma vez")]
    public bool desativarAposUso = false;
    
    [Header("Efeitos Visuais (Opcional)")]
    public ParticleSystem particulas;
    public AudioClip somCheckpoint;
    
    private bool jaUsado = false;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        // Garante que tem um Collider2D configurado como trigger
        Collider2D col = GetComponent<Collider2D>(); 
        if (col != null)
        {
            col.isTrigger = true;
        }
        else
        {
            Debug.LogWarning("⚠ CheckpointTrigger precisa de um Collider2D!");
        }
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        // Certifique-se que o Player tem a tag "Player" e um Rigidbody2D!
        if (other.CompareTag("Player"))
        {
            if (jaUsado && desativarAposUso)
                return;
            
            if (GameManager.Instance != null)
            {
                // 1. Define o Checkpoint no GameManager
                GameManager.Instance.SetCheckpoint(transform.position);
                
                jaUsado = true;
                
                MostrarEfeito();

                // 2. 🌟 Exibe a mensagem usando o UIManager global 🌟
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.ShowGlobalMessage("Checkpoint Salvo!");
                }
                
                if (desativarAposUso)
                {
                    enabled = false; 
                }
                
                Debug.Log($"✓ Checkpoint ativado: {gameObject.name}");
            }
        }
    }
    
    void MostrarEfeito()
    {
        if (particulas != null)
        {
            particulas.Play();
        }
        
        if (somCheckpoint != null && audioSource != null)
        {
            audioSource.PlayOneShot(somCheckpoint);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}