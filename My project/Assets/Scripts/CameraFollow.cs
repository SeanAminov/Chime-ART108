using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public float lookAhead = 1.5f;
    public float horizontalOffset = 2f;
    public float verticalSmoothTime = 0.5f;
    public float verticalOffset = 2f;

    private float currentX;
    private float currentY;
    private float xVelocity;
    private float yVelocity;
    private float targetY;
    private Rigidbody2D targetRb;
    private PlayerMovement playerMovement;

    // screen shake
    private float shakeDuration;
    private float shakeIntensity;

    void Start()
    {
        if (target == null) return;

        currentX = target.position.x;
        currentY = target.position.y;
        targetY = currentY;
        targetRb = target.GetComponent<Rigidbody2D>();
        playerMovement = target.GetComponent<PlayerMovement>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        // horizontal: smooth follow with lookahead
        float ahead = 0f;
        if (targetRb != null)
        {
            float speed = targetRb.linearVelocity.x;
            ahead = Mathf.Sign(speed) * lookAhead * Mathf.Clamp01(Mathf.Abs(speed) / 3f);
        }

        float desiredX = target.position.x + ahead + horizontalOffset;
        currentX = Mathf.SmoothDamp(currentX, desiredX, ref xVelocity, smoothTime);

        // vertical: only update target when grounded so jumps don't move the camera
        if (playerMovement != null && playerMovement.isGrounded)
            targetY = target.position.y + verticalOffset;

        currentY = Mathf.SmoothDamp(currentY, targetY, ref yVelocity, verticalSmoothTime);

        Vector3 pos = new Vector3(currentX, currentY, transform.position.z);

        // add shake offset
        if (shakeDuration > 0)
        {
            pos += (Vector3)Random.insideUnitCircle * shakeIntensity;
            shakeDuration -= Time.deltaTime;
        }

        transform.position = pos;
    }

    // call this from other scripts to shake the camera
    public void Shake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeIntensity = intensity;
    }
}
