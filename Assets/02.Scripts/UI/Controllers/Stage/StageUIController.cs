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

    [Header("Buttons")]
    [SerializeField]
    private RerollButtonClick terrainRefreshButton;
    [SerializeField]
    private AccelerateButton accelerateButton;

    [Header("Controllers")]
    [SerializeField]
    private QueueUIController queueCtr;
    [SerializeField]
    private ItemSlotUIController itemCtr;
    [SerializeField]
    private WaveEnemyInfoUIController enemyInfoCtr;
    [SerializeField]
    private StageOptionUIController stageOptionCtr;

    private TowerGradeUpgradePresenter gradePresenter;
    private TowerActionMenuPresenter actionMenuPresenter;
    private TowerStatUpgradePresernter statPresenter;
    private SessionInfoPresenter sessionInfoPresenter;
    private ItemInfoPresenter itemInfoPresenter;
    private EnemyInfoPresenter enemyInfoPresenter;

    private Tower selectedTower;

    public event Func<int, bool> OnGoldToTowerInterection;
    public event Action<Tower, UpgradeType> OnTowerStatUpgrade;
    public event Action OnTerrainRerollClicked;
    public event Action<ItemData, int> OnRequestItemSell;
    public event Action OnClickAccelerateButton;
    public event Action OnStagePause;
    public event Action OnStageContinue;
    public event Action OnMoveToLobby;
    private void Awake()
    {
        CreatePresenter();

        BindTowerUI();
        BindQueueUI();
        BindItemUI();
        BindEnemyUI();
        BindRerollUI();
        BindAccelerateUI();
        BindOptionButton();

        HideDetailViews();
    }

    private void OnDestroy()
    {
        UnBindTowerUI();
        UnBindQueueUI();
        UnBindItemUI();
        UnBindEnemyUI();
        UnBindRerollUI();
        UnBindOptionButton();

        sessionInfoPresenter.UnBindAction();
    }

    private void CreatePresenter()
    {
        gradePresenter = new TowerGradeUpgradePresenter(gradeUpgradeView);
        actionMenuPresenter = new TowerActionMenuPresenter(actionMenuView);
        statPresenter = new TowerStatUpgradePresernter(statUpgradeView);
        sessionInfoPresenter = new SessionInfoPresenter(sessionView);
        itemInfoPresenter = new ItemInfoPresenter(itemView);
        enemyInfoPresenter = new EnemyInfoPresenter(enemyInfoView);
    }

    #region Bind UIs
    private void BindTowerUI()
    {
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
    }

    private void BindQueueUI()
    {
        towerCtr.OnQueueTowerBuildSuccess += queueCtr.RemoveTower;

        queueCtr.OnRequestBuildTower += towerCtr.BeginBuildTower;
        queueCtr.OnRemoveTowerFromQueue += RemoveTower;
    }

    private void BindItemUI()
    {
        itemCtr.OnClickItem += OnClickItemInfo;
        itemCtr.OnRequestSellItem += OnRequestSellItem;

        itemInfoPresenter.OnItemSell += itemCtr.RequestSellItem;
    }

    private void BindEnemyUI()
    {
        enemyInfoCtr.onClickEnemyInfo += OnClickWaveEnemyInfo;
    }

    private void BindRerollUI()
    {
        terrainRefreshButton.OnClickReroll += OnClickedTerrainRefreshButton;
    }

    private void BindAccelerateUI()
    {
        accelerateButton.BindButton(OnClickAccelerate);
    }

    private void BindOptionButton()
    {
        OnStagePause += stageOptionCtr.ShowOptionPanel;
        stageOptionCtr.OnStageGameContinue += StageContinue;
        stageOptionCtr.OnMoveToLobby += MoveToLobby;
    }

    #endregion

    #region UnBind UIs
    private void UnBindTowerUI()
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

        towerCtr.OnTowerSelectCleared -= ClearSelection;
        towerCtr.OnTowerSelected -= SetSelectedTower;
        towerCtr.OnShowGradeUpgrade -= OnClickGradeUpgrade;
        towerCtr.OnShowStatUpgrade -= OnClickStatUpgrade;
        towerCtr.OnGoldInterection -= OnGoldToTowerIntertion;
        towerCtr.OnFieldTowerMoveToQueueSlot -= OnMoveFieldTowerToQueue;
    }

    private void UnBindQueueUI()
    {
        towerCtr.OnQueueTowerBuildSuccess -= queueCtr.RemoveTower;

        queueCtr.OnRequestBuildTower -= towerCtr.BeginBuildTower;
        queueCtr.OnRemoveTowerFromQueue -= RemoveTower;
    }
    private void UnBindItemUI()
    {
        itemCtr.OnClickItem -= OnClickItemInfo;
        itemCtr.OnRequestSellItem -= OnRequestSellItem;

        itemInfoPresenter.OnItemSell -= itemCtr.RequestSellItem;
    }
    private void UnBindEnemyUI()
    {
        enemyInfoCtr.onClickEnemyInfo -= OnClickWaveEnemyInfo;
    }

    private void UnBindRerollUI()
    {
        terrainRefreshButton.OnClickReroll -= OnClickedTerrainRefreshButton;
    }

    private void UnBindOptionButton()
    {
        OnStagePause -= stageOptionCtr.ShowOptionPanel;
        stageOptionCtr.OnStageGameContinue -= StageContinue;
        stageOptionCtr.OnMoveToLobby -= MoveToLobby;
    }
    #endregion UnBind UIs

    #region Hide View
    private void HideDetailViews()
    {
        HideTowerDetailInfoView();
        HideItemDetailInfoView();
        HideEnemyDetailInfoView();
    }

    private void HideTowerDetailInfoView()
    {
        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
    }

    private void HideItemDetailInfoView()
    {
        itemInfoPresenter.Hide();
    }

    private void HideEnemyDetailInfoView()
    {
        enemyInfoPresenter.Hide();
    }
    #endregion

    private void OnClickedTerrainRefreshButton()
    {
        OnTerrainRerollClicked?.Invoke();
    }

    private void SetSelectedTower(Tower getTower)
    {
        selectedTower = getTower;

        HideDetailViews();
        actionMenuPresenter.SetModel(selectedTower);
    }

    private void ClearSelection()
    {
        selectedTower = null;
        HideDetailViews();
    }

    private void OnClickMove()
    {
        if (selectedTower == null)
            return;

        HideDetailViews();

        towerCtr.SetTowerMoveMode();
    }

    private void OnMoveFieldTowerToQueue()
    {
        if (selectedTower == null)
            return;

        queueCtr.MoveFieldTowerToQueue(selectedTower.TowerUID);
    }

    private void RemoveTower()
    {
        towerCtr.RemoveTower();
    }

    private void OnClickGradeUpgrade(Tower tower)
    {
        if(tower == null) 
            return;

        towerCtr.SetTowerGradeUpgradeMode();

        HideDetailViews();
        gradePresenter.SetModel(tower);
    }

    private void OnClickStatUpgrade(Tower tower)
    {
        if (tower == null)
            return;

        HideDetailViews();
        statPresenter.SetModel(tower);
    }

    private void OnClickItemInfo(ItemData item, int index)
    {
        if (item == null)
            return;

        HideDetailViews();
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

    private void OnRequestSellItem(int index)
    {
        ItemData item = itemCtr.GetItem(index);

        if (item == null)
            return;

        itemInfoPresenter.Hide();
        OnRequestItemSell?.Invoke(item, index);
    }

    private void OnGoldToTowerIntertion(int value)
    {
        OnGoldToTowerInterection?.Invoke(value);
    }

    private void OnClickWaveEnemyInfo(WaveEnemyRosterData waveEnemy)
    {
        HideDetailViews();
        enemyInfoPresenter.GetModel(waveEnemy);
    }

    private void OnClickAccelerate()
    {
        OnClickAccelerateButton?.Invoke();
    }

    public void BindSessionDataManager(RunSessionDataManager getRunSession)
    {
        sessionInfoPresenter.GetRunSessionDatamanager(getRunSession);
    }

    public void SetWaveEnemyInfo(List<WaveEnemyRosterData> data)
    {
        enemyInfoCtr.GetWaveInfo(data);
    }

    public void SetTerrainRerollCount(int cnt)
    {
        terrainRefreshButton.SetRerollCnt(cnt);
    }

    public void ChangeGameSpeed(int speed)
    {
        accelerateButton.ChangedGameSpeed(speed);
    }

    public void ShowOptions()
    {
        OnStagePause?.Invoke();
    }
    public void StageContinue()
    {
        OnStageContinue?.Invoke();
    }
    public void MoveToLobby()
    {
        OnMoveToLobby.Invoke();
    }
}
