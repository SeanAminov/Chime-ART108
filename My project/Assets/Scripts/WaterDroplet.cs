using UnityEngine;
using TMPro;

public class WaterDroplet : MonoBehaviour
{
    public static int collectedCount = 0;
    public static int totalRequired = 6;

    public static TextMeshProUGUI waterText;
    private static bool uiVisible = false;

    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        collectedCount++;
        UpdateUI();
        Destroy(gameObject);
    }

    public static void ShowUI()
    {
        uiVisible = true;
        if (waterText != null)
            waterText.gameObject.SetActive(true);
        UpdateUI();
    }

    public static void ResetCount()
    {
        collectedCount = 0;
        uiVisible = false;
        UpdateUI();
    }

    public static void UpdateUI()
    {
        if (waterText != null)
            waterText.text = string.Format("Water: {0} / {1}", collectedCount, totalRequired);
    }
}
