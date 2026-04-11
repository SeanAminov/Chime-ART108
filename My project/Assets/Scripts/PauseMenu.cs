using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Canvas canvas;

    private GameObject pausePanel;
    private bool isPaused;

    void Start()
    {
        CreatePausePanel();
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;

        if (playerMovement != null)
            playerMovement.enabled = false;
    }

    void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;

        if (playerMovement != null)
            playerMovement.enabled = true;
    }

    void CreatePausePanel()
    {
        if (canvas == null) return;

        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = pausePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image bg = pausePanel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.85f);

        CreateText(pausePanel.transform, "PauseTitle", "PAUSED",
            new Vector2(0.5f, 0.88f), 36, Color.white, FontStyles.Bold);

        string[] labels = new string[]
        {
            "WASD  -  Move",
            "W / Space  -  Jump",
            "Hold W / Space  -  Glide (while falling)",
            "Shift  -  Toggle Sprint",
            "E  -  Toggle See Ability",
            "R (Hold)  -  Heal (requires coins)",
            "F  -  Interact / Talk / Climb",
            "F / Enter  -  Advance Dialogue",
            "Esc  -  Pause / Resume"
        };

        float startY = 0.72f;
        float spacing = 0.055f;

        for (int i = 0; i < labels.Length; i++)
        {
            float y = startY - (i * spacing);
            CreateText(pausePanel.transform, "Key" + i, labels[i],
                new Vector2(0.5f, y), 20, new Color(0.85f, 0.85f, 0.85f), FontStyles.Normal);
        }

        CreateText(pausePanel.transform, "ResumeHint", "Press Esc to resume",
            new Vector2(0.5f, 0.12f), 18, new Color(0.6f, 0.6f, 0.6f), FontStyles.Italic);
    }

    void CreateText(Transform parent, string name, string content,
        Vector2 anchorPos, int fontSize, Color color, FontStyles style)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);

        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorPos;
        rect.anchorMax = anchorPos;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(600f, 40f);

        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.fontStyle = style;
        tmp.alignment = TextAlignmentOptions.Center;
    }
}
