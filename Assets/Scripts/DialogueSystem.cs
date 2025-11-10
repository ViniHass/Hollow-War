using UnityEngine;

public enum STATE {
    DISABLED,
    WAITING,
    TYPING
}

public class DialogueSystem : MonoBehaviour {

    public DialogueData dialogueData;

    int currentIndex;
    bool finished;

    TypeTextAnimation typeText;
    DialogueUI dialogueUI;

    STATE state = STATE.DISABLED;

    void Awake() {
        typeText = FindObjectOfType<TypeTextAnimation>();
        dialogueUI = FindObjectOfType<DialogueUI>();

        if (typeText != null)
            typeText.TypeFinished += OnTypeFinished;
        
        if (typeText == null)
            Debug.LogError("DialogueSystem: TypeTextAnimation não encontrado na cena!");
        if (dialogueUI == null)
            Debug.LogError("DialogueSystem: DialogueUI não encontrado na cena!");
    }

    void OnDestroy() {
        if (typeText != null)
            typeText.TypeFinished -= OnTypeFinished;
            
    }

    public void SetDialogue(DialogueData newData) {
        dialogueData = newData;
        currentIndex = 0;
        finished = false;
    }

    public void StartDialogue() {
        if (dialogueData == null || dialogueData.talkScript.Count == 0) {
            Debug.LogWarning("DialogueSystem: DialogueData vazio ou não atribuído.");
            return;
        }

        dialogueUI.Enable();
        state = STATE.WAITING;
        Next();
    }

    public void Next() {
        if (dialogueData == null || dialogueData.talkScript.Count == 0) return;
        if (finished) return;

        var currentDialogue = dialogueData.talkScript[currentIndex];

        dialogueUI.SetName(currentDialogue.name);
        
        // O TypeTextAnimation vai cuidar do texto
        typeText.fullText = currentDialogue.text;
        typeText.StartTyping();

        currentIndex++;
        finished = currentIndex >= dialogueData.talkScript.Count;

        state = STATE.TYPING;
    }

    void OnTypeFinished() {
        state = STATE.WAITING;
    }

    void Update() {
        if (state == STATE.DISABLED) return;

        if (Input.GetKeyDown(KeyCode.P)) {
            if (state == STATE.TYPING) {
                typeText.Skip();
            } else if (state == STATE.WAITING) {
                if (!finished) {
                    Next();
                } else {
                    dialogueUI.Disable();
                    state = STATE.DISABLED;
                    currentIndex = 0;
                    finished = false;
                }
            }
        }
    }

    public bool IsActive() => state != STATE.DISABLED;
}