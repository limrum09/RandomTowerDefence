using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerStatUpgradeView : MonoBehaviour
{
    [Header("Info")]
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI towerNameText;
    [SerializeField]
    private TextMeshProUGUI towerGradeText;
    [SerializeField]
    private TextMeshProUGUI towerSkillText;

    [Header("Stat Value Text")]
    [SerializeField]
    private TextMeshProUGUI currentDamageStepText;
    [SerializeField]
    private TextMeshProUGUI currentDamageValueText;
    [SerializeField]
    private TextMeshProUGUI nextDamageStepText;
    [SerializeField]
    private TextMeshProUGUI nextDamageValueText;
    [SerializeField]
    private TextMeshProUGUI currentAttackSpeedStepText;
    [SerializeField]
    private TextMeshProUGUI currentAttackSpeedValueText;
    [SerializeField]
    private TextMeshProUGUI nextAttackSpeedStepText;
    [SerializeField]
    private TextMeshProUGUI nextAttackSpeedValueText;

    [Header("Upgrade Value Text")]
    [SerializeField]
    private TextMeshProUGUI damagePriceText;
    [SerializeField]
    private TextMeshProUGUI attackSpeedPriceText;

    [Header("UpGrade Button")]
    [SerializeField]
    private Button damageStatUpgradeBtn;
    [SerializeField]
    private Button attackSpeedStatUpgradeBtn;

    private string upgradeText;
    private string gradeText;
    public void Init()
    {
        upgradeText = Managers.Local.GetString("Sheets", "TEXT_UPGRADE_STEP");
        gradeText = Managers.Local.GetString("Sheets","TEXT_GRADE");
    }

    public void Clear()
    {
        towerNameText.text = "";
        towerSkillText.text = "";
        towerGradeText.text = "";
        currentDamageStepText.text = "";
        currentDamageValueText.text = "";
        nextDamageStepText.text = "";
        nextDamageValueText.text = "";
        damagePriceText.text = "";
        currentAttackSpeedStepText.text = "";
        currentAttackSpeedValueText.text = "";
        nextAttackSpeedStepText.text = "";
        nextAttackSpeedValueText.text = "";
        attackSpeedPriceText.text = "";
    }

    public void Hide()
    {
        Clear();
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void SetIconImage(Sprite icon) => iconImage.sprite = icon;

    public void TowerGrade(int grade, string nextUGUI)
    {
        if (nextUGUI == "Master" || nextUGUI == "MASTER")
        {
            towerGradeText.text = "Master";
            return;
        }

        towerGradeText.text = string.Format(gradeText, grade);
    }

    public void SetTowerName(string name) => towerNameText.text = name;
    public void SetSkillName(string name) => towerSkillText.text = name;
    public void SetCurrentDamageStepText(int value) => currentDamageStepText.text = string.Format(upgradeText, value);
    public void SetCurrentDamageText(float value) => currentDamageValueText.text = value.ToString();
    public void SetNextDamageStepText(int value) => nextDamageStepText.text = string.Format(upgradeText, value);
    public void SetNextDamageText(string value) => nextDamageValueText.text = value;
    public void SetDamaePriceText(int value) => damagePriceText.text = value.ToString();
    public void SetCurrentAttakSpeedStepText(int value) => currentAttackSpeedStepText.text = string.Format(upgradeText, value);
    public void SetCurrentAttakSpeedText(float value) => currentAttackSpeedValueText.text = value.ToString();
    public void SetNextAttakSpeedStepText(int value) => nextAttackSpeedStepText.text = string.Format(upgradeText, value);
    public void SetNextAttakSpeedText(string value) => nextAttackSpeedValueText.text = value;
    public void SetAttakSpeedPriceText(int value) => attackSpeedPriceText.text = value.ToString();

    public void BindDamageUpgrade(UnityAction action) => damageStatUpgradeBtn.onClick.AddListener(action);
    public void BindAttakSpeedUpgrade(UnityAction action) => attackSpeedStatUpgradeBtn.onClick.AddListener(action);
}
