using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeView : MonoBehaviour
{
    [Header("Frame")]
    [SerializeField]
    private GameObject frame;

    [Header("User Info")]
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI metaCurrenyText;

    [Header("Option Toggles")]
    [SerializeField]
    private Toggle publicTypeToggle;
    [SerializeField]
    private Toggle towerTypeToggle;

    [Header("Tower Toggles")]
    [SerializeField]
    private Toggle humanToggle;
    [SerializeField]
    private Toggle elfToggle;
    [SerializeField]
    private Toggle orcToggle;
    [SerializeField]
    private Toggle dwarfToggle;
    [SerializeField]
    private Toggle dragonianToggle;
    [SerializeField]
    private Toggle wereBeastToggle;

    [Header("Object")]
    [SerializeField]
    private GameObject towersToggleGroup;

    [Header("Select Option")]
    [SerializeField]
    private List<MetaUpgradeSelectView> selectViews;

    [Header("Upgrade View")]
    [SerializeField]
    private MetaUpgradeInfoView infoView;

    public event Func<MetaUpgradeTarget, MetaUpgradeType, string, int, bool> OnMetaUpgrade;

    private void Awake()
    {
        BindTowerToggle(humanToggle, TowerType.Human);
        BindTowerToggle(elfToggle, TowerType.Elf);
        BindTowerToggle(orcToggle, TowerType.Orc);
        BindTowerToggle(dwarfToggle, TowerType.Dwarf);
        BindTowerToggle(dragonianToggle, TowerType.Dragonian);
        BindTowerToggle(wereBeastToggle, TowerType.Werebeast);

        for(int i = 0; i< selectViews.Count; i++)
        {
            selectViews[i].SetOwner(this);
        }

        infoView.SetOwner(this);
        towerTypeToggle.onValueChanged.AddListener(isOn =>
        {
            towersToggleGroup.SetActive(isOn);

            if (isOn)
            {
                humanToggle.isOn = isOn;
                OnClickTowerToggle(TowerType.Human);
            }
        });

        publicTypeToggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn)
                return;
            OnClickPublicToggle();
        });

        HIde();
    }

    private void OnEnable()
    {
        publicTypeToggle.isOn = true;
        OnClickPublicToggle();
        towersToggleGroup.SetActive(false);
    }
    private void BindTowerToggle(Toggle toggle, TowerType type)
    {
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn)
                return;

            OnClickTowerToggle(type);
        });
    }

    public void Show()
    {
        frame.gameObject.SetActive(true);
    }
    public void HIde()
    {
        frame.gameObject.SetActive(false);
    }

    public void OnClickPublicToggle()
    {
        for(int i = 0; i < selectViews.Count; i++)
        {
            if(Enum.IsDefined(typeof(MetaUpgradeType), i))
            {
                selectViews[i].gameObject.SetActive(true);
                selectViews[i].SetPublicDataView((MetaUpgradeType)i, i);
            }
            else
            {
                selectViews[i].gameObject.SetActive(false);
            }
        }

        infoView.SetPublicInfo((MetaUpgradeType)0, MetaUpgradeTarget.Public, 0);
    }
    public void OnClickTowerToggle(TowerType type)
    {
        List<TowerData> datas = Managers.TowerData.GetTowerData(type);

        int cnt = datas.Count > selectViews.Count ? selectViews.Count : datas.Count;

        for(int i = 0; i < cnt; i++)
        {
            selectViews[i].gameObject.SetActive(true);
            selectViews[i].SetTowerDataView(datas[i], MetaUpgradeTarget.Tower, i);
        }

        infoView.SetTowerInfo(datas[0], MetaUpgradeTarget.Tower, 0);
    }
    public void OnClickSelectButton(string getUid, MetaUpgradeTarget type, int getIndex)
    {
        if (type == MetaUpgradeTarget.Tower)
        {
            TowerData data = Managers.TowerData.GetTowerData(getUid);
            infoView.SetTowerInfo(data, type, getIndex);
        }
    }
    public void OnClickSelectButton(MetaUpgradeType getMetaType, MetaUpgradeTarget type, int getIndex)
    {
        if(type == MetaUpgradeTarget.Public)
        {
            infoView.SetPublicInfo(getMetaType, type, getIndex);
        }
    }
    public void MetaUpgrade(MetaUpgradeTarget getType, MetaUpgradeType upgradeType, string uid, int value, int getIndex)
    {
        bool complete = OnMetaUpgrade?.Invoke(getType, upgradeType, uid, value) ?? false;

        if (complete)
        {
            if (getType == MetaUpgradeTarget.Tower)
            {
                selectViews[getIndex].TowerUIRefresh();
                infoView.RefreshTowerInfo();
            }
                
            else if (getType == MetaUpgradeTarget.Public)
            {
                selectViews[getIndex].PublicUIRefresh();
                infoView.RefreshPublicInfo();
            }
        }
    }
}
