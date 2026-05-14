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
    private MetaUpgradeTarget metaType;
    private MetaUpgradeType publicType;
    private string uid;
    private int upValue = 1;
    private int index;

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
        metaType = getType;
        uid = getTower.TowerUID;
        index = getIndex;

        RefreshTowerInfo();
    }

    public void RefreshTowerInfo()
    {
        upgradeButtonFrame1.SetActive(true);
        upgradeButtonFrame2.SetActive(true);

        nameText.text = $"{towerData.grade}등급 {Managers.TowerData.GetTowerNameType(towerData.towerType)}타워";
        icon.sprite = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{towerData.towerType}_{towerData.grade}_Idle");
        optionInfoText.text = "타워 공격력 및 공격속도 영구강화";

        upgradeTypeText1.text = "공격 속도 강화";
        upgradeTypeText2.text = "공격력 강화";

        int speedLevel = Managers.TowerMetaUpgrade.GetAttackSpeedLevel(towerData.towerType, towerData.grade);
        int damageLevel = Managers.TowerMetaUpgrade.GetDamageLevel(towerData.towerType, towerData.grade);

        currentValueText1.text = (towerData.baseAtkSpeed + (2.0f * speedLevel)).ToString();
        currentValueText2.text = (towerData.baseAtk + (2.0f * damageLevel)).ToString();

        nextValueText1.text = (towerData.baseAtkSpeed + (2.0f * (speedLevel + 1))).ToString();
        nextValueText2.text = (towerData.baseAtk + (2.0f * (damageLevel + 1))).ToString();

        upgradeValueText1.text = "9999";
        upgradeValueText2.text = "12345";
    }

    public void SetPublicInfo(MetaUpgradeType getPubicType, MetaUpgradeTarget getType, int getIndex)
    {
        ResetDatas();

        publicType = getPubicType;
        metaType = getType;
        index = getIndex;
        uid = publicType.ToString();

        RefreshPublicInfo();
    }

    public void RefreshPublicInfo()
    {
        upgradeButtonFrame1.SetActive(true);
        upgradeButtonFrame2.SetActive(false);

        nameText.text = Managers.PublicMetaUpgrade.GetTypeName(publicType);
        optionInfoText.text = Managers.PublicMetaUpgrade.GetTypeInfoStr(publicType);
        upgradeTypeText1.text = Managers.PublicMetaUpgrade.GetTypeCountStr(publicType) + " 강화";

        int level = Managers.PublicMetaUpgrade.GetPublicMetaDataLevel(publicType);
        currentValueText1.text = (500 + (100 * level)).ToString();
        nextValueText1.text = (500 + (100 * (level + 1))).ToString();

        upgradeValueText1.text = "1568";
    }

    private void OnButton1Click()
    {
        owner.MetaUpgrade(metaType, MetaUpgradeType.AttackSpeed, uid, upValue, index);
    }

    private void OnButton2Click()
    {
        owner.MetaUpgrade(metaType, MetaUpgradeType.Damage, uid, upValue, index);
    }

    private void ResetDatas()
    {
        towerData = null;
        metaType = MetaUpgradeTarget.Tower;
        uid = string.Empty;
        index = -1;
        publicType = MetaUpgradeType.StartingGold;
    }
}
