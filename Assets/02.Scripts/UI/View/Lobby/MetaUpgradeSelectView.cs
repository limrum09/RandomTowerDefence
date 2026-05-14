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
    private MetaUpgradeTarget upgradeType;
    private MetaUpgradeType publicType;
    private int index;

    private void Start()
    {
        btn.onClick.AddListener(OnClickSelectButton);
    }

    public void SetOwner(MetaUpgradeView getOwner) => Owner = getOwner;

    public void TowerUIRefresh()
    {
        upgradeFrame1.SetActive(true);
        upgradeFrame2.SetActive(true);
        title.text = $"{tower.grade}등급 {Managers.TowerData.GetTowerNameType(tower.towerType)}타워";
        icon.sprite = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{tower.towerType}_{tower.grade}_Idle");
        info.text = "타워 공격력 및 공격속도 영구강화";

        upgradeText1.text = "공격 속도";
        upgradeText2.text = "공격력";

        int damageLevel = Managers.TowerMetaUpgrade.GetDamageLevel(tower.towerType, tower.grade);
        int speedLevel = Managers.TowerMetaUpgrade.GetAttackSpeedLevel(tower.towerType, tower.grade);

        currentValue1.text = (tower.baseAtkSpeed + (2.0f * speedLevel)).ToString();
        currentValue2.text = (tower.baseAtk + (2.0f * damageLevel)).ToString();

        nextValue1.text = (tower.baseAtkSpeed + (2.0f * (speedLevel + 1))).ToString();
        nextValue2.text = (tower.baseAtk + (2.0f * (damageLevel + 1))).ToString();
    }

    public void SetTowerDataView(TowerData data, MetaUpgradeTarget getUpgradeType, int getIndex)
    {
        ResetDatas();

        tower = data;
        getUID = tower.TowerUID;
        upgradeType = getUpgradeType;
        index = getIndex;

        TowerUIRefresh();
    }

    public void PublicUIRefresh()
    {
        title.text = Managers.PublicMetaUpgrade.GetTypeName(publicType);
        info.text = Managers.PublicMetaUpgrade.GetTypeInfoStr(publicType);
        upgradeText1.text = Managers.PublicMetaUpgrade.GetTypeCountStr(publicType);
        icon.sprite = null;

        int level = Managers.PublicMetaUpgrade.GetPublicMetaDataLevel(publicType);
        currentValue1.text = (600 + (100 * level)).ToString();
        nextValue1.text = (600 + (100 * (level + 1))).ToString();
    }

    public void SetPublicDataView(MetaUpgradeType type, int getIndex)
    {
        upgradeFrame1.SetActive(true);
        upgradeFrame2.SetActive(false);

        ResetDatas();

        upgradeType = MetaUpgradeTarget.Public;
        publicType = type;
        index = getIndex;

        PublicUIRefresh();
    }

    public void OnClickSelectButton()
    {
        if(!string.IsNullOrEmpty(getUID))
            Owner.OnClickSelectButton(getUID, upgradeType, index);
        else
            Owner.OnClickSelectButton(publicType, upgradeType, index);

    }

    private void ResetDatas()
    {
        tower = null;
        getUID = string.Empty;
        upgradeType = MetaUpgradeTarget.Tower;
        index = -1;
        publicType = MetaUpgradeType.StartingGold;
    }
}
