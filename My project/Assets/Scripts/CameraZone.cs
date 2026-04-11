using UnityEngine;

public class CameraZone : MonoBehaviour
{
    public bool enableFreeFollow = true;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CameraFollow cam = Camera.main.GetComponent<CameraFollow>();
        if (cam != null)
            cam.freeFollow = enableFreeFollow;
    }
}
