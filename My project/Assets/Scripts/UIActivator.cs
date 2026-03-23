using UnityEngine;

public class UIActivator : MonoBehaviour
{
    public GameObject uiCanvas;

    void Start()
    {
        // enable the UI canvas when the game starts
        if (uiCanvas != null)
            uiCanvas.SetActive(true);
    }
}
