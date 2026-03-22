using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // movement
    public float moveSpeed = 5f;
    public float sprintMultiplier = 1.6f;
    public float dropDuration = 0.25f;

    // jumping
    public float jumpForce = 12f;
    public float maxJumpTime = 0.25f;
    public float glideGravity = 0.5f;

    private Rigidbody2D rb;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public float facingDirection = 1f;
    private float moveInput;
    private bool jumpPressed;
    private bool jumpHeld;
    private bool dropPressed;
    private bool isJumping;
    private float jumpTimer;
    private float normalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        normalGravity = rb.gravityScale;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            jumpPressed = true;

        // track if jump key is being held
        jumpHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            dropPressed = true;

    }

    void FixedUpdate()
    {
        // sprint if holding shift
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            speed *= sprintMultiplier;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);

        // track which way the player is facing
        if (moveInput != 0)
            facingDirection = Mathf.Sign(moveInput);

        // start a jump
        if (isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpTimer = 0f;
        }
        jumpPressed = false;

        // hold jump to go higher
        if (isJumping && jumpHeld)
        {
            jumpTimer += Time.fixedDeltaTime;
            if (jumpTimer < maxJumpTime)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
            else
            {
                isJumping = false;
            }
        }

        // let go of jump early = stop rising
        if (isJumping && !jumpHeld)
            isJumping = false;

        // glide: hold W while falling to slow descent
        if (!isGrounded && !isJumping && jumpHeld && rb.linearVelocity.y < 0)
        {
            rb.gravityScale = glideGravity;
        }
        else
        {
            rb.gravityScale = normalGravity;
        }

        // drop through platforms
        if (isGrounded && dropPressed)
            StartCoroutine(DropThroughPlatform());
        dropPressed = false;
    }

    bool IsPlatform(GameObject obj)
    {
        return obj.CompareTag("Ground") ||
               obj.CompareTag("OneWayPlatform") ||
               obj.CompareTag("HiddenPlatform");
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (IsPlatform(col.gameObject))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (IsPlatform(col.gameObject))
            isGrounded = false;
    }

    private System.Collections.IEnumerator DropThroughPlatform()
    {
        Collider2D playerCollider = GetComponent<Collider2D>();

        Collider2D[] platforms = Physics2D.OverlapBoxAll(
            transform.position, new Vector2(0.5f, 0.5f), 0f
        );

        foreach (Collider2D platform in platforms)
        {
            if (platform.CompareTag("OneWayPlatform"))
                Physics2D.IgnoreCollision(playerCollider, platform, true);
        }

        yield return new WaitForSeconds(dropDuration);

        foreach (Collider2D platform in platforms)
        {
            if (platform != null && platform.CompareTag("OneWayPlatform"))
                Physics2D.IgnoreCollision(playerCollider, platform, false);
        }
    }

}
