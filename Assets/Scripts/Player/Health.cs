using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    public Transform healthBar;
    public GameObject healthBarObject;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSound;
    [Range(0f, 2f)]
    [SerializeField] private float damageVolume = 1f;

    private Vector3 healthBarScale;
    private float healthPercent;

    void Start()
    {
        currentHealth = maxHealth;
        
        if (healthBar != null)
        {
            healthBarScale = healthBar.localScale;
            healthPercent = healthBarScale.x / maxHealth; 
            UpdateHealthbar();
        }
    }

    public void RestoreHealthFull()
    {
        currentHealth = maxHealth;
        UpdateHealthbar();
        Debug.Log("♥ Vida restaurada: " + currentHealth + "/" + maxHealth);
    }

    /// <summary>
    /// Cura uma quantidade específica de vida, sem ultrapassar o máximo.
    /// </summary>
    public void Heal(int amount)
    {
        if (amount <= 0) return;

        int previousHealth = currentHealth;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthbar();

        int actualHealing = currentHealth - previousHealth;
        if (actualHealing > 0)
        {
            Debug.Log($"♥ {gameObject.name} curou {actualHealing} de vida. ({currentHealth}/{maxHealth})");
        }
    }

    /// <summary>
    /// Aumenta a vida máxima e atualiza a barra de vida proporcionalmente.
    /// </summary>
    public void IncreaseMaxHealth(int amount)
    {
        if (amount <= 0) return;

        maxHealth += amount;
        currentHealth += amount; // Aumenta a vida atual também
        
        // Recalcula o percentual da barra de vida com o novo máximo
        if (healthBar != null)
        {
            healthPercent = healthBarScale.x / maxHealth;
        }
        
        UpdateHealthbar();
        Debug.Log($"⚡ Vida máxima aumentada! Novo máximo: {maxHealth}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthbar();

        // Reproduz o som de dano com o volume ajustável
        if (AudioManager.Instance != null && damageSound != null)
        {
            AudioManager.Instance.PlaySound(damageSound, transform.position, damageVolume);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("☠ " + gameObject.name + " morreu.");
        
        gameObject.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PersonagemMorreu();
        }
        else
        {
            Debug.LogError("❌ GameManager não encontrado! Recarregando cena.");
            SceneManager.LoadScene("Overworld");
        }
    }

    void UpdateHealthbar()
    {
        if (healthBar != null)
        {
            healthBarScale.x = healthPercent * currentHealth;
            healthBar.localScale = healthBarScale;
        }
    }
}