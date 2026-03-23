using UnityEngine;

public class DialogueTest : MonoBehaviour
{
    public DialogueBox dialogueBox;

    void Update()
    {
        // press T to test the dialogue box
        if (Input.GetKeyDown(KeyCode.T))
        {
            dialogueBox.Show("Hello... I can feel the warmth of the sun, but I cannot see it. Will you guide me?");
        }
    }
}
