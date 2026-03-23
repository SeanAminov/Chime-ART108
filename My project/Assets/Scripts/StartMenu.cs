using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StartMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public IntroSequence introSequence;

    void Start()
    {
        // show menu, hide intro until player clicks start
        menuPanel.SetActive(true);
        introSequence.gameObject.SetActive(false);
    }

    public void OnStartClicked()
    {
        menuPanel.SetActive(false);
        introSequence.gameObject.SetActive(true);
    }
}
