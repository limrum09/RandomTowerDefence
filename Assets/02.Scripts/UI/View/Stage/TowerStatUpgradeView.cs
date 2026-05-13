using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerStatUpgradeView : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI towerNameText;
    [SerializeField]
    private TextMeshProUGUI towerGradeText;
    [SerializeField]
    private TextMeshProUGUI towerSkillText;
    [SerializeField]
    private TextMeshProUGUI currentDamageStepText;
    [SerializeField]
    private TextMeshProUGUI currentDamageValueText;
    [SerializeField]
    private TextMeshProUGUI nextDamageStepText;
    [SerializeField]
    private TextMeshProUGUI nextDamageValueText;
    [SerializeField]
    private TextMeshProUGUI damagePriceText;
    [SerializeField]
    private TextMeshProUGUI currentAttackSpeedStepText;
    [SerializeField]
    private TextMeshProUGUI currentAttackSpeedValueText;
    [SerializeField]
    private TextMeshProUGUI nextAttackSpeedStepText;
    [SerializeField]
    private TextMeshProUGUI nextAttackSpeedValueText;
    [SerializeField]
    private TextMeshProUGUI attackSpeedPriceText;

    [SerializeField]
    private Button damageStatUpgradeBtn;
    [SerializeField]
    private Button attackSpeedStatUpgradeBtn;

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

        towerGradeText.text = grade.ToString() + "등급";
    }

    public void SetTowerName(string name) => towerNameText.text = name;
    public void SetSkillName(string name) => towerSkillText.text = name;
    public void SetCurrentDamageStepText(int value) => currentDamageStepText.text = $"{value}강";
    public void SetCurrentDamageText(int value) => currentDamageValueText.text = value.ToString();
    public void SetNextDamageStepText(int value) => nextDamageStepText.text = $"{value}강";
    public void SetNextDamageText(string value) => nextDamageValueText.text = value;
    public void SetDamaePriceText(int value) => damagePriceText.text = value.ToString();
    public void SetCurrentAttakSpeedStepText(int value) => currentAttackSpeedStepText.text = $"{value}강";
    public void SetCurrentAttakSpeedText(float value) => currentAttackSpeedValueText.text = value.ToString();
    public void SetNextAttakSpeedStepText(int value) => nextAttackSpeedStepText.text = $"{value}강";
    public void SetNextAttakSpeedText(string value) => nextAttackSpeedValueText.text = value;
    public void SetAttakSpeedPriceText(int value) => attackSpeedPriceText.text = value.ToString();

    public void BindDamageUpgrade(UnityAction action) => damageStatUpgradeBtn.onClick.AddListener(action);
    public void BindAttakSpeedUpgrade(UnityAction action) => attackSpeedStatUpgradeBtn.onClick.AddListener(action);
}
