using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour {

    public GameObject panel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI bodyText;

    void Awake() {
        Disable();
        if (panel == null)
            Debug.LogError("DialogueUI: Panel não atribuído!");
        if (nameText == null)
            Debug.LogWarning("DialogueUI: NameText não atribuído!");
        if (bodyText == null)
            Debug.LogError("DialogueUI: BodyText não atribuído!");
    }

    public void Enable() {
        if (panel) panel.SetActive(true);
    }

    public void Disable() {
        if (panel) panel.SetActive(false);
       
    }

    public void SetName(string n) {
        if (nameText) nameText.text = n;
    }

    public void SetBody(string t) {
        if (bodyText) bodyText.text = t;
    }
}
