using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public IntroSequence introSequence;

    [Header("Game UI (hidden on start, shown on play)")]
    public GameObject healthText;
    public GameObject coinText;
    public GameObject energySlider;
    public GameObject waterText;

    void Start()
    {
        menuPanel.SetActive(true);
        introSequence.gameObject.SetActive(false);

        if (healthText != null) healthText.SetActive(false);
        if (coinText != null) coinText.SetActive(false);
        if (energySlider != null) energySlider.SetActive(false);
        if (waterText != null) waterText.SetActive(false);
    }

    public void OnStartClicked()
    {
        menuPanel.SetActive(false);
        introSequence.gameObject.SetActive(true);

        if (healthText != null) healthText.SetActive(true);
        if (coinText != null) coinText.SetActive(true);
        if (energySlider != null) energySlider.SetActive(true);
        // water text appears after the Kappa dialogue
        if (waterText != null) waterText.SetActive(false);
    }
}
