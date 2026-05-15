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

    private TowerData towerData;
    private MetaUpgradeView owner;
    private MetaUpgradeTarget upgradeTarget;
    private MetaUpgradeType upgradeType;
    private string uid;
    private int upValue = 1;
    private int index;

    private MetaResearchDataManager metaData;

    public void SetOwner(MetaUpgradeView getOwner)
    {
        owner = getOwner;

        metaData = Managers.ResearchData;


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

        nameText.text = $"{towerData.grade}등급 {Managers.TowerData.GetTowerNameType(towerData.towerType)}타워";
        icon.sprite = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{towerData.towerType}_{towerData.grade}_Idle");
        optionInfoText.text = "타워 공격력 및 공격속도 영구강화";

        MetaUpgradeDisplayData displayData = Managers.Game.GetTowerDisplayData(towerData);
        upgradeButtonFrame2.SetActive(displayData.useSecondValue);

        upgradeTypeText1.text = $"공격 속도 강화 (+{displayData.level1})";
        upgradeTypeText2.text = $"공격력 강화 (+{displayData.level2})";

        currentValueText1.text = displayData.currentValue1.ToString("N2");
        currentValueText2.text = displayData.currentValue2.ToString();

        nextValueText1.text = displayData.nextValue1.ToString("N2");
        nextValueText2.text = displayData.nextValue2.ToString();

        upgradeValueText1.text = displayData.costValue1.ToString();
        upgradeValueText2.text = displayData.costValue2.ToString();
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

        nameText.text = Managers.PublicMetaUpgrade.GetTypeName(upgradeType);
        optionInfoText.text = Managers.PublicMetaUpgrade.GetTypeInfoStr(upgradeType);

        MetaUpgradeDisplayData displayData = Managers.Game.GetPublicDisplayData(upgradeType);
        
        upgradeButtonFrame2.SetActive(displayData.useSecondValue);
        upgradeTypeText1.text = Managers.PublicMetaUpgrade.GetTypeCountStr(upgradeType) + $" 강화 (+{displayData.level1})";

        currentValueText1.text = displayData.currentValue1.ToString();
        nextValueText1.text = displayData.nextValue1.ToString();

        upgradeValueText1.text = displayData.costValue1.ToString();
    }

    private void OnButton1Click()
    {
        owner.MetaUpgrade(upgradeTarget, MetaUpgradeType.AttackSpeed, uid, upValue, index);
    }

    private void OnButton2Click()
    {
        owner.MetaUpgrade(upgradeTarget, MetaUpgradeType.Damage, uid, upValue, index);
    }

    private void ResetDatas()
    {
        towerData = null;
        upgradeTarget = MetaUpgradeTarget.Tower;
        uid = string.Empty;
        index = -1;
        upgradeType = MetaUpgradeType.StartingGold;
    }
}
