using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public float invincibleTime = 1f;
    public float knockbackForce = 5f;

    [Header("Coins")]
    public int coinsForHP = 10; // collect this many coins to get 1 HP
    private int coinCount;

    [Header("Healing")]
    public float healHoldTime = 2f; // hold F for this long to heal
    public ParticleSystem healEffect; // assign in inspector or auto-created
    private float healTimer;
    private bool isHealing;
    private float healCooldown;

    private int currentHealth;
    private bool invincible;
    private float invincibleTimer;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private bool hasShownHealHint;

    // UI - supports both TMP and legacy Text
    private TextMeshProUGUI healthText;
    private TextMeshProUGUI coinText;
    private Text healthTextLegacy;
    private Text coinTextLegacy;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // find UI elements including inactive ones
    void FindUI()
    {
        if (healthText != null && coinText != null) return;
        if (healthTextLegacy != null && coinTextLegacy != null) return;

        // try TMP first
        TextMeshProUGUI[] allTMP = FindObjectsOfType<TextMeshProUGUI>(true);
        foreach (var tmp in allTMP)
        {
            if (tmp.gameObject.name == "HealthText")
                healthText = tmp;
            else if (tmp.gameObject.name == "CoinText")
                coinText = tmp;
        }

        // also try legacy Text
        Text[] allLegacy = FindObjectsOfType<Text>(true);
        foreach (var t in allLegacy)
        {
            if (t.gameObject.name == "HealthText" && healthText == null)
                healthTextLegacy = t;
            else if (t.gameObject.name == "CoinText" && coinText == null)
                coinTextLegacy = t;
        }

        bool found = healthText != null || coinText != null ||
                     healthTextLegacy != null || coinTextLegacy != null;
        if (found)
            UpdateUI();
    }

    void Update()
    {
        // keep trying to find UI until we have it
        FindUI();

        // invincibility blink
        if (invincible)
        {
            invincibleTimer -= Time.deltaTime;
            spriteRenderer.enabled = Mathf.Sin(Time.time * 20f) > 0;

            if (invincibleTimer <= 0)
            {
                invincible = false;
                spriteRenderer.enabled = true;
            }
        }

        // cooldown after a heal so you can't just hold F forever
        if (healCooldown > 0)
        {
            healCooldown -= Time.deltaTime;
            healTimer = 0f;
            isHealing = false;
        }
        // hold F to heal (hollow knight style: hold, heal once, forced out)
        else if (Input.GetKeyDown(KeyCode.F) && currentHealth < maxHealth && coinCount >= coinsForHP && !isHealing)
        {
            // start the heal charge
            isHealing = true;
            healTimer = 0f;
            if (healEffect != null) healEffect.Play();
        }

        if (isHealing)
        {
            if (!Input.GetKey(KeyCode.F))
            {
                // let go too early, cancel
                isHealing = false;
                healTimer = 0f;
                if (healEffect != null) healEffect.Stop();
            }
            else
            {
                healTimer += Time.deltaTime;

                // subtle shake that builds as the heal charges
                CameraFollow cam = FindObjectOfType<CameraFollow>();
                if (cam != null)
                {
                    float intensity = Mathf.Lerp(0.01f, 0.06f, healTimer / healHoldTime);
                    cam.Shake(0.1f, intensity);
                }

                if (healTimer >= healHoldTime)
                {
                    // heal completes, spend coins, force out
                    coinCount -= coinsForHP;
                    Heal(1);
                    isHealing = false;
                    healTimer = 0f;
                    healCooldown = 0.5f;
                    if (healEffect != null) healEffect.Stop();
                }
            }
        }
    }

    public void CollectCoin()
    {
        coinCount++;
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();

        // little camera shake when healing
        CameraFollow cam = FindObjectOfType<CameraFollow>();
        if (cam != null)
            cam.Shake(0.3f, 0.05f);
    }

    // called by rokurokubi to gift health
    public void GiftHealth()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }

    public void TakeDamage(Vector2 hitDirection)
    {
        if (invincible) return;

        currentHealth--;
        UpdateUI();

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);

        invincible = true;
        invincibleTimer = invincibleTime;

        // nai reminds you about healing after taking a hit
        if (!hasShownHealHint && currentHealth > 0)
        {
            hasShownHealHint = true;
            ShowHealReminder();
        }

        if (currentHealth <= 0)
            Die();
    }

    void UpdateUI()
    {
        string hpStr = "HP: " + currentHealth + " / " + maxHealth;
        string coinStr = "Coins: " + coinCount;

        if (healthText != null) healthText.text = hpStr;
        if (healthTextLegacy != null) healthTextLegacy.text = hpStr;
        if (coinText != null) coinText.text = coinStr;
        if (coinTextLegacy != null) coinTextLegacy.text = coinStr;
    }

    void ShowHealReminder()
    {
        DialogueBox db = FindObjectOfType<DialogueBox>(true);
        if (db == null || db.gameObject.activeSelf) return;

        // find nai's portrait for the dialogue
        Sprite naiSprite = null;
        NaiFollow nai = FindObjectOfType<NaiFollow>();
        if (nai != null)
            naiSprite = nai.GetComponent<SpriteRenderer>()?.sprite;

        db.Show("Hold F to focus and recover your health!", naiSprite, "Nai", false);
    }

    public void InstantKill()
    {
        Die();
    }

    void Die()
    {
        if (Checkpoint.hasCheckpoint)
            transform.position = Checkpoint.lastCheckpoint;
        else
            transform.position = startPosition;

        rb.linearVelocity = Vector2.zero;
        currentHealth = maxHealth;
        invincible = false;
        spriteRenderer.enabled = true;
        UpdateUI();
    }
}
