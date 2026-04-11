using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3f;
    public float lookAhead = 1.5f;
    public float horizontalOffset = 2f;
    public float verticalSmoothTime = 0.5f;
    public float verticalOffset = 2f;

    [Header("Free Follow (glide sections)")]
    public float freeSmoothTime = 0.1f;
    public float freeVerticalSmoothTime = 0.1f;

    [HideInInspector] public bool freeFollow;

    private float currentX;
    private float currentY;
    private float xVelocity;
    private float yVelocity;
    private float targetY;
    private Rigidbody2D targetRb;
    private PlayerMovement playerMovement;

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

        float ahead = 0f;
        if (targetRb != null)
        {
            float speed = targetRb.linearVelocity.x;
            ahead = Mathf.Sign(speed) * lookAhead * Mathf.Clamp01(Mathf.Abs(speed) / 3f);
        }

        float hSmooth = freeFollow ? freeSmoothTime : smoothTime;
        float vSmooth = freeFollow ? freeVerticalSmoothTime : verticalSmoothTime;

        float desiredX = target.position.x + ahead + horizontalOffset;
        currentX = Mathf.SmoothDamp(currentX, desiredX, ref xVelocity, hSmooth);

        if (freeFollow || (playerMovement != null && playerMovement.isGrounded))
            targetY = target.position.y + verticalOffset;

        currentY = Mathf.SmoothDamp(currentY, targetY, ref yVelocity, vSmooth);

        Vector3 pos = new Vector3(currentX, currentY, transform.position.z);

        if (shakeDuration > 0)
        {
            pos += (Vector3)Random.insideUnitCircle * shakeIntensity;
            shakeDuration -= Time.deltaTime;
        }

        transform.position = pos;
    }

    public void Shake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeIntensity = intensity;
    }
}
