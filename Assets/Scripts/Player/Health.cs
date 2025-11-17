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