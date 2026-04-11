using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public float parallaxSpeed = 0.1f;

    private Transform cam;
    private Vector2 lastCamPos;

    void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
    }

    void LateUpdate()
    {
        Vector2 delta = (Vector2)cam.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxSpeed, delta.y * parallaxSpeed, 0f);
        lastCamPos = cam.position;
    }
}
