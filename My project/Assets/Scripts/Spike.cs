using UnityEngine;

public class Spike : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        PlayerHealth health = col.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            // knock the player upward and away from the spike
            Vector2 knockDir = (col.transform.position - transform.position).normalized;
            knockDir.y = 1f; // always knock up a bit
            health.TakeDamage(knockDir);
        }
    }
}
