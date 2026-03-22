using UnityEngine;

public class NaiFollow : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 8f;
    public float leadDistance = 1.5f;
    public float idleYOffset = 0f;
    public float walkYOffset = 0f;

    private Animator animator;
    private PlayerMovement playerMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (player != null)
            playerMovement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (player == null || playerMovement == null) return;

        float facing = playerMovement.facingDirection;

        // smooth X to lead in front of the player
        float targetX = player.position.x + (leadDistance * facing);
        float newX = Mathf.Lerp(transform.position.x, targetX, followSpeed * Time.deltaTime);

        // is she still catching up?
        bool isMoving = Mathf.Abs(targetX - newX) > 0.05f;

        // just match the player's Y with the right offset
        float yOff = isMoving ? walkYOffset : idleYOffset;
        float newY = player.position.y + yOff;

        transform.position = new Vector3(newX, newY, transform.position.z);

        // animation
        if (animator != null)
            animator.SetFloat("Speed", isMoving ? 1f : 0f);

        // flip to match player direction
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * facing;
        transform.localScale = scale;
    }
}
