using UnityEngine;

// Este script deve ser colocado no GameObject que define a área da arena.
public class ArenaTilemapToggle : MonoBehaviour
{
    [Header("Referência")]
    [Tooltip("Arraste aqui o GameObject do Tilemap que você quer desativar.")]
    public GameObject tilemapToToggle;

    // Garante que o GameObject tenha um Collider 2D
    private void Awake()
    {
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("Este script precisa de um Collider2D no mesmo GameObject para funcionar!", this);
        }
    }

    // Chamado quando o player ENTRA no trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Desativa o GameObject do Tilemap
            if (tilemapToToggle != null)
            {
                tilemapToToggle.SetActive(false);
            }
        }
    }

    // Chamado quando o player SAI do trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        // Verifica se quem saiu tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Reativa o GameObject do Tilemap
            if (tilemapToToggle != null)
            {
                tilemapToToggle.SetActive(true);
            }
        }
    }
}