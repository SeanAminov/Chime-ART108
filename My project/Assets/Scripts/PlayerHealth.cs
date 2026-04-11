using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public float invincibleTime = 1f;
    public float knockbackForce = 5f;

    [Header("Coins")]
    public int coinsForHP = 10;
    private int coinCount;

    [Header("Healing")]
    public float healHoldTime = 2f;
    public ParticleSystem healEffect;
    private float healTimer;
    private bool isHealing;
    private float healCooldown;

    [Header("References")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI coinText;
    public CameraFollow cameraFollow;
    public DialogueBox dialogueBox;
    public NaiFollow nai;

    private int currentHealth;
    private bool invincible;
    private float invincibleTimer;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Vector3 startPosition;
    private bool hasShownHealHint;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateUI();
    }

    void Update()
    {
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

        if (healCooldown > 0)
        {
            healCooldown -= Time.deltaTime;
            healTimer = 0f;
            isHealing = false;
        }
        else if (Input.GetKeyDown(KeyCode.R) && currentHealth < maxHealth && coinCount >= coinsForHP && !isHealing)
        {
            isHealing = true;
            healTimer = 0f;
            if (healEffect != null) healEffect.Play();
        }

        if (isHealing)
        {
            if (!Input.GetKey(KeyCode.R))
            {
                isHealing = false;
                healTimer = 0f;
                if (healEffect != null) healEffect.Stop();
            }
            else
            {
                healTimer += Time.deltaTime;

                if (cameraFollow != null)
                {
                    float intensity = Mathf.Lerp(0.01f, 0.06f, healTimer / healHoldTime);
                    cameraFollow.Shake(0.1f, intensity);
                }

                if (healTimer >= healHoldTime)
                {
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

        if (cameraFollow != null)
            cameraFollow.Shake(0.3f, 0.05f);
    }

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

        if (!hasShownHealHint && currentHealth > 0)
        {
            hasShownHealHint = true;
            ShowHealReminder();
        }

        if (currentHealth <= 0)
            Die();
    }

    public void UpdateUI()
    {
        if (healthText != null)
            healthText.text = string.Format("HP: {0} / {1}", currentHealth, maxHealth);
        if (coinText != null)
            coinText.text = string.Format("Coins: {0}", coinCount);
    }

    void ShowHealReminder()
    {
        if (dialogueBox == null || dialogueBox.gameObject.activeSelf) return;

        Sprite naiSprite = null;
        if (nai != null)
            naiSprite = nai.GetComponent<SpriteRenderer>()?.sprite;

        dialogueBox.Show("Hold R to focus and recover your health!", naiSprite, "Nai", false);
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
        invincible = true;
        invincibleTimer = invincibleTime;
        spriteRenderer.enabled = true;
        UpdateUI();
    }
}
