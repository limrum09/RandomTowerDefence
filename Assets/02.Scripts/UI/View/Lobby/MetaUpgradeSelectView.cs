using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeSelectView : MonoBehaviour
{
    [Header("Basic")]
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

    [Header("Limit")]
    [SerializeField]
    private GameObject limitPanel;
    [SerializeField]
    private TextMeshProUGUI limitText;

    private MetaUpgradeView Owner;
    private TowerData tower;
    private string getUID;
    private MetaUpgradeTarget upgradeTarget;
    private MetaUpgradeType upgradeType;
    private int index;

    private TowerMetaUpgradeManager towerMetaManager;
    private PublicMetaUpgradeManager publicMetaManager;
    private MetaResearchUpgradeDataManager metaData;

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
        metaData = Managers.ResearchUpgrade;
        towerMetaManager = Managers.TowerMetaUpgrade;
        publicMetaManager = Managers.PublicMetaUpgrade;
    }

    public void TowerUIRefresh()
    {
        upgradeFrame1.SetActive(true);

        string gradeStr = string.Format(Managers.Local.GetString("UI", "UI_GRADE"), tower.grade);
        string towerStr = Managers.Local.GetString("Tower", tower.stringKey) + Managers.Local.GetString("UI", "UI_TOWER");
        title.text = $"{gradeStr} {towerStr}";
        icon.sprite = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{tower.towerType}_{tower.grade}_Idle");
        info.text = Managers.Local.GetString("UI", "UI_META_TOWER_INFO");

        upgradeText1.text = Managers.Local.GetString("UI", "UI_ATK_SPEED");
        upgradeText2.text = Managers.Local.GetString("UI", "UI_ATK_DAMAGE");

        MetaUpgradeCal displayData = Managers.Game.GetTowerDisplayData(tower);
        upgradeFrame2.SetActive(displayData.useSecondValue);

        currentValue1.text = displayData.currentValue1.ToString("N2");
        currentValue2.text = displayData.currentValue2.ToString();

        nextValue1.text = displayData.nextValue1.ToString("N2");
        nextValue2.text = displayData.nextValue2.ToString();

        limitPanel.SetActive(displayData.isUnlocked);
        btn.interactable = !displayData.isUnlocked;
        
        if(displayData.isUnlocked)
            limitText.text = string.Format(Managers.Local.GetString("UI", "UI_META_LIMIT"), displayData.needResearchLevel);
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
        title.text = Managers.Local.GetString("UI", publicMetaManager.GetTypeName(upgradeType));
        info.text = Managers.Local.GetString("UI", publicMetaManager.GetTypeInfoStr(upgradeType));
        upgradeText1.text = Managers.Local.GetString("UI", publicMetaManager.GetTypeCountStr(upgradeType));
        icon.sprite = null;

        MetaUpgradeCal displayData = Managers.Game.GetPublicDisplayData(upgradeType);
        upgradeFrame2.SetActive(displayData.useSecondValue);

        currentValue1.text = displayData.currentValue1.ToString();
        nextValue1.text = displayData.nextValue1.ToString();

        limitPanel.SetActive(false);
        btn.interactable = true;
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
