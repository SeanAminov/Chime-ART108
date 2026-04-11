using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ladder : MonoBehaviour
{
    [Header("Destination")]
    public Transform destination;

    [Header("Fade")]
    public Image fadePanel;
    public float fadeDuration = 0.5f;

    [Header("References")]
    public PlayerMovement playerMovement;
    public DialogueBox dialogueBox;

    private bool playerInRange;
    private bool isTransitioning;

    void Update()
    {
        if (playerInRange && !isTransitioning && Input.GetKeyDown(KeyCode.F))
        {
            if (dialogueBox != null && dialogueBox.gameObject.activeSelf)
                return;

            StartCoroutine(ClimbTransition());
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    IEnumerator ClimbTransition()
    {
        isTransitioning = true;

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
            playerMovement.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        }

        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadePanel.color = new Color(0, 0, 0, timer / fadeDuration);
                yield return null;
            }
            fadePanel.color = Color.black;
        }

        if (destination != null && playerMovement != null)
            playerMovement.transform.position = destination.position;

        yield return new WaitForSeconds(0.3f);

        if (fadePanel != null)
        {
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadePanel.color = new Color(0, 0, 0, 1f - (timer / fadeDuration));
                yield return null;
            }
            fadePanel.gameObject.SetActive(false);
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        isTransitioning = false;
    }
}
