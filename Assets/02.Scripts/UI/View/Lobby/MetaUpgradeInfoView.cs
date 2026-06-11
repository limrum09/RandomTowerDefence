using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeInfoView : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI optionInfoText;
    [SerializeField]
    private GameObject upgradeButtonFrame1;
    [SerializeField]
    private Button upgradeButton1;
    [SerializeField]
    private TextMeshProUGUI upgradeTypeText1;
    [SerializeField]
    private TextMeshProUGUI currentValueText1;
    [SerializeField]
    private TextMeshProUGUI nextValueText1;
    [SerializeField]
    private TextMeshProUGUI upgradeValueText1;
    [SerializeField]
    private GameObject upgradeButtonFrame2;
    [SerializeField]
    private Button upgradeButton2;
    [SerializeField]
    private TextMeshProUGUI upgradeTypeText2;
    [SerializeField]
    private TextMeshProUGUI currentValueText2;
    [SerializeField]
    private TextMeshProUGUI nextValueText2;
    [SerializeField]
    private TextMeshProUGUI upgradeValueText2;
    [SerializeField]
    private float fadeInDuration = 0.3f;

    private TowerData towerData;
    private MetaUpgradeView owner;
    private MetaUpgradeTarget upgradeTarget;
    private MetaUpgradeType upgradeType;
    private string uid;
    private int upValue = 1;
    private int index;
    private int upgradeCostValue1;
    private int upgradeCostValue2;

    private int prevLevel1;
    private int prevLevel2;
    public void SetOwner(MetaUpgradeView getOwner)
    {
        owner = getOwner;

        upgradeButton1.onClick.AddListener(OnButton1Click);
        upgradeButton2.onClick.AddListener(OnButton2Click);
    }

    public void SetTowerInfo(TowerData getTower, MetaUpgradeTarget getType, int getIndex)
    {
        ResetDatas();

        towerData = getTower;
        upgradeTarget = getType;
        uid = getTower.TowerUID;
        index = getIndex;

        RefreshTowerInfo();
    }

    public void RefreshTowerInfo()
    {
        upgradeButtonFrame1.SetActive(true);

        string gradeStr = string.Format(Managers.Local.GetString("UI", "UI_GRADE"), towerData.grade);
        string towerStr = Managers.Local.GetString("Tower", towerData.stringKey) + Managers.Local.GetString("UI", "UI_TOWER");
        nameText.text = $"{gradeStr} {towerStr}";
        icon.sprite = ResourceCache.Load<Sprite>($"Tower/Images/Icon_Tower_{towerData.towerType}_{towerData.grade}_Idle");
        optionInfoText.text = Managers.Local.GetString("UI", "UI_META_TOWER_INFO");

        MetaUpgradeCal displayData = Managers.Game.GetTowerDisplayData(towerData);
        upgradeButtonFrame2.SetActive(displayData.useSecondValue);

        upgradeTypeText1.text = $"{Managers.Local.GetString("UI", "UI_ATK_SPEED_UPGRADE")} (+{displayData.level1})";
        upgradeTypeText2.text = $"{Managers.Local.GetString("UI", "UI_ATK_DAMAGE_UPGRADE")} (+{displayData.level2})";

        currentValueText1.text = displayData.currentValue1.ToString("N2");
        currentValueText2.text = displayData.currentValue2.ToString();

        nextValueText1.text = displayData.nextValue1.ToString("N2");
        nextValueText2.text = displayData.nextValue2.ToString();

        upgradeCostValue1 = displayData.costValue1;
        upgradeCostValue2 = displayData.costValue2;

        upgradeValueText1.text = upgradeCostValue1.ToString();
        upgradeValueText2.text = upgradeCostValue2.ToString();

        if(prevLevel1 != -1 && prevLevel1 != displayData.level1)
        {
            currentValueText1.FadeIn(fadeInDuration);
            nextValueText1.FadeIn(fadeInDuration);
            upgradeValueText1.FadeIn(fadeInDuration);
        }

        if(prevLevel2 != -1 && prevLevel2 != displayData.level2)
        {
            currentValueText2.FadeIn(fadeInDuration);
            nextValueText2.FadeIn(fadeInDuration);
            upgradeValueText2.FadeIn(fadeInDuration);
        }

        prevLevel1 = displayData.level1;
        prevLevel2 = displayData.level2;
    }

    public void SetPublicInfo(MetaUpgradeType getPubicType, MetaUpgradeTarget getType, int getIndex)
    {
        ResetDatas();

        upgradeType = getPubicType;
        upgradeTarget = getType;
        index = getIndex;
        uid = upgradeType.ToString();

        RefreshPublicInfo();
    }

    public void RefreshPublicInfo()
    {
        upgradeButtonFrame1.SetActive(true);

        nameText.text = Managers.Local.GetString("UI", Managers.PublicMetaUpgrade.GetTypeName(upgradeType));
        optionInfoText.text = Managers.Local.GetString("UI", Managers.PublicMetaUpgrade.GetTypeInfoStr(upgradeType));

        MetaUpgradeCal displayData = Managers.Game.GetPublicDisplayData(upgradeType);
        
        upgradeButtonFrame2.SetActive(displayData.useSecondValue);
        upgradeTypeText1.text = Managers.Local.GetString("UI", Managers.PublicMetaUpgrade.GetTypeCountStr(upgradeType))
            + $" (+{displayData.level1})";

        currentValueText1.text = displayData.currentValue1.ToString();
        nextValueText1.text = displayData.nextValue1.ToString();

        upgradeCostValue1 = displayData.costValue1;
        upgradeValueText1.text = upgradeCostValue1.ToString();

        if (prevLevel1 != -1)
        {
            currentValueText1.FadeIn(fadeInDuration);
            nextValueText1.FadeIn(fadeInDuration);
            upgradeValueText1.FadeIn(fadeInDuration);
        }

        prevLevel1 = displayData.level1;
    }

    private void OnButton1Click()
    {
        owner.MetaUpgrade(upgradeTarget, MetaUpgradeType.AttackSpeed, uid, upgradeCostValue1, upValue, index);
    }

    private void OnButton2Click()
    {
        owner.MetaUpgrade(upgradeTarget, MetaUpgradeType.Damage, uid, upgradeCostValue2, upValue, index);
    }

    private void ResetDatas()
    {
        towerData = null;
        upgradeTarget = MetaUpgradeTarget.Tower;
        uid = string.Empty;
        index = -1;
        upgradeType = MetaUpgradeType.StartingGold;

        prevLevel1 = -1;
        prevLevel2 = -1;
    }
}
