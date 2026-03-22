using UnityEngine;

public class SeeAbility : MonoBehaviour
{
    // the layer that hidden platforms live on
    public LayerMask hiddenLayer;

    private Collider2D playerCollider;
    private bool seeing;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        bool spaceHeld = Input.GetKey(KeyCode.Space);

        if (spaceHeld && !seeing)
        {
            seeing = true;
            ShowHiddenPlatforms(true);
        }
        else if (!spaceHeld && seeing)
        {
            seeing = false;
            ShowHiddenPlatforms(false);
        }
    }

    void ShowHiddenPlatforms(bool visible)
    {
        // find all objects tagged as hidden platforms
        GameObject[] hiddenPlatforms = GameObject.FindGameObjectsWithTag("HiddenPlatform");

        foreach (GameObject platform in hiddenPlatforms)
        {
            // show or hide the sprite
            SpriteRenderer sr = platform.GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.enabled = visible;

            // enable or disable the collider so the player can stand on it
            Collider2D col = platform.GetComponent<Collider2D>();
            if (col != null)
                col.enabled = visible;
        }
    }
}
