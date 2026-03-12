using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // stuff we can tweak in the inspector
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public float dropDuration = 0.25f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // check if we're on the ground using a small circle at the player's feet
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // left and right movement
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // flip the sprite when changing direction
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();

        // jumping - only works on the ground
        if (isGrounded && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // drop through platforms tagged "OneWayPlatform"
        if (isGrounded && (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            StartCoroutine(DropThroughPlatform());
        }
    }

    // briefly disables collision so the player falls through
    private System.Collections.IEnumerator DropThroughPlatform()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D[] platforms = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);

        foreach (Collider2D platform in platforms)
        {
            if (platform.CompareTag("OneWayPlatform"))
                Physics2D.IgnoreCollision(playerCollider, platform, true);
        }

        yield return new WaitForSeconds(dropDuration);

        // turn collision back on
        foreach (Collider2D platform in platforms)
        {
            if (platform != null && platform.CompareTag("OneWayPlatform"))
                Physics2D.IgnoreCollision(playerCollider, platform, false);
        }
    }

    // flips the sprite by inverting the x scale
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    // draws the ground check radius in the editor so we can see it
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
