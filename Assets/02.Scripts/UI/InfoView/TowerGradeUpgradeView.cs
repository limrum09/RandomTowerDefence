using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TowerGradeUpgradeView : MonoBehaviour, IPopUpPublicUI
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
    private TextMeshProUGUI towerSkillDesText;
    [SerializeField]
    private TextMeshProUGUI damageCurrentValueText;
    [SerializeField]
    private TextMeshProUGUI attackSpeedCurrentValueText;
    [SerializeField]
    private TextMeshProUGUI rangeCurretnValueText;
    [SerializeField]
    private TextMeshProUGUI premiunUpgradePriceText;
    [SerializeField]
    private TextMeshProUGUI normalUpgradePriceText;
    [SerializeField]
    private TextMeshProUGUI towerSellPriceText;
    [SerializeField]
    private Image upgradeMaster1;
    [SerializeField]
    private Image upgradeMaster2;

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

        upgradeMaster1.gameObject.SetActive(false);
        upgradeMaster2.gameObject.SetActive(false);
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

    public void SetIconImage(string path)
    {
        Sprite icon = Resources.Load<Sprite>("Tower/Images/" + path + "_Idle");
        iconImage.sprite = icon;
    }

    public void TowerGrade(int grade, string nextUGUI)
    {
        if (nextUGUI == "Master" || nextUGUI == "MASTER")
        {
            towerGradeText.text = "Master";
            upgradeMaster1.gameObject.SetActive(true);
            upgradeMaster2.gameObject.SetActive(true);
            return;
        }

        towerGradeText.text = grade.ToString() + "µî±Þ";
        upgradeMaster1.gameObject.SetActive(false);
        upgradeMaster2.gameObject.SetActive(false);
    }

    public void SetTowerName(string name) => towerNameText.text = name;
    public void SetSkillName(string name) => towerSkillText.text = name;
    public void SetSkillDes(string des) => towerSkillDesText.text = des;
    public void SetDamageCurrentValue(int value) => damageCurrentValueText.text = value.ToString();
    public void SetAttackSpeedCurrentValue(float value) => attackSpeedCurrentValueText.text = value.ToString();
    public void SetRangeCurrentValue(float value) => rangeCurretnValueText.text = value.ToString();
    public void PremiumUpgradePirce(int value) => premiunUpgradePriceText.text = value.ToString();
    public void NormalUpgradePrice(int value) => normalUpgradePriceText.text = value.ToString();
    public void TowerSellPrice(float value) => towerSellPriceText.text = value.ToString();
}
