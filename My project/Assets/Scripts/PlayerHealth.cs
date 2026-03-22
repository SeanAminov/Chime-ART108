using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public float invincibleTime = 1f;
    public float knockbackForce = 5f;

    private int currentHealth;
    private bool invincible;
    private float invincibleTimer;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    // UI reference
    private Text healthText;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // find the health text in the scene
        GameObject healthObj = GameObject.Find("HealthText");
        if (healthObj != null)
            healthText = healthObj.GetComponent<Text>();

        UpdateUI();
    }

    void Update()
    {
        // flash the sprite while invincible
        if (invincible)
        {
            invincibleTimer -= Time.deltaTime;

            // blink effect
            spriteRenderer.enabled = Mathf.Sin(Time.time * 20f) > 0;

            if (invincibleTimer <= 0)
            {
                invincible = false;
                spriteRenderer.enabled = true;
            }
        }
    }

    public void TakeDamage(Vector2 hitDirection)
    {
        if (invincible) return;

        currentHealth--;
        UpdateUI();

        // knockback away from the hit
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode2D.Impulse);

        // start invincibility frames
        invincible = true;
        invincibleTimer = invincibleTime;

        if (currentHealth <= 0)
            Die();
    }

    void UpdateUI()
    {
        if (healthText != null)
            healthText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    void Die()
    {
        // for now just respawn at start
        transform.position = new Vector3(-0.52f, -2.5f, 0f);
        currentHealth = maxHealth;
        invincible = false;
        spriteRenderer.enabled = true;
        UpdateUI();
    }
}
