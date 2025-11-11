using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " tomou " + damage + " de dano. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " morreu.");
        // Por enquanto, vamos apenas desativar o objeto.
        // Mais tarde, você pode adicionar animações de morte, efeitos, etc.

        gameObject.SetActive(false);

        SceneManager.LoadScene("Overworld");

        // Carrega a segunda cena sem descarregar a primeira
        SceneManager.LoadScene("Pradaria", LoadSceneMode.Additive);
    }
}