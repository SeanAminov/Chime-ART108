using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueBox dialogueBox;
    public bool requireKeyPress = true;
    public bool triggerOnce = false;
    public bool freezePlayer = true;
    public bool showWaterUI = false;

    private bool playerInRange;
    private bool hasTriggered;

    void Update()
    {
        if (requireKeyPress && playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (dialogueBox != null && dialogueBox.gameObject.activeSelf)
                return;

            StartDialogue();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        if (!requireKeyPress)
            StartDialogue();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void StartDialogue()
    {
        if (dialogue == null || dialogueBox == null) return;
        if (triggerOnce && hasTriggered) return;

        hasTriggered = true;

        if (showWaterUI)
            WaterDroplet.ShowUI();

        var lines = new List<DialogueLine>();
        foreach (var line in dialogue.lines)
        {
            string text = line.text.Replace("{name}", IntroSequence.playerName);
            lines.Add(new DialogueLine(text, line.portrait, line.speaker));
        }

        dialogueBox.ShowSequence(lines, null, freezePlayer);
    }
}
