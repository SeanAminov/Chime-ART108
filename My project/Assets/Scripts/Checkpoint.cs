using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int order = 1;
    [Range(0f, 1f)]
    public float inactiveAlpha = 0.4f;

    public static int currentOrder;
    public static Vector3 lastCheckpoint;
    public static bool hasCheckpoint;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        currentOrder = 0;
        lastCheckpoint = Vector3.zero;
        hasCheckpoint = false;
    }

    private SpriteRenderer sr;
    private bool activated;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = inactiveAlpha;
            sr.color = c;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        if (order <= currentOrder) return;

        activated = true;
        currentOrder = order;
        hasCheckpoint = true;
        lastCheckpoint = transform.position;

        if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }
}
