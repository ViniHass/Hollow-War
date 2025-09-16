using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TreeTransparency : MonoBehaviour
{
    [Tooltip("Se vazio, pega todos os SpriteRenderers filhos do root.")]
    [SerializeField] private SpriteRenderer[] renderers;
    [Range(0f, 1f)] [SerializeField] private float fadedAlpha = 0.35f;
    [SerializeField] private float fadeDuration = 0.15f;
    [SerializeField] private string playerTag = "Player";

    private Coroutine fadeCoroutine;

    private void Reset()
    {
        // deixa o collider como trigger por padrão
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (renderers == null || renderers.Length == 0)
        {
            // tenta pegar todos os SpriteRenderers filhos do root (Tree)
            var root = transform.parent != null ? transform.parent : transform;
            renderers = root.GetComponentsInChildren<SpriteRenderer>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        StartFadeTo(fadedAlpha);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        StartFadeTo(1f);
    }

    private void StartFadeTo(float targetAlpha)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(targetAlpha));
    }

    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float t = 0f;
        float duration = Mathf.Max(0.0001f, fadeDuration);
        float[] starts = new float[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            starts[i] = renderers[i].color.a;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;
            for (int i = 0; i < renderers.Length; i++)
            {
                var c = renderers[i].color;
                c.a = Mathf.Lerp(starts[i], targetAlpha, lerp);
                renderers[i].color = c;
            }
            yield return null;
        }

        // garante valor final
        for (int i = 0; i < renderers.Length; i++)
        {
            var c = renderers[i].color;
            c.a = targetAlpha;
            renderers[i].color = c;
        }

        fadeCoroutine = null;
    }
}
