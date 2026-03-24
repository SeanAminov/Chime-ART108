using UnityEngine;
using System.Collections.Generic;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public DialogueBox dialogueBox;
    public bool requireKeyPress = true;  // press E to talk
    public bool triggerOnce = false;     // only plays once
    public bool freezePlayer = true;    // freeze movement during dialogue

    private bool playerInRange;
    private bool hasTriggered;

    void Update()
    {
        // only start if player is nearby, presses E, and no dialogue is already open
        if (requireKeyPress && playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueBox != null && dialogueBox.gameObject.activeSelf)
                return; // dialogue already playing, let DialogueBox handle the E press

            StartDialogue();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        // auto-trigger zones (no key press needed)
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

        // convert to DialogueLine list
        var lines = new List<DialogueLine>();
        foreach (var line in dialogue.lines)
        {
            // swap in the player's name if text has {name}
            string text = line.text.Replace("{name}", IntroSequence.playerName);
            lines.Add(new DialogueLine(text, line.portrait, line.speaker));
        }

        dialogueBox.ShowSequence(lines, null, freezePlayer);
    }
}
