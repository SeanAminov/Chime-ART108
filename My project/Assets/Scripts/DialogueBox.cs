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
    public PlayerMovement playerMovement;

    private bool isTyping;
    private bool skipTyping;
    private Coroutine typeRoutine;
    private Queue<DialogueLine> lineQueue = new Queue<DialogueLine>();
    private bool waitingForAdvance;
    private bool isFrozen;

    public System.Action onDialogueEnd;

    private Queue<PendingDialogue> pendingQueue = new Queue<PendingDialogue>();

    public bool IsActive => gameObject.activeSelf;

    void Start()
    {
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
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Return))
        {
            if (isTyping)
            {
                skipTyping = true;
            }
            else if (waitingForAdvance)
            {
                waitingForAdvance = false;
                ShowNextLine();
            }
        }
    }

    public void Show(string text, Sprite portrait = null, string speaker = "", bool freezeMovement = true)
    {
        var lines = new List<DialogueLine> { new DialogueLine(text, portrait, speaker) };
        ShowSequence(lines, null, freezeMovement);
    }

    public void ShowSequence(List<DialogueLine> lines, System.Action onEnd = null, bool freezeMovement = true)
    {
        if (gameObject.activeSelf)
        {
            pendingQueue.Enqueue(new PendingDialogue(lines, onEnd, freezeMovement));
            return;
        }

        lineQueue.Clear();
        foreach (var line in lines)
            lineQueue.Enqueue(line);
        onDialogueEnd = onEnd;
        isFrozen = freezeMovement;
        gameObject.SetActive(true);
        if (freezeMovement) FreezePlayer();
        ShowNextLine();
    }

    void ShowNextLine()
    {
        if (lineQueue.Count == 0)
        {
            System.Action callback = onDialogueEnd;
            onDialogueEnd = null;

            gameObject.SetActive(false);
            if (isFrozen && pendingQueue.Count == 0)
                UnfreezePlayer();
            isFrozen = false;

            callback?.Invoke();

            if (pendingQueue.Count > 0)
            {
                PendingDialogue next = pendingQueue.Dequeue();
                ShowSequence(next.lines, next.onEnd, next.freeze);
            }

            return;
        }

        DialogueLine line = lineQueue.Dequeue();

        if (speakerName != null)
            speakerName.text = line.speaker;

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

        if (advanceHint != null)
            advanceHint.enabled = false;

        if (typeRoutine != null)
            StopCoroutine(typeRoutine);
        typeRoutine = StartCoroutine(TypeText(line.text));
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (isFrozen) UnfreezePlayer();
        isFrozen = false;
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

        if (advanceHint != null)
            advanceHint.enabled = true;
    }

    private class PendingDialogue
    {
        public List<DialogueLine> lines;
        public System.Action onEnd;
        public bool freeze;

        public PendingDialogue(List<DialogueLine> lines, System.Action onEnd, bool freeze)
        {
            this.lines = lines;
            this.onEnd = onEnd;
            this.freeze = freeze;
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
