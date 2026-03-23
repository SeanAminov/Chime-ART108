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
    public Dialogue introDialogue;  // optional: use a Dialogue asset instead of hardcoded

    // store the player's chosen name
    public static string playerName = "Kid";

    private PlayerMovement playerMovement;

    void OnEnable()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();

        // freeze player during intro
        if (playerMovement != null)
            playerMovement.enabled = false;

        // black screen
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            fadePanel.color = Color.black;
            fadePanel.transform.SetAsFirstSibling();
        }

        // hide name prompt
        if (namePrompt != null)
            namePrompt.SetActive(false);

        // dialogue on top of fade
        if (dialogueBox != null)
            dialogueBox.transform.SetAsLastSibling();

        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        yield return new WaitForSeconds(1f);

        // nai's first line over black screen
        var preLines = new List<DialogueLine>
        {
            new DialogueLine("Hey, wake up!!", naiPortrait, "Nai"),
        };

        bool waiting = true;
        dialogueBox.ShowSequence(preLines, () => waiting = false);
        while (waiting) yield return null;

        // ask for the player's name
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

        // nai calls the player by name
        waiting = true;
        dialogueBox.ShowSequence(new List<DialogueLine>
        {
            new DialogueLine(playerName + "!!! Wake up!!", naiPortrait, "Nai"),
        }, () => waiting = false);
        while (waiting) yield return null;

        // fade from black to game (eyes opening)
        if (fadePanel != null)
            yield return StartCoroutine(FadeFromBlack());

        // first line after waking up
        waiting = true;
        dialogueBox.ShowSequence(new List<DialogueLine>
        {
            new DialogueLine(
                "There you are!! Are you alright? You fell all the way to the bottom of the town, you know.",
                naiPortrait, "Nai"
            ),
        }, () => waiting = false);
        while (waiting) yield return null;

        // unfreeze player
        if (playerMovement != null)
            playerMovement.enabled = true;

        // show health UI now that gameplay starts
        GameObject healthObj = GameObject.Find("HealthText");
        if (healthObj != null)
            healthObj.SetActive(true);
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
