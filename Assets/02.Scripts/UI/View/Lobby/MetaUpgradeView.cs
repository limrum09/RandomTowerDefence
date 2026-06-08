using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeView : MonoBehaviour
{
    [Header("Frame")]
    [SerializeField]
    private CanvasGroup canvasGroup;

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

    [Header("Player Data")]
    [SerializeField]
    private TextMeshProUGUI playerLevelText;
    [SerializeField]
    private TextMeshProUGUI metaCurrencyText;

    [Header("Select Option")]
    [SerializeField]
    private MetaUpgradeSelectViewAnim viewAnim;
    [SerializeField]
    private List<MetaUpgradeSelectView> selectViews;

    [Header("Upgrade View")]
    [SerializeField]
    private MetaUpgradeInfoView infoView;

    public event Func<MetaUpgradeTarget, MetaUpgradeType, string, int, int, bool> OnMetaUpgrade;

    private bool isShow = true;
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

        viewAnim.Init();
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

        isShow = true;
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

    private void ShowPlayerProgressData()
    {
        playerLevelText.text = "Lv. " + Managers.Player.GetPlayerLevel().ToString();
        metaCurrencyText.text = Managers.Player.GetCurreny().ToString();
    }

    private async Task SaveOnHideAsync()
    {
        if (Managers.isQuitting)
            return;

        if (Managers.Save == null)
            return;

        try
        {
            await Managers.Save.SavePlayerProgressData();
            await Managers.Save.SaveMetaUpgradeData();
        }
        catch (Exception e)
        {
            Debug.LogError($"Hide Save Failed : {e.Message}");
        }
    }

    public void Show()
    {
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        towersToggleGroup.SetActive(false);

        publicTypeToggle.SetIsOnWithoutNotify(true);
        towerTypeToggle.SetIsOnWithoutNotify(false);

        OnClickPublicToggle();

        ShowPlayerProgressData();
        isShow = true;
    }

    public void Hide()
    {
        if (!isShow)
            return;

        isShow = false;

        canvasGroup.alpha = 0.0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (Managers.isQuitting)
            return;

        if (Managers.Save == null)
            return;

        _ = SaveOnHideAsync();
    }

    public void OnClickPublicToggle()
    {
        for(int i = 2; i < selectViews.Count + 2; i++)
        {
            if(Enum.IsDefined(typeof(MetaUpgradeType), i))
            {
                selectViews[i - 2].gameObject.SetActive(true);
                selectViews[i - 2].SetPublicDataView((MetaUpgradeType)i, i - 2);
            }
            else
            {
                selectViews[i - 2].gameObject.SetActive(false);
            }
        }

        infoView.SetPublicInfo((MetaUpgradeType)2, MetaUpgradeTarget.Public, 2);
        viewAnim.ChangedToggle();
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
        viewAnim.ChangedToggle();
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
    public void MetaUpgrade(MetaUpgradeTarget getType, MetaUpgradeType upgradeType, string uid, int upgradeCost, int value, int getIndex)
    {
        Debug.Log($"업그레이드 시작 - 타입 : {getType}, 종류 : {upgradeType}, UID : {uid}, 비용 : {upgradeCost}, 값 : {value}");
        bool complete = OnMetaUpgrade?.Invoke(getType, upgradeType, uid, upgradeCost, value) ?? false;

        Debug.Log("업그레이드 결과 : " + complete);

        if (complete)
        {
            Debug.Log("업그레이드 완료 : " + getType);
            if (getType == MetaUpgradeTarget.Tower)
            {
                Debug.Log("타워 업그레이드 완료 : " + upgradeType);
                selectViews[getIndex].TowerUIRefresh();
                infoView.RefreshTowerInfo();
            }
                
            else if (getType == MetaUpgradeTarget.Public)
            {
                Debug.Log("공용 업그레이드 완료 : " + upgradeType);
                selectViews[getIndex].PublicUIRefresh();
                infoView.RefreshPublicInfo();
            }

            ShowPlayerProgressData();
            Managers.Save.MarkMetaUpgradeDirty();
            Managers.Save.MarkPlayerDirty();
        }
    }

    public void ChangedResearchLevel()
    {
        for(int i = 0; i < selectViews.Count; i++)
        {
            selectViews[i].ChangedReserchLevel();
        }
    }
}
