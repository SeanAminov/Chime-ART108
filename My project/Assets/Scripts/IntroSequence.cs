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

        if (namePrompt != null)
            namePrompt.SetActive(false);

        if (dialogueBox != null)
            dialogueBox.transform.SetAsLastSibling();

        StartCoroutine(RunIntro());
    }

    IEnumerator RunIntro()
    {
        yield return new WaitForSeconds(1f);

        // nai's first line over black (false = don't let dialoguebox touch freeze, we handle it)
        bool waiting = true;
        dialogueBox.ShowSequence(new List<DialogueLine>
        {
            new DialogueLine("Hey, wake up!!", naiPortrait, "Nai"),
        }, () => waiting = false, false);
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

        // nai calls them by name
        waiting = true;
        dialogueBox.ShowSequence(new List<DialogueLine>
        {
            new DialogueLine(playerName + "!!! Wake up!!", naiPortrait, "Nai"),
        }, () => waiting = false, false);
        while (waiting) yield return null;

        // fade from black (eyes opening)
        if (fadePanel != null)
            yield return StartCoroutine(FadeFromBlack());

        // post-wakeup dialogue (don't freeze via dialoguebox, we handle it ourselves)
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
                "...it's WASD or arrow keys to move, " + playerName + ".",
                naiPortrait, "Nai"
            ),
        }, () => waiting = false, false);
        while (waiting) yield return null;

        // NOW unfreeze - all intro dialogue is done
        if (playerMovement != null)
            playerMovement.enabled = true;

        // show health and coin UI
        GameObject healthObj = GameObject.Find("HealthText");
        if (healthObj != null)
            healthObj.SetActive(true);
        GameObject coinObj = GameObject.Find("CoinText");
        if (coinObj != null)
            coinObj.SetActive(true);

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
