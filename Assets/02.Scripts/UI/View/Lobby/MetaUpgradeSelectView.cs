using System.Net.NetworkInformation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeSelectView : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI info;
    [SerializeField]
    private GameObject upgradeFrame1;
    [SerializeField]
    private TextMeshProUGUI upgradeText1;
    [SerializeField]
    private TextMeshProUGUI currentValue1;
    [SerializeField]
    private TextMeshProUGUI nextValue1;
    [SerializeField]
    private GameObject upgradeFrame2;
    [SerializeField]
    private TextMeshProUGUI upgradeText2;
    [SerializeField]
    private TextMeshProUGUI currentValue2;
    [SerializeField]
    private TextMeshProUGUI nextValue2;
    [SerializeField]
    private Button btn;

    private MetaUpgradeView Owner;
    private TowerData tower;
    private string getUID;
    private MetaUpgradeTarget upgradeTarget;
    private MetaUpgradeType upgradeType;
    private int index;

    private TowerMetaUpgradeManager towerMetaManager;
    private PublicMetaUpgradeManager publicMetaManager;
    private MetaResearchDataManager metaData;

    private void ResetDatas()
    {
        tower = null;
        getUID = string.Empty;
        upgradeTarget = MetaUpgradeTarget.Tower;
        index = -1;
        upgradeType = MetaUpgradeType.StartingGold;
    }

    public void SetOwner(MetaUpgradeView getOwner)
    {
        Owner = getOwner;
        btn.onClick.AddListener(OnClickSelectButton);
        metaData = Managers.ResearchData;
        towerMetaManager = Managers.TowerMetaUpgrade;
        publicMetaManager = Managers.PublicMetaUpgrade;
    }

    public void TowerUIRefresh()
    {
        upgradeFrame1.SetActive(true);
        
        title.text = $"{tower.grade}등급 {Managers.TowerData.GetTowerNameType(tower.towerType)}타워";
        icon.sprite = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{tower.towerType}_{tower.grade}_Idle");
        info.text = "타워 공격력 및 공격속도 영구강화";

        upgradeText1.text = "공격 속도";
        upgradeText2.text = "공격력";

        MetaUpgradeDisplayData displayData = Managers.Game.GetTowerDisplayData(tower);
        upgradeFrame2.SetActive(displayData.useSecondValue);

        currentValue1.text = displayData.currentValue1.ToString("N2");
        currentValue2.text = displayData.currentValue2.ToString();

        nextValue1.text = displayData.nextValue1.ToString("N2");
        nextValue2.text = displayData.nextValue2.ToString();
    }

    public void SetTowerDataView(TowerData data, MetaUpgradeTarget getUpgradeType, int getIndex)
    {
        ResetDatas();

        tower = data;
        getUID = tower.TowerUID;
        upgradeTarget = getUpgradeType;
        index = getIndex;

        TowerUIRefresh();
    }

    public void PublicUIRefresh()
    {
        title.text = publicMetaManager.GetTypeName(upgradeType);
        info.text = publicMetaManager.GetTypeInfoStr(upgradeType);
        upgradeText1.text = publicMetaManager.GetTypeCountStr(upgradeType);
        icon.sprite = null;

        MetaUpgradeDisplayData displayData = Managers.Game.GetPublicDisplayData(upgradeType);
        upgradeFrame2.SetActive(displayData.useSecondValue);

        currentValue1.text = displayData.currentValue1.ToString();
        nextValue1.text = displayData.nextValue1.ToString();
    }

    public void SetPublicDataView(MetaUpgradeType type, int getIndex)
    {
        upgradeFrame1.SetActive(true);

        ResetDatas();

        upgradeTarget = MetaUpgradeTarget.Public;
        upgradeType = type;
        index = getIndex;

        PublicUIRefresh();
    }

    public void OnClickSelectButton()
    {
        if(!string.IsNullOrEmpty(getUID))
            Owner.OnClickSelectButton(getUID, upgradeTarget, index);
        else
            Owner.OnClickSelectButton(upgradeType, upgradeTarget, index);
    }
}
