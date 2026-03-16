using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public float idleDelay = 1f;

    private Animator animator;
    private Rigidbody2D rb;
    private float idleTimer;
    private bool isIdle;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        if (isMoving)
        {
            // reset the timer whenever the player moves
            idleTimer = 0f;
            if (isIdle)
            {
                isIdle = false;
                animator.enabled = false;
            }
        }
        else
        {
            // start counting up when the player stops
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleDelay && !isIdle)
            {
                isIdle = true;
                animator.enabled = true;
            }
        }
    }
}
