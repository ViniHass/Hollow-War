using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Animador")]
    [SerializeField] private Animator anim; // arraste o Animator do child Sprite aqui

    private Vector2 mov;              // input do player
    private Vector2 lastMoveDir;      // última direção usada para animação

    // Singleton opcional
    public static PlayerController Instance { get; private set; }

    private void Awake()
    {
        // Configura singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        HandleInput();
        UpdateAnimator();
        HandleAttack();
    }

    private void FixedUpdate()
    {
        Move();
    }

    // -----------------------------
    private void HandleInput()
    {
        var keyboard = Keyboard.current;

        mov = Vector2.zero;
        if (keyboard.wKey.isPressed) mov.y += 1;
        if (keyboard.sKey.isPressed) mov.y -= 1;
        if (keyboard.aKey.isPressed) mov.x -= 1;
        if (keyboard.dKey.isPressed) mov.x += 1;

        mov = mov.normalized;

        // salva última direção não nula
        if (mov != Vector2.zero)
            lastMoveDir = mov;
    }

    private void Move()
    {
        if (rb != null)
            rb.MovePosition(rb.position + mov * speed * Time.fixedDeltaTime);
    }

    private void UpdateAnimator()
    {
        if (anim == null) return;

        anim.SetFloat("Horizontal", lastMoveDir.x);
        anim.SetFloat("Vertical", lastMoveDir.y);
        anim.SetFloat("Speed", mov.sqrMagnitude);
    }

    private void HandleAttack()
    {
        if (anim == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            anim.SetTrigger("Attack");
        }
    }
}
