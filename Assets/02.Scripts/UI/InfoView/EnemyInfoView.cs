using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoView : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI enemyNameText;
    [SerializeField]
    private TextMeshProUGUI enemyLevelText;
    [SerializeField]
    private TextMeshProUGUI enemyHealthValueText;
    [SerializeField]
    private TextMeshProUGUI enemySheildValueText;
    [SerializeField]
    private TextMeshProUGUI enemySpeedValueText;
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
        this.gameObject.SetActive(false);
        Clear();
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void SetIcon(Sprite icon) => iconImage.sprite = icon;
    public void SetName(string name) => enemyNameText.text = name;
    public void SetLevel(int value) => enemyLevelText.text = value.ToString();
    public void SetHealthText(int value) => enemyHealthValueText.text = value.ToString();
    public void SetSheildText(int value) => enemySheildValueText.text = value.ToString();
    public void SetSpeedText(float value) => enemySpeedValueText.text = value.ToString();
    public void SetSkillName(string skillName) => skillNameText.text = skillName;
    public void SetSkillDesText(string des) => skillDesText.text = des;
}
