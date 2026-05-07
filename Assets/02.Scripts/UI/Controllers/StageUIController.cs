using System;
using System.Collections.Generic;
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
    [SerializeField]
    private EnemyInfoView enemyInfoView;
    [SerializeField]
    private RerollButtonClick terrainRefreshButton;

    [Header("Controllers")]
    [SerializeField]
    private QueueController queueCtr;
    [SerializeField]
    private ItemSlotController itemCtr;
    [SerializeField]
    private WaveEnemyController enemyInfoCtr;

    private TowerGradeUpgradePresenter gradePresenter;
    private TowerActionMenuPresenter actionMenuPresenter;
    private TowerStatUpgradePresernter statPresenter;
    private SessionInfoPresenter sessionInfoPresenter;
    private ItemInfoPresenter itemInfoPresenter;
    private EnemyInfoPresenter enemyInfoPresenter;

    private Tower selectedTower;

    public event Action<Tower, UpgradeType> OnTowerStatUpgrade;
    public event Action<int> onGoldToTowerInterection;
    public event Action OnTerrainRerollClicked;

    private void Awake()
    {
        gradePresenter = new TowerGradeUpgradePresenter(gradeUpgradeView);
        actionMenuPresenter = new TowerActionMenuPresenter(actionMenuView);
        statPresenter = new TowerStatUpgradePresernter(statUpgradeView);
        sessionInfoPresenter = new SessionInfoPresenter(sessionView);
        itemInfoPresenter = new ItemInfoPresenter(itemView);
        enemyInfoPresenter = new EnemyInfoPresenter(enemyInfoView);

        gradePresenter.onClickNormalUpgrade += OnTowerGradeNormalUpgrade;
        gradePresenter.onClickPremiumUpgrade += OnTowerGradePreminumUpgrade;
        gradePresenter.onClickTowerSell += OnClickTowerSell;

        actionMenuPresenter.OnClickMove += OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade += OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade += OnClickStatUpgrade;
        actionMenuPresenter.OnClickTowerMoveToQueueSlot += OnMoveFieldTowerToQueue;

        statPresenter.onClickDamageUpgrade += OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade += OnTowerStatAttackSpeedUpgrade;

        towerCtr.OnTowerSelectCleared += ClearSelection;
        towerCtr.OnTowerSelected += SetSelectedTower;
        towerCtr.OnShowGradeUpgrade += OnClickGradeUpgrade;
        towerCtr.OnShowStatUpgrade += OnClickStatUpgrade;
        towerCtr.OnGoldInterection += OnGoldToTowerIntertion;
        towerCtr.OnFieldTowerMoveToQueueSlot += OnMoveFieldTowerToQueue;

        queueCtr.OnRequestBuildTower += towerCtr.BeginBuildTower;
        queueCtr.OnRemoveTowerFromQueue += RemoveTower;

        itemCtr.OnClickItem += OnClickItemInfo;

        itemInfoPresenter.OnItemSell += OnClickItemSellButton;
        itemInfoPresenter.OnItemSell += itemCtr.SellItem;

        enemyInfoCtr.onClickEnemyInfo += OnClickWaveEnemyInfo;

        terrainRefreshButton.OnClickReroll += OnClickedTerrainRefreshButton;

        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
        enemyInfoPresenter.Hide();
    }

    private void OnDestroy()
    {
        gradePresenter.onClickNormalUpgrade -= OnTowerGradeNormalUpgrade;
        gradePresenter.onClickPremiumUpgrade -= OnTowerGradePreminumUpgrade;
        gradePresenter.onClickTowerSell -= OnClickTowerSell;

        actionMenuPresenter.OnClickMove -= OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade -= OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade -= OnClickStatUpgrade;
        actionMenuPresenter.OnClickTowerMoveToQueueSlot -= OnMoveFieldTowerToQueue;

        statPresenter.onClickDamageUpgrade -= OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade -= OnTowerStatAttackSpeedUpgrade;

        itemInfoPresenter.OnItemSell -= OnClickItemSellButton;
        itemInfoPresenter.OnItemSell -= itemCtr.SellItem;

        towerCtr.OnTowerSelectCleared -= ClearSelection;
        towerCtr.OnTowerSelected -= SetSelectedTower;
        towerCtr.OnShowGradeUpgrade -= OnClickGradeUpgrade;
        towerCtr.OnShowStatUpgrade -= OnClickStatUpgrade;
        towerCtr.OnGoldInterection -= OnGoldToTowerIntertion;
        towerCtr.OnFieldTowerMoveToQueueSlot -= OnMoveFieldTowerToQueue;

        queueCtr.OnRequestBuildTower -= towerCtr.BeginBuildTower;
        queueCtr.OnRemoveTowerFromQueue -= RemoveTower;

        enemyInfoCtr.onClickEnemyInfo -= OnClickWaveEnemyInfo;

        itemCtr.OnClickItem -= OnClickItemInfo;

        terrainRefreshButton.OnClickReroll -= OnClickedTerrainRefreshButton;

        sessionInfoPresenter.UnBindAction();
    }

    public void BindSessionDataManager(RunSessionDataManager getRunSession)
    {
        sessionInfoPresenter.GetRunSessionDatamanager(getRunSession);
    }

    public void SetWaveEnemyInfo(List<WaveEnemyRosterData> data)
    {
        enemyInfoCtr.GetWaveInfo(data);
    }

    public void OnClickedTerrainRefreshButton()
    {
        OnTerrainRerollClicked?.Invoke();
    }

    public void SetTerrainRerollCount(int cnt)
    {
        terrainRefreshButton.SetRerollCnt(cnt);
    }

    public void SetSelectedTower(Tower getTower)
    {
        selectedTower = getTower;
        
        actionMenuPresenter.SetModel(selectedTower);
        gradePresenter.HideModel();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
        enemyInfoPresenter.Hide();
    }

    public void ClearSelection()
    {
        selectedTower = null;
        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
        enemyInfoPresenter.Hide();
    }

    private void OnClickMove()
    {
        if (selectedTower == null)
            return;

        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
        enemyInfoPresenter.Hide();

        towerCtr.SetTowerMoveMode();
    }

    public void OnMoveFieldTowerToQueue()
    {
        if (selectedTower == null)
            return;

        queueCtr.MoveFieldTowerToQueue(selectedTower.TowerUID);
    }

    public void RemoveTower()
    {
        towerCtr.RemoveTower();
    }

    public void OnClickGradeUpgrade(Tower tower)
    {
        if(tower == null) 
            return;

        towerCtr.SetTowerGradeUpgradeMode();
        itemInfoPresenter.Hide();
        statPresenter.Hide();
        gradePresenter.SetModel(tower);
        enemyInfoPresenter.Hide();
    }

    public void OnClickStatUpgrade(Tower tower)
    {
        if (tower == null)
            return;

        gradePresenter.HideModel();
        itemInfoPresenter.Hide();
        statPresenter.SetModel(tower);
        enemyInfoPresenter.Hide();
    }

    public void OnClickItemInfo(ItemData item, int index)
    {
        if (item == null)
            return;

        gradePresenter.HideModel();
        statPresenter.Hide();
        itemInfoPresenter.SetModel(item, index);
        enemyInfoPresenter.Hide();
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
    private void OnClickTowerSell()
    {
        if (towerCtr == null)
            return;

        towerCtr.SellTower();
    }
    private void OnTowerStatDamageUpgrade(Tower tower)
    {
        OnTowerStatUpgrade?.Invoke(tower, UpgradeType.Damge);
        statPresenter.SetModel(tower);
    }

    private void OnTowerStatAttackSpeedUpgrade(Tower tower)
    {
        OnTowerStatUpgrade?.Invoke(tower, UpgradeType.Speed);
        statPresenter.SetModel(tower);
    }

    private void OnClickItemSellButton(int index)
    {
        itemInfoPresenter.Hide();
    }

    private void OnGoldToTowerIntertion(int value)
    {
        onGoldToTowerInterection?.Invoke(value);
    }

    private void OnClickWaveEnemyInfo(WaveEnemyRosterData waveEnemy)
    {
        enemyInfoPresenter.GetModel(waveEnemy);

        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
    }
}
