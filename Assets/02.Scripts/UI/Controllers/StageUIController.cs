using System;
using UnityEngine;

public class StageUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TowerController towerCtr;
    [SerializeField]
    private TowerGradeUpgradeView gradeUpgradeView;
    [SerializeField]
    private TowerStatUpgradeView statUpgradeView;
    [SerializeField]
    private TowerActionMenuView actionMenuView;
    [SerializeField]
    private SessionInfoView sessionView;
    [SerializeField]
    private ItemInfoView itemView;

    [Header("Controllers")]
    [SerializeField]
    private QueueController queueCtr;
    [SerializeField]
    private ItemSlotController itemCtr;

    private TowerGradeUpgradePresenter gradePresenter;
    private TowerActionMenuPresenter actionMenuPresenter;
    private TowerStatUpgradePresernter statPresenter;
    private SessionInfoPresenter sessionInfoPresenter;
    private ItemInfoPresenter itemInfoPresenter;

    private Tower selectedTower;

    public event Action<Tower, UpgradeType> OnTowerStatUpgrade;

    private void Awake()
    {
        gradePresenter = new TowerGradeUpgradePresenter(gradeUpgradeView);
        actionMenuPresenter = new TowerActionMenuPresenter(actionMenuView);
        statPresenter = new TowerStatUpgradePresernter(statUpgradeView);
        sessionInfoPresenter = new SessionInfoPresenter(sessionView);
        itemInfoPresenter = new ItemInfoPresenter(itemView);

        gradePresenter.onClickNormalUpgrade += OnTowerGradeNormalUpgrade;
        gradePresenter.onClickPremiumUpgrade += OnTowerGradePreminumUpgrade;

        actionMenuPresenter.OnClickMove += OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade += OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade += OnClickStatUpgrade;

        statPresenter.onClickDamageUpgrade += OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade += OnTowerStatAttackSpeedUpgrade;

        towerCtr.OnTowerSelectCleared += ClearSelection;
        towerCtr.OnTowerSelected += SetSelectedTower;
        towerCtr.OnShowGradeUpgrade += OnClickGradeUpgrade;
        towerCtr.OnShowStatUpgrade += OnClickStatUpgrade;

        queueCtr.BindTowerController(towerCtr);

        itemCtr.OnClickItem += OnClickItemInfo;

        itemInfoPresenter.OnItemSell += OnClickItemSellButton;
        itemInfoPresenter.OnItemSell += itemCtr.SellItem;

        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
    }

    private void OnDestroy()
    {
        gradePresenter.onClickNormalUpgrade -= OnTowerGradeNormalUpgrade;
        gradePresenter.onClickPremiumUpgrade -= OnTowerGradePreminumUpgrade;

        actionMenuPresenter.OnClickMove -= OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade -= OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade -= OnClickStatUpgrade;

        statPresenter.onClickDamageUpgrade -= OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade -= OnTowerStatAttackSpeedUpgrade;

        itemInfoPresenter.OnItemSell -= OnClickItemSellButton;
        itemInfoPresenter.OnItemSell -= itemCtr.SellItem;

        towerCtr.OnTowerSelectCleared -= ClearSelection;
        towerCtr.OnTowerSelected -= SetSelectedTower;
        towerCtr.OnShowGradeUpgrade -= OnClickGradeUpgrade;
        towerCtr.OnShowStatUpgrade -= OnClickStatUpgrade;

        itemCtr.OnClickItem -= OnClickItemInfo;

        sessionInfoPresenter.UnBindAction();
    }

    public void BindSessionDataManager(RunSessionDataManager getRunSession)
    {
        sessionInfoPresenter.GetRunSessionDatamanager(getRunSession);
    }

    public void SetSelectedTower(Tower getTower)
    {
        selectedTower = getTower;
        
        actionMenuPresenter.SetModel(selectedTower);
        gradePresenter.HideModel();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
    }

    public void ClearSelection()
    {
        selectedTower = null;
        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
    }

    private void OnClickMove()
    {
        if (selectedTower == null)
            return;

        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
        itemInfoPresenter.Hide();

        towerCtr.SetTowerMoveMode();
    }

    public void OnClickGradeUpgrade(Tower tower)
    {
        if(tower == null) 
            return;

        itemInfoPresenter.Hide();
        statPresenter.Hide();
        gradePresenter.SetModel(tower);
    }

    public void OnClickStatUpgrade(Tower tower)
    {
        if (tower == null)
            return;

        gradePresenter.HideModel();
        itemInfoPresenter.Hide();
        statPresenter.SetModel(tower);
    }

    public void OnClickItemInfo(ItemData item, int index)
    {
        if (item == null)
            return;

        gradePresenter.HideModel();
        statPresenter.Hide();
        itemInfoPresenter.SetModel(item, index);
    }

    private void OnTowerGradeNormalUpgrade()
    {
        if (towerCtr == null)
            return;

        towerCtr.TowerGradeNormalUpgrade();
    }

    private void OnTowerGradePreminumUpgrade()
    {
        if (towerCtr == null)
            return;

        towerCtr.TowerGradePreminumUpgrade();
    }

    private void OnTowerStatDamageUpgrade(Tower tower)
    {
        OnTowerStatUpgrade?.Invoke(tower, UpgradeType.Damge);
    }

    private void OnTowerStatAttackSpeedUpgrade(Tower tower)
    {
        OnTowerStatUpgrade?.Invoke(tower, UpgradeType.Speed);
    }

    private void OnClickItemSellButton(int index)
    {
        itemInfoPresenter.Hide();
    }
}
