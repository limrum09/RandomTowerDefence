using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerGradeUpgradeView : MonoBehaviour, IPopUpPublicUI
{
    [Header("Option")]
    [SerializeField]
    private CanvasGroup canvas;

    [Header("Info")]
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI towerNameText;
    [SerializeField]
    private TextMeshProUGUI towerGradeText;

    [Header("Skill Texts")]
    [SerializeField]
    private TextMeshProUGUI towerSkillText;
    [SerializeField]
    private TextMeshProUGUI towerSkillDesText;

    [Header("Stat Value Texts")]
    [SerializeField]
    private TextMeshProUGUI damageCurrentValueText;
    [SerializeField]
    private TextMeshProUGUI attackSpeedCurrentValueText;
    [SerializeField]
    private TextMeshProUGUI rangeCurretnValueText;

    [Header("Price Texts")]
    [SerializeField]
    private TextMeshProUGUI premiunUpgradePriceText;
    [SerializeField]
    private TextMeshProUGUI normalUpgradePriceText;
    [SerializeField]
    private TextMeshProUGUI towerSellPriceText;

    [Header("Images")]
    [SerializeField]
    private Image upgradeMaster1;
    [SerializeField]
    private Image upgradeMaster2;

    [Header("Buttons")]
    [SerializeField]
    private Button normalUpgradeBtn;
    [SerializeField]
    private Button premiumUpgradeBtn;
    [SerializeField]
    private Button sellBtn;

    [Header("ButtonTexts")]
    [SerializeField]
    private TextMeshProUGUI normalBtnText;
    [SerializeField]
    private TextMeshProUGUI premiumBtnText;
    public void Clear()
    {
        towerNameText.text = "";
        towerSkillText.text = "";
        towerSkillDesText.text = "";
        damageCurrentValueText.text = "";
        attackSpeedCurrentValueText.text = "";
        rangeCurretnValueText.text = "";
        premiunUpgradePriceText.text = "1000";
        normalUpgradePriceText.text = "300";
        towerSellPriceText.text = "";
        towerGradeText.text = "";

        normalBtnText.text = $"{Managers.Local.GetString("Stage", "STAGE_TOWER_NORMAL_UPGRADE")} ({Managers.InputData.GetKeyCode(InputAction.TowerGradeNormalUpgrade)})";
        premiumBtnText.text = $"{Managers.Local.GetString("Stage", "STAGE_TOWER_FIXED_UPGRADE")} ({Managers.InputData.GetKeyCode(InputAction.TowerGradePremiumUpgrade)})";

        normalUpgradeBtn.interactable = true;
        premiumUpgradeBtn.interactable = true;

        upgradeMaster1.gameObject.SetActive(false);
        upgradeMaster2.gameObject.SetActive(false);
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

    public void SetIconImage(Sprite icon) => iconImage.sprite = icon;
    public void TowerGrade(int grade, string nextUGUI)
    {
        if (nextUGUI == "Master" || nextUGUI == "MASTER")
        {
            towerGradeText.text = "Master";
            upgradeMaster1.gameObject.SetActive(true);
            upgradeMaster2.gameObject.SetActive(true);

            normalUpgradeBtn.interactable = false;
            premiumUpgradeBtn.interactable = false;
            return;
        }

        towerGradeText.text = string.Format(Managers.Local.GetString("UI", "UI_GRADE"), grade);
        upgradeMaster1.gameObject.SetActive(false);
        upgradeMaster2.gameObject.SetActive(false);
    }

    public void SetTowerName(string name) => towerNameText.text = name;
    public void SetSkillName(string name) => towerSkillText.text = name;
    public void SetSkillDes(string des) => towerSkillDesText.text = des;
    public void SetDamageCurrentValue(float value) => damageCurrentValueText.text = value.ToString();
    public void SetAttackSpeedCurrentValue(float value) => attackSpeedCurrentValueText.text = value.ToString();
    public void SetRangeCurrentValue(float value) => rangeCurretnValueText.text = value.ToString();
    public void PremiumUpgradePirce(int value) => premiunUpgradePriceText.text = value.ToString();
    public void NormalUpgradePrice(int value) => normalUpgradePriceText.text = value.ToString();
    public void TowerSellPrice(float value) => towerSellPriceText.text = value.ToString();

    public void BindNormalUpgrade(UnityAction action) => normalUpgradeBtn.onClick.AddListener(action);
    public void BindPreminumUpgrade(UnityAction action) => premiumUpgradeBtn.onClick.AddListener(action);
    public void BindTowerSell(UnityAction action) => sellBtn.onClick.AddListener(action);
}
