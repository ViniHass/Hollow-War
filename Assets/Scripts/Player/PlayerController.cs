using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PlayerStats stats; // Seu gamedata

    [Header("Hitbox References")]
    [SerializeField] private GameObject hitboxNE;
    [SerializeField] private GameObject hitboxNW;
    [SerializeField] private GameObject hitboxSW;
    [SerializeField] private GameObject hitboxSE;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip footstepLoop; 
    [Range(0f, 2f)]
    [SerializeField] private float footstepVolume = 0.5f;

    // --- NOVAS LINHAS ADICIONADAS ---
    [Header("Skill: Decoy")]
    [SerializeField] private GameObject decoyPrefab; // Arraste seu prefab 'Decoy'
    [SerializeField] private float decoySpawnOffset = 1.0f;
    
    private float decoyCooldownTimer = 0f;
    private bool isDecoyOnCooldown = false;
    // --- FIM DAS NOVAS LINHAS ---

    // Eventos
    public static event Action<Vector2> OnMove;
    public static event Action<Vector2> OnAttack;

    // Componentes e Variáveis
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lastDirection = new Vector2(1, 1);
    private bool isAttacking = false;
    private Hitbox activeHitbox;
    private AudioSource footstepAudioSource; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        footstepAudioSource = gameObject.AddComponent<AudioSource>();
        footstepAudioSource.clip = footstepLoop;
        footstepAudioSource.loop = true; 
        footstepAudioSource.playOnAwake = false;
        footstepAudioSource.volume = footstepVolume;

        hitboxNE.SetActive(false);
        hitboxNW.SetActive(false);
        hitboxSW.SetActive(false);
        hitboxSE.SetActive(false);
    }

    void Update()
    {
        // --- NOVO: Gerenciamento do Cooldown ---
        if (isDecoyOnCooldown)
        {
            decoyCooldownTimer -= Time.deltaTime;
            if (decoyCooldownTimer <= 0)
            {
                isDecoyOnCooldown = false;
                Debug.Log("Skill Decoy pronta!");
            }
        }

        if (isAttacking)
        {
            movementInput = Vector2.zero;
            OnMove?.Invoke(movementInput);
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput.magnitude > 0)
        {
            lastDirection = movementInput;
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.volume = footstepVolume;
                footstepAudioSource.Play();
            }
        }
        else
        {
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }

        OnMove?.Invoke(movementInput);

        // Input de Ataque
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackCoroutine());
        }

        // --- NOVO: Input da Skill Decoy (Tecla '1') ---
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isAttacking && !isDecoyOnCooldown)
        {
            UseDecoy();
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementInput * stats.moveSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        OnAttack?.Invoke(lastDirection);

        if (AudioManager.Instance != null && attackSound != null)
        {
            AudioManager.Instance.PlaySound(attackSound, transform.position, 1f);
        }

        yield return new WaitForSeconds(stats.attackHitboxDelay);
        GameObject hitboxToActivate = GetHitboxForDirection(lastDirection);
        if (hitboxToActivate != null)
        {
            hitboxToActivate.SetActive(true);
            activeHitbox = hitboxToActivate.GetComponent<Hitbox>();
            if (activeHitbox != null)
            {
                activeHitbox.damage = stats.attackDamage;
            }
        }
        yield return new WaitForSeconds(stats.attackHitboxActiveTime);
        if (hitboxToActivate != null)
        {
            hitboxToActivate.SetActive(false);
        }
        float remainingTime = stats.attackAnimationDuration - stats.attackHitboxDelay - stats.attackHitboxActiveTime;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }
        isAttacking = false;
    }

    private GameObject GetHitboxForDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > 22.5f && angle <= 67.5f) return hitboxNE;
        if (angle > 67.5f && angle <= 112.5f) return hitboxNE;
        if (angle > 112.5f && angle <= 157.5f) return hitboxNW;
        if (angle > 157.5f || angle <= -157.5f) return hitboxNW;
        if (angle > -157.5f && angle <= -112.5f) return hitboxSW;
        if (angle > -112.5f && angle <= -67.5f) return hitboxSW;
        if (angle > -67.5f && angle <= -22.5f) return hitboxSE;
        if (angle > -22.5f && angle <= 22.5f) return hitboxSE;

        return hitboxNE;
    }

    // --- NOVO MÉTODO PARA USAR O DECOY ---
    private void UseDecoy()
    {
        isDecoyOnCooldown = true;
        decoyCooldownTimer = stats.decoyCooldown; // Lê do gamedata
        Debug.Log("Usou Decoy! Cooldown de " + stats.decoyCooldown + "s iniciado.");

        // 1. Determina a direção (Leste ou Oeste)
        bool facingRight = lastDirection.x >= 0;

        // 2. Calcula a posição do spawn
        Vector2 spawnDirection = facingRight ? Vector2.right : Vector2.left;
        Vector3 spawnPosition = transform.position + (Vector3)spawnDirection * decoySpawnOffset;

        // 3. Instancia (cria) o Decoy do prefab
        GameObject decoyInstance = Instantiate(decoyPrefab, spawnPosition, Quaternion.identity);

        // 4. "Liga" o Decoy, passando os stats E a direção
        Decoy decoyScript = decoyInstance.GetComponent<Decoy>();
        if (decoyScript != null)
        {
            // Passa os valores lidos do gamedata e a direção
            decoyScript.Initialize(stats.decoyDuration, stats.decoyDestructionAnimTime, facingRight);
        }
        else
        {
            Debug.LogError("O prefab 'Decoy' está sem o script Decoy.cs!");
        }
    }
}