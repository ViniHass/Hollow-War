using UnityEngine;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private PlayerStats stats;

    [Header("Hitbox References")]
    [SerializeField] private GameObject hitboxNE;
    [SerializeField] private GameObject hitboxNW;
    [SerializeField] private GameObject hitboxSW;
    [SerializeField] private GameObject hitboxSE;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSound; // Arraste seu MP3 aqui

    // Eventos
    public static event Action<Vector2> OnMove;
    public static event Action<Vector2> OnAttack;

    // Componentes e Variáveis
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 lastDirection = new Vector2(1, 1);
    private bool isAttacking = false;
    private Hitbox activeHitbox;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // Garante que todas as hitboxes começam desativadas
        hitboxNE.SetActive(false);
        hitboxNW.SetActive(false);
        hitboxSW.SetActive(false);
        hitboxSE.SetActive(false);
    }

    void Update()
    {
        if (isAttacking)
        {
            movementInput = Vector2.zero;
            OnMove?.Invoke(movementInput);
            return;
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput.magnitude > 0)
        {
            lastDirection = movementInput;
        }

        OnMove?.Invoke(movementInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(AttackCoroutine());
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

        // Toca o som de ataque (ajuste o volume aqui: 1f = 100%, 2f = 200%)
        if (AudioManager.Instance != null && attackSound != null)
        {
            AudioManager.Instance.PlaySound(attackSound, transform.position, 10f);
        }

        // 1. Espera o delay inicial
        yield return new WaitForSeconds(stats.attackHitboxDelay);

        // 2. Determina qual hitbox ativar com base na última direção
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

        // 3. Espera o tempo em que a hitbox ficará ativa
        yield return new WaitForSeconds(stats.attackHitboxActiveTime);

        // 4. Desativa a hitbox
        if (hitboxToActivate != null)
        {
            hitboxToActivate.SetActive(false);
        }

        // 5. Espera o restante da animação para destravar o movimento
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
}