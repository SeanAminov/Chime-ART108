using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class KappaEndTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    public Dialogue successDialogue;
    public DialogueBox dialogueBox;

    [Header("Bridge")]
    public GameObject bridgeDown;
    public GameObject bridgeUp;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 0.5f;

    private bool hasCompleted;
    private bool playerInRange;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (dialogueBox != null && dialogueBox.gameObject.activeSelf)
                return;
            TryComplete();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void TryComplete()
    {
        if (hasCompleted) return;
        if (dialogueBox == null) return;
        if (WaterDroplet.collectedCount < WaterDroplet.totalRequired) return;

        hasCompleted = true;
        PlayDialogue(successDialogue, () => StartCoroutine(BridgeTransition()));
    }

    IEnumerator BridgeTransition()
    {
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadePanel.color = new Color(0, 0, 0, timer / fadeDuration);
                yield return null;
            }
            fadePanel.color = Color.black;
        }

        if (bridgeDown != null) bridgeDown.SetActive(false);
        if (bridgeUp != null) bridgeUp.SetActive(true);

        yield return new WaitForSeconds(0.3f);

        if (fadePanel != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadePanel.color = new Color(0, 0, 0, 1f - (timer / fadeDuration));
                yield return null;
            }
            fadePanel.gameObject.SetActive(false);
        }
    }

    void PlayDialogue(Dialogue dialogue, System.Action onEnd)
    {
        if (dialogue == null || dialogueBox == null) return;

        var lines = new List<DialogueLine>();
        foreach (var line in dialogue.lines)
        {
            string text = line.text.Replace("{name}", IntroSequence.playerName);
            lines.Add(new DialogueLine(text, line.portrait, line.speaker));
        }

        dialogueBox.ShowSequence(lines, onEnd, true);
    }
}
