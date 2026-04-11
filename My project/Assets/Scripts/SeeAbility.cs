using UnityEngine;
using UnityEngine.UI;

public class SeeAbility : MonoBehaviour
{
    [Header("Cooldown")]
    public float maxEnergy = 10f;
    public float rechargeRate = 1f;
    public float drainRate = 1f;
    public Slider energySlider;

    [Header("Hidden Platforms")]
    public GameObject[] hiddenPlatforms;

    [Header("Effects")]
    public ParticleSystem sparkles;
    public Color pulseColor = new Color(0.5f, 0.8f, 1f, 0.6f);
    public float pulseSpeed = 3f;
    public float maxPulseSize = 3f;

    private Collider2D playerCollider;
    private bool seeing;
    private float currentEnergy;

    private Renderer[] cachedRenderers;
    private Collider2D[] cachedColliders;

    private GameObject pulseRing;
    private SpriteRenderer pulseRenderer;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
        currentEnergy = maxEnergy;

        CacheHiddenComponents();
        ShowHiddenPlatforms(false);

        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = maxEnergy;
        }

        pulseRing = new GameObject("PulseRing");
        pulseRing.transform.SetParent(transform);
        pulseRing.transform.localPosition = Vector3.zero;

        pulseRenderer = pulseRing.AddComponent<SpriteRenderer>();
        pulseRenderer.sprite = CreateCircleSprite();
        pulseRenderer.color = pulseColor;
        pulseRenderer.sortingOrder = 5;
        pulseRing.SetActive(false);
    }

    void CacheHiddenComponents()
    {
        if (hiddenPlatforms == null)
        {
            cachedRenderers = new Renderer[0];
            cachedColliders = new Collider2D[0];
            return;
        }

        int rCount = 0, cCount = 0;
        foreach (var p in hiddenPlatforms)
        {
            if (p == null) continue;
            rCount += p.GetComponentsInChildren<Renderer>(true).Length;
            cCount += p.GetComponentsInChildren<Collider2D>(true).Length;
        }

        cachedRenderers = new Renderer[rCount];
        cachedColliders = new Collider2D[cCount];

        int ri = 0, ci = 0;
        foreach (var p in hiddenPlatforms)
        {
            if (p == null) continue;
            foreach (var r in p.GetComponentsInChildren<Renderer>(true))
                cachedRenderers[ri++] = r;
            foreach (var c in p.GetComponentsInChildren<Collider2D>(true))
                cachedColliders[ci++] = c;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && (seeing || currentEnergy >= maxEnergy * 0.1f))
            seeing = !seeing;

        if (seeing && !pulseRing.activeSelf)
        {
            ShowHiddenPlatforms(true);
            Spike.SetAllHiddenVisible(true);
            pulseRing.SetActive(true);
            if (sparkles != null) sparkles.Play();
        }
        else if (!seeing && pulseRing.activeSelf)
        {
            ShowHiddenPlatforms(false);
            Spike.SetAllHiddenVisible(false);
            pulseRing.SetActive(false);
            if (sparkles != null) sparkles.Stop();
        }

        if (seeing)
        {
            currentEnergy -= drainRate * Time.deltaTime;
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
                seeing = false;
            }
        }
        else
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            if (currentEnergy >= maxEnergy)
                currentEnergy = maxEnergy;
        }

        if (energySlider != null)
            energySlider.value = currentEnergy;

        if (seeing && pulseRing.activeSelf)
        {
            float scale = Mathf.PingPong(Time.time * pulseSpeed, maxPulseSize) + 0.5f;
            pulseRing.transform.localScale = new Vector3(scale, scale, 1f);

            float alpha = Mathf.Lerp(0.5f, 0f, scale / maxPulseSize);
            Color c = pulseColor;
            c.a = alpha;
            pulseRenderer.color = c;
        }
    }

    void ShowHiddenPlatforms(bool visible)
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

    Sprite CreateCircleSprite()
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size);
        tex.filterMode = FilterMode.Bilinear;

        float center = size / 2f;
        float outerRadius = size / 2f;
        float innerRadius = outerRadius - 4f;
        float outerSq = outerRadius * outerRadius;
        float innerSq = innerRadius * innerRadius;

        for (int x = 0; x < size; x++)
        {
            float dx = x - center;
            for (int y = 0; y < size; y++)
            {
                float dy = y - center;
                float distSq = dx * dx + dy * dy;
                if (distSq < outerSq && distSq > innerSq)
                    tex.SetPixel(x, y, Color.white);
                else
                    tex.SetPixel(x, y, Color.clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
    }
}
