using UnityEngine;
using System.Collections;

public class Decoy : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite spriteDireita;
    [SerializeField] private Sprite spriteEsquerda;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); 
    }

    // O PlayerController vai chamar esta função
    public void Initialize(float duration, float destructionAnimDuration, bool viradoParaDireita)
    {
        // Define o sprite CORRETO (direita ou esquerda)
        if (viradoParaDireita)
        {
            spriteRenderer.sprite = spriteDireita;
        }
        else
        {
            spriteRenderer.sprite = spriteEsquerda;
        }

        // Inicia a contagem regressiva para a destruição
        StartCoroutine(DecoyLifecycle(duration, destructionAnimDuration));
    }

    private IEnumerator DecoyLifecycle(float duration, float destructionAnimDuration)
    {
        // 1. Espera a duração (ex: 5 segundos)
        yield return new WaitForSeconds(duration);

        // 2. Toca a animação de destruição
        if (animator != null)
        {
            // Puxa o gatilho "Destroy" que criamos no Animator
            animator.SetTrigger("Destroy");
        }

        // 3. Desativa o collider IMEDIATAMENTE
        // (Para que os monstros parem de persegui-lo)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 4. Destrói o objeto APÓS o tempo da animação
        Destroy(gameObject, destructionAnimDuration);
    }
}