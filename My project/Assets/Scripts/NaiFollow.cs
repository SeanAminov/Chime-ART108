using UnityEngine;

public class NaiFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 8f;
    public float leadDistance = 2.5f;
    public float idleYOffset = 0f;
    public float walkYOffset = 0f;

    private Animator animator;
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;
    private bool playerIsMoving;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
            playerRb = player.GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (player == null || playerMovement == null) return;

        float facing = playerMovement.facingDirection;
        float targetX = player.position.x + (leadDistance * facing);

        playerIsMoving = playerRb != null && Mathf.Abs(playerRb.linearVelocity.x) > 0.1f;

        // faster when far, snappier when close
        float dist = Mathf.Abs(targetX - transform.position.x);
        float speed = followSpeed;
        if (dist < 0.3f)
            speed *= 4f;

        float newX = Mathf.Lerp(transform.position.x, targetX, speed * Time.deltaTime);

        float yOff = playerIsMoving ? walkYOffset : idleYOffset;
        float newY = player.position.y + yOff;

        transform.position = new Vector3(newX, newY, transform.position.z);

        if (animator != null)
            animator.SetFloat("Speed", playerIsMoving ? 1f : 0f);

        // flip to match player direction
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        transform.localScale = scale;
    }
}
