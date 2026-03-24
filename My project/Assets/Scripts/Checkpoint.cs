using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int order = 1; // set in inspector: 1, 2, 3, etc.
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.gray;

    // tracks the highest checkpoint reached
    public static int currentOrder = 0;
    public static Vector3 lastCheckpoint;
    public static bool hasCheckpoint;

    private SpriteRenderer sr;
    private bool activated;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = inactiveColor;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        // only save if this checkpoint is further than the current one
        if (order <= currentOrder) return;

        activated = true;
        currentOrder = order;
        hasCheckpoint = true;
        lastCheckpoint = transform.position;

        if (sr != null)
            sr.color = activeColor;
    }
}
