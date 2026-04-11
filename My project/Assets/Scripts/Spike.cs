using UnityEngine;

public class Spike : MonoBehaviour
{
    public bool hiddenUntilSeen = false;

    private static readonly System.Collections.Generic.List<Spike> hiddenInstances =
        new System.Collections.Generic.List<Spike>();

    private Renderer[] cachedRenderers;
    private Collider2D[] cachedColliders;

    void Awake()
    {
        if (!hiddenUntilSeen) return;

        cachedRenderers = GetComponentsInChildren<Renderer>(true);
        cachedColliders = GetComponentsInChildren<Collider2D>(true);

        hiddenInstances.Add(this);
        SetVisible(false);
    }

    void OnDestroy()
    {
        hiddenInstances.Remove(this);
    }

    public void SetVisible(bool visible)
    {
        if (cachedRenderers != null)
        {
            for (int i = 0; i < cachedRenderers.Length; i++)
                if (cachedRenderers[i] != null) cachedRenderers[i].enabled = visible;
        }
        if (cachedColliders != null)
        {
            for (int i = 0; i < cachedColliders.Length; i++)
                if (cachedColliders[i] != null) cachedColliders[i].enabled = visible;
        }
    }

    public static void SetAllHiddenVisible(bool visible)
    {
        for (int i = 0; i < hiddenInstances.Count; i++)
            if (hiddenInstances[i] != null) hiddenInstances[i].SetVisible(visible);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            Vector2 knockDir = (col.transform.position - transform.position).normalized;
            knockDir.y = 1f;
            health.TakeDamage(knockDir);
        }
    }
}
