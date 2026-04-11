using UnityEngine;
using TMPro;

public class Level1Manager : MonoBehaviour
{
    [Header("Tear Collection")]
    public int tearsRequired = 6;

    [Header("UI")]
    public TextMeshProUGUI waterText;

    void Awake()
    {
        WaterDroplet.totalRequired = tearsRequired;
        WaterDroplet.waterText = waterText;
        WaterDroplet.ResetCount();
    }

    void OnValidate()
    {
        if (tearsRequired < 1) tearsRequired = 1;
        WaterDroplet.totalRequired = tearsRequired;
    }
}
