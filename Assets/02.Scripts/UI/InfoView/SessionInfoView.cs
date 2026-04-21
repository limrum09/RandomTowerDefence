using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionInfoView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI currentLevelText;
    [SerializeField]
    private TextMeshProUGUI currentExpText;
    [SerializeField]
    private TextMeshProUGUI needExpText;
    [SerializeField]
    private TextMeshProUGUI currentWaveText;
    [SerializeField]
    private TextMeshProUGUI currentLifeText;
    [SerializeField]
    private Image expBar;

    public void SetCurrentLevel(int value)
    {
        string frontText = "Lv .";

        if (value < 10)
            frontText += "0";

        currentLevelText.text = frontText + value.ToString();
    }
    public void SetCurrentExpBar(int currentExp, int needExp)
    {
        currentExpText.text = currentExp.ToString();
        needExpText.text = needExp.ToString();

        float expPer = (float)currentExp / needExp;
        Debug.Log("EXP Per - " + expPer);
        expBar.fillAmount = expPer;
    }
    public void SetCurrentWave(int value) => currentWaveText.text = value.ToString();
    public void SetCurrentLife(int value) => currentLifeText.text = value.ToString();
}
