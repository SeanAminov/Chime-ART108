using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public float idleDelay = 1f;

    private Animator animator;
    private Rigidbody2D rb;
    private float idleTimer;
    private float lastDirection = 1f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float hSpeed = rb.linearVelocity.x;
        float absSpeed = Mathf.Abs(hSpeed);
        bool isMoving = absSpeed > 0.1f;

        if (isMoving)
            lastDirection = Mathf.Sign(hSpeed);

        if (isMoving)
        {
            idleTimer = 0f;
            animator.SetFloat("Speed", absSpeed);
            animator.SetFloat("Direction", Mathf.Sign(hSpeed));
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer < idleDelay)
                animator.SetFloat("Speed", 0.2f);
            else
                animator.SetFloat("Speed", 0f);

            animator.SetFloat("Direction", lastDirection);
        }
    }
}
