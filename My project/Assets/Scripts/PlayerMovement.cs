using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float sprintSpeed = 10f;
    public float acceleration = 35f;
    public float deceleration = 45f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public float minJumpForce = 5f;
    public float maxJumpTime = 0.3f;
    public float glideGravity = 0.4f;

    [Header("Drop Through")]
    public float dropThroughTime = 0.35f;

    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private readonly System.Collections.Generic.List<Collider2D> touchedOneWays =
        new System.Collections.Generic.List<Collider2D>();

    private int platformContacts;
    public bool isGrounded => platformContacts > 0;

    [HideInInspector] public float facingDirection = 1f;
    [HideInInspector] public bool isSprinting;

    private float moveInput;
    private bool jumpPressed;
    private bool jumpReleased;
    private bool jumpHeld;
    private bool isJumping;
    private float jumpTimer;
    private float normalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        normalGravity = rb.gravityScale;
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space))
            jumpReleased = true;

        jumpHeld = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            isSprinting = !isSprinting;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            TryDropThrough();
    }

    void FixedUpdate()
    {
        float targetSpeed = moveInput * (isSprinting ? sprintSpeed : moveSpeed);
        float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float newX = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, rate * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        if (moveInput != 0)
            facingDirection = Mathf.Sign(moveInput);

        if (isGrounded && jumpPressed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumping = true;
            jumpTimer = 0f;
        }
        jumpPressed = false;

        if (isJumping && jumpHeld)
        {
            jumpTimer += Time.fixedDeltaTime;
            if (jumpTimer < maxJumpTime)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            else
                isJumping = false;
        }

        if (isJumping && jumpReleased)
        {
            if (rb.linearVelocity.y > minJumpForce)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, minJumpForce);
            isJumping = false;
        }
        jumpReleased = false;

        if (!isGrounded && !isJumping && jumpHeld && rb.linearVelocity.y < 0)
            rb.gravityScale = glideGravity;
        else
            rb.gravityScale = normalGravity;
    }

    void TryDropThrough()
    {
        if (playerCollider == null || touchedOneWays.Count == 0) return;

        var platforms = touchedOneWays.ToArray();
        foreach (var col in platforms)
        {
            if (col == null) continue;
            Physics2D.IgnoreCollision(playerCollider, col, true);
            StartCoroutine(ReenableCollision(col));
        }
    }

    System.Collections.IEnumerator ReenableCollision(Collider2D col)
    {
        yield return new WaitForSeconds(dropThroughTime);
        if (col != null && playerCollider != null)
            Physics2D.IgnoreCollision(playerCollider, col, false);
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
        {
            platformContacts++;
            isJumping = false;
        }

        if (col.gameObject.CompareTag("OneWayPlatform") && !touchedOneWays.Contains(col.collider))
            touchedOneWays.Add(col.collider);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (IsPlatform(col.gameObject))
            platformContacts = Mathf.Max(0, platformContacts - 1);

        if (col.gameObject.CompareTag("OneWayPlatform"))
            touchedOneWays.Remove(col.collider);
    }
}
