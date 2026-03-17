using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public float idleDelay = 1f;

    private Animator animator;
    private Rigidbody2D rb;
    private float idleTimer;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float speed = Mathf.Abs(rb.linearVelocity.x);
        bool isMoving = speed > 0.1f;

        if (isMoving)
        {
            // walking, reset the idle timer
            idleTimer = 0f;
            animator.SetFloat("Speed", speed);
        }
        else
        {
            idleTimer += Time.deltaTime;

            // keep reporting movement until the idle delay kicks in
            if (idleTimer < idleDelay)
                animator.SetFloat("Speed", 0.2f);
            else
                animator.SetFloat("Speed", 0f);
        }
    }
}
