using UnityEngine;

public class SeeAbility : MonoBehaviour
{
    public LayerMask hiddenLayer;

    [Header("Effects")]
    public ParticleSystem sparkles; // assign in inspector
    public Color pulseColor = new Color(0.5f, 0.8f, 1f, 0.6f);
    public float pulseSpeed = 3f;
    public float maxPulseSize = 3f;

    private Collider2D playerCollider;
    private bool seeing;

    // pulse ring created at runtime (just a simple circle)
    private GameObject pulseRing;
    private SpriteRenderer pulseRenderer;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();

        // hide all hidden platforms at the start of the game
        ShowHiddenPlatforms(false);

        // create a simple pulse ring
        pulseRing = new GameObject("PulseRing");
        pulseRing.transform.SetParent(transform);
        pulseRing.transform.localPosition = Vector3.zero;

        pulseRenderer = pulseRing.AddComponent<SpriteRenderer>();
        pulseRenderer.sprite = CreateCircleSprite();
        pulseRenderer.color = pulseColor;
        pulseRenderer.sortingOrder = 5;
        pulseRing.SetActive(false);
    }

    void Update()
    {
        bool spaceHeld = Input.GetKey(KeyCode.Space);

        if (spaceHeld && !seeing)
        {
            seeing = true;
            ShowHiddenPlatforms(true);
            pulseRing.SetActive(true);
            if (sparkles != null) sparkles.Play();
        }
        else if (!spaceHeld && seeing)
        {
            seeing = false;
            ShowHiddenPlatforms(false);
            pulseRing.SetActive(false);
            if (sparkles != null) sparkles.Stop();
        }

        // animate the pulse expanding outward
        if (seeing && pulseRing.activeSelf)
        {
            float scale = Mathf.PingPong(Time.time * pulseSpeed, maxPulseSize) + 0.5f;
            pulseRing.transform.localScale = new Vector3(scale, scale, 1f);

            float alpha = Mathf.Lerp(0.5f, 0f, scale / maxPulseSize);
            pulseRenderer.color = new Color(pulseColor.r, pulseColor.g, pulseColor.b, alpha);
        }
    }

    void ShowHiddenPlatforms(bool visible)
    {
        GameObject[] hiddenPlatforms = GameObject.FindGameObjectsWithTag("HiddenPlatform");

        foreach (GameObject platform in hiddenPlatforms)
        {
            SpriteRenderer sr = platform.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = visible;

            Collider2D col = platform.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = visible;
        }
    }

    Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;

        float center = size / 2f;
        float outerRadius = size / 2f;
        float innerRadius = outerRadius - 4f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                if (dist < outerRadius && dist > innerRadius)
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
