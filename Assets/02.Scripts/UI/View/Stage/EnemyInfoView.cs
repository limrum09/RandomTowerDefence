using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoView : MonoBehaviour
{
    [Header("Option")]
    [SerializeField]
    private CanvasGroup canvas;

    [Header("Info")]
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI enemyNameText;
    [SerializeField]
    private TextMeshProUGUI enemyLevelText;

    [Header("Stat Value Texts")]
    [SerializeField]
    private TextMeshProUGUI enemyHealthValueText;
    [SerializeField]
    private TextMeshProUGUI enemySheildValueText;
    [SerializeField]
    private TextMeshProUGUI enemySpeedValueText;

    [Header("Skill Texts")]
    [SerializeField]
    private TextMeshProUGUI skillNameText;
    [SerializeField]
    private TextMeshProUGUI skillDesText;

    private void Clear()
    {
        iconImage.sprite = null;
        enemyNameText.text = string.Empty;
        enemyLevelText.text = string.Empty;
        enemyHealthValueText.text = string.Empty;
        enemySheildValueText.text = string.Empty;
        enemySpeedValueText.text = string.Empty;
        skillNameText.text = string.Empty;
        skillDesText.text = string.Empty;
    }

    public void Hide()
    {
        Clear();

        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    public void Show()
    {
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void SetIcon(Sprite icon) => iconImage.sprite = icon;
    public void SetName(string name) => enemyNameText.text = name;
    public void SetLevel(string value) => enemyLevelText.text = value;
    public void SetHealthText(int value) => enemyHealthValueText.text = value.ToString();
    public void SetSheildText(int value) => enemySheildValueText.text = value.ToString();
    public void SetSpeedText(float value) => enemySpeedValueText.text = value.ToString();
    public void SetSkillName(string skillName) => skillNameText.text = skillName;
    public void SetSkillDesText(string des) => skillDesText.text = des;
}
