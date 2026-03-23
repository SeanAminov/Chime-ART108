using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueBox : MonoBehaviour
{
    public Image textboxImage;
    public Image characterPortrait;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerName;
    public TextMeshProUGUI advanceHint;
    public float typeSpeed = 0.04f;
    private bool shownHintOnce;

    private bool isTyping;
    private bool skipTyping;
    private Coroutine typeRoutine;
    private Queue<DialogueLine> lineQueue = new Queue<DialogueLine>();
    private bool waitingForAdvance;

    // callback when all lines are done
    public System.Action onDialogueEnd;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        gameObject.SetActive(false);
    }

    void FreezePlayer()
    {
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            playerMovement.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }
    }

    void UnfreezePlayer()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                // skip to full text
                skipTyping = true;
            }
            else if (waitingForAdvance)
            {
                // show next line or close
                waitingForAdvance = false;
                ShowNextLine();
            }
        }
    }

    // show a single line
    public void Show(string text, Sprite portrait = null, string speaker = "")
    {
        lineQueue.Clear();
        lineQueue.Enqueue(new DialogueLine(text, portrait, speaker));
        gameObject.SetActive(true);
        FreezePlayer();
        ShowNextLine();
    }

    // queue up multiple lines, then show them one by one
    public void ShowSequence(List<DialogueLine> lines, System.Action onEnd = null)
    {
        lineQueue.Clear();
        foreach (var line in lines)
            lineQueue.Enqueue(line);
        onDialogueEnd = onEnd;
        gameObject.SetActive(true);
        FreezePlayer();
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (lineQueue.Count == 0)
        {
            Hide();
            onDialogueEnd?.Invoke();
            onDialogueEnd = null;
            return;
        }

        DialogueLine line = lineQueue.Dequeue();

        // set speaker name
        if (speakerName != null)
            speakerName.text = line.speaker;

        // set portrait
        if (characterPortrait != null)
        {
            if (line.portrait != null)
            {
                characterPortrait.sprite = line.portrait;
                characterPortrait.enabled = true;
            }
            else
            {
                characterPortrait.enabled = false;
            }
        }

        // hide hint while typing
        if (advanceHint != null)
            advanceHint.enabled = false;

        if (typeRoutine != null)
            StopCoroutine(typeRoutine);
        typeRoutine = StartCoroutine(TypeText(line.text));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        UnfreezePlayer();
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        skipTyping = false;
        dialogueText.text = "";

        foreach (char c in text)
        {
            if (skipTyping)
            {
                dialogueText.text = text;
                break;
            }
            dialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        waitingForAdvance = true;

        // show the hint only the first time
        if (advanceHint != null && !shownHintOnce)
        {
            advanceHint.enabled = true;
            shownHintOnce = true;
        }
    }
}

[System.Serializable]
public class DialogueLine
{
    public string text;
    public Sprite portrait;
    public string speaker;

    public DialogueLine(string text, Sprite portrait = null, string speaker = "")
    {
        this.text = text;
        this.portrait = portrait;
        this.speaker = speaker;
    }
}
