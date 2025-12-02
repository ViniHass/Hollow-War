using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TypeTextAnimation : MonoBehaviour {

    public event Action TypeFinished;
    
    [Header("Configurações")]
    public float typeDelay = 0.05f;
    public TextMeshProUGUI textObject;

    [HideInInspector]
    public string fullText;

    Coroutine coroutine;
    bool isTyping = false;

    void Awake() {
        if (textObject == null)
            Debug.LogError("TypeTextAnimation: TextMeshProUGUI não atribuído!");
    }

    public void StartTyping() {
        if (textObject == null) {
            Debug.LogError("TypeTextAnimation: TextMeshProUGUI não atribuído!");
            return;
        }

        if (string.IsNullOrEmpty(fullText)) {
            Debug.LogWarning("TypeTextAnimation: fullText está vazio!");
            return;
        }

        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText() {
        isTyping = true;
        
        textObject.text = fullText;
        textObject.maxVisibleCharacters = 0;

        textObject.ForceMeshUpdate();
        int totalChars = textObject.textInfo.characterCount;

        for (int i = 0; i <= totalChars; i++) {
            textObject.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typeDelay);
        }

        isTyping = false;
        TypeFinished?.Invoke();
    }

    public void Skip() {
        if (textObject == null || !isTyping) return;

        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        textObject.ForceMeshUpdate();
        textObject.maxVisibleCharacters = textObject.textInfo.characterCount;
        
        isTyping = false;
        TypeFinished?.Invoke();
    }

    void OnDisable() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        isTyping = false;
    }

    public void Clear() {
    if (textObject != null)
      textObject.text = "";
    
            
    }
}
