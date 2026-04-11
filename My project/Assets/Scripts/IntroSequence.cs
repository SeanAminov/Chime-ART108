using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class IntroSequence : MonoBehaviour
{
    public DialogueBox dialogueBox;
    public Image fadePanel;
    public TMP_InputField nameInput;
    public GameObject namePrompt;
    public Sprite naiPortrait;
    public float fadeDuration = 2f;
    public PlayerMovement playerMovement;

    public static string playerName = "Kid";

    void OnEnable()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = Color.black;
            fadePanel.transform.SetAsFirstSibling();
        }

        if (namePrompt != null)
            namePrompt.SetActive(false);

        if (dialogueBox != null)
            dialogueBox.transform.SetAsLastSibling();

        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        yield return new WaitForSeconds(1f);

        bool waiting = true;
        dialogueBox.ShowSequence(new List<DialogueLine>
        {
            new DialogueLine("Hey, wake up!!", naiPortrait, "Nai"),
        }, () => waiting = false, false);
        while (waiting) yield return null;

        if (namePrompt != null)
        {
            namePrompt.SetActive(true);
            namePrompt.transform.SetAsLastSibling();
            nameInput.text = "";
            nameInput.ActivateInputField();

            bool nameConfirmed = false;
            nameInput.onSubmit.AddListener((name) => {
                if (!string.IsNullOrWhiteSpace(name))
                {
                    playerName = name.Trim();
                    nameConfirmed = true;
                }
            });

            while (!nameConfirmed) yield return null;
            namePrompt.SetActive(false);
        }

        waiting = true;
        dialogueBox.ShowSequence(new List<DialogueLine>
        {
            new DialogueLine(playerName + "!!! Wake up!!", naiPortrait, "Nai"),
        }, () => waiting = false, false);
        while (waiting) yield return null;

        if (fadePanel != null)
            yield return StartCoroutine(FadeFromBlack());

        waiting = true;
        dialogueBox.ShowSequence(new List<DialogueLine>
        {
            new DialogueLine(
                "There you are!! Are you alright? You fell all the way to the bottom of the town, you know.",
                naiPortrait, "Nai"
            ),
            new DialogueLine(
                "Well, you look alright... come on!! We have to get back up now!!",
                naiPortrait, "Nai"
            ),
            new DialogueLine(
                "...it's WASD to move and Space to jump, " + playerName + ".",
                naiPortrait, "Nai"
            ),
        }, () => waiting = false, false);
        while (waiting) yield return null;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    IEnumerator FadeFromBlack()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / fadeDuration);
            fadePanel.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadePanel.gameObject.SetActive(false);
    }
}
