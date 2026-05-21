using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SessionInfoView : MonoBehaviour
{
    [Header("Level")]
    [SerializeField]
    private TextMeshProUGUI currentLevelText;
    [SerializeField]
    private TextMeshProUGUI currentExpText;
    [SerializeField]
    private TextMeshProUGUI needExpText;
    [SerializeField]
    private Image expBar;

    [Header("Wave")]
    [SerializeField]
    private TextMeshProUGUI currentWaveText;
    [SerializeField]
    private TextMeshProUGUI currentSpawnEnemyCountText;

    [Header("Life")]
    [SerializeField]
    private TextMeshProUGUI currentLifeText;

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
        expBar.fillAmount = expPer;
    }

    public void SetEnemyRemainCount(int remain, int total) => currentSpawnEnemyCountText.text = $"{remain} / {total}";
    public void SetCurrentWave(int value) => currentWaveText.text = $"{value.ToString()} / 60";
    public void SetCurrentLife(int value) => currentLifeText.text = value.ToString();
}
