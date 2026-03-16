using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // stuff we can tweak in the inspector
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public float dropDuration = 0.25f;

    private Rigidbody2D rb;
    [HideInInspector] public bool isGrounded;
    private bool facingRight = true;
    private float moveInput;
    private bool jumpPressed;
    private bool dropPressed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // read input every frame so we don't miss key presses
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            jumpPressed = true;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            dropPressed = true;

        // flip the sprite when changing direction
        if (moveInput > 0 && !facingRight)
            Flip();
        else if (moveInput < 0 && facingRight)
            Flip();
    }

    void FixedUpdate()
    {
        // move the player
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // jumping
        if (isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        jumpPressed = false;

        // drop through platforms
        if (isGrounded && dropPressed)
        {
            StartCoroutine(DropThroughPlatform());
        }
        dropPressed = false;
    }

    // ground detection using collision events
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }

    // briefly disables collision so the player falls through
    private System.Collections.IEnumerator DropThroughPlatform()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();

        // find nearby platforms using a small overlap check
        Collider2D[] platforms = Physics2D.OverlapBoxAll(
            transform.position, new Vector2(0.5f, 0.5f), 0f
        );

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
}
