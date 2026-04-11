using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EndGameTrigger : MonoBehaviour
{
    [Header("Nai")]
    public NaiFollow nai;
    public Vector2 naiStopPosition = new Vector2(382f, -1.69f);

    [Header("Farewell Dialogue")]
    public Dialogue farewellDialogue;
    public DialogueBox dialogueBox;

    [Header("Fade & Win Screen")]
    public Image fadePanel;
    public float fadeDuration = 1f;
    public GameObject winScreen;

    private bool triggered;

    void Start()
    {
        if (winScreen != null)
            winScreen.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (nai != null)
        {
            nai.enabled = false;
            nai.transform.position = new Vector3(naiStopPosition.x, naiStopPosition.y, nai.transform.position.z);
        }

        PlayFarewell();
    }

    void PlayFarewell()
    {
        if (farewellDialogue == null || dialogueBox == null)
        {
            StartCoroutine(FadeToWin());
            return;
        }

        var lines = new List<DialogueLine>();
        foreach (var line in farewellDialogue.lines)
        {
            string text = line.text.Replace("{name}", IntroSequence.playerName);
            lines.Add(new DialogueLine(text, line.portrait, line.speaker));
        }

        dialogueBox.ShowSequence(lines, () => StartCoroutine(FadeToWin()), true);
    }

    IEnumerator FadeToWin()
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

        yield return new WaitForSeconds(0.5f);

        if (winScreen != null)
            winScreen.SetActive(true);
    }
}
