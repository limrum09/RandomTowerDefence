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
    [SerializeField]
    private StageCombatTextController stageCombatTextCtr;
    [SerializeField]
    private GameOverUIController gameOver;

    [Header("Info Panel Animtors")]
    [SerializeField]
    private InfoPanelController coverPanelAnimator;
    [SerializeField]
    private InfoPanelController itemPanelAnimator;
    [SerializeField]
    private InfoPanelController enemyPanelAnimtor;
    [SerializeField]
    private InfoPanelController towerStatPanelAnimator;
    [SerializeField]
    private InfoPanelController towerGradePanelAnimator;

    private TowerGradeUpgradePresenter gradePresenter;
    private TowerStatUpgradePresenter statPresenter;    
    private ItemInfoPresenter itemInfoPresenter;
    private EnemyInfoPresenter enemyInfoPresenter;
    private TowerActionMenuPresenter actionMenuPresenter;
    private SessionInfoPresenter sessionInfoPresenter;

    private Tower selectedTower;
    private InfoPanelController currentInfoPanel;
    private bool isInfoPanelTransitioning;
    private StageInfoPanelType currentInfoPanelType = StageInfoPanelType.None;

    public event Func<GoldChangedReason, int, bool> OnGoldToTowerInterection;
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

        HideDetailViewsImmediate();
        LoadSceneManager.Instance.NotifySceneUIReady();
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
        statPresenter = new TowerStatUpgradePresenter(statUpgradeView);
        sessionInfoPresenter = new SessionInfoPresenter(sessionView);
        itemInfoPresenter = new ItemInfoPresenter(itemView);
        enemyInfoPresenter = new EnemyInfoPresenter(enemyInfoView);
    }

    #region Bind UIs
    private void BindTowerUI()
    {
        gradePresenter.onClickNormalUpgrade += OnTowerGradeNormalUpgrade;
        gradePresenter.onClickPremiumUpgrade += OnTowerGradePremiumUpgrade;
        gradePresenter.onClickTowerSell += OnClickTowerSell;

        actionMenuPresenter.OnClickMove += OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade += OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade += OnClickStatUpgrade;
        actionMenuPresenter.OnClickTowerMoveToQueueSlot += OnMoveFieldTowerToQueue;

        statPresenter.onClickDamageUpgrade += OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade += OnTowerStatAttackSpeedUpgrade;

        towerCtr.OnTowerSelectCleared += ClearSelection;
        towerCtr.OnSelectTowerRemove += ClearRemoveTowerSelection;
        towerCtr.OnTowerSelected += SetSelectedTower;
        towerCtr.OnShowGradeUpgrade += OnClickGradeUpgrade;
        towerCtr.OnShowStatUpgrade += OnClickStatUpgrade;
        towerCtr.OnGoldInteraction += OnGoldToTowerIntertion;
        towerCtr.OnFieldTowerMoveToQueueSlot += OnMoveFieldTowerToQueue;
    }

    private void BindQueueUI()
    {
        towerCtr.OnQueueTowerBuildSuccess += queueCtr.RemoveTower;

        queueCtr.OnRequestBuildTower += towerCtr.BeginBuildTower;
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
        gradePresenter.onClickPremiumUpgrade -= OnTowerGradePremiumUpgrade;
        gradePresenter.onClickTowerSell -= OnClickTowerSell;

        actionMenuPresenter.OnClickMove -= OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade -= OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade -= OnClickStatUpgrade;
        actionMenuPresenter.OnClickTowerMoveToQueueSlot -= OnMoveFieldTowerToQueue;

        statPresenter.onClickDamageUpgrade -= OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade -= OnTowerStatAttackSpeedUpgrade;

        towerCtr.OnTowerSelectCleared -= ClearSelection;
        towerCtr.OnSelectTowerRemove -= ClearRemoveTowerSelection;
        towerCtr.OnTowerSelected -= SetSelectedTower;
        towerCtr.OnShowGradeUpgrade -= OnClickGradeUpgrade;
        towerCtr.OnShowStatUpgrade -= OnClickStatUpgrade;
        towerCtr.OnGoldInteraction -= OnGoldToTowerIntertion;
        towerCtr.OnFieldTowerMoveToQueueSlot -= OnMoveFieldTowerToQueue;
    }

    private void UnBindQueueUI()
    {
        towerCtr.OnQueueTowerBuildSuccess -= queueCtr.RemoveTower;

        queueCtr.OnRequestBuildTower -= towerCtr.BeginBuildTower;
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
    private void HideDetailViewsImmediate()
    {
        gradePresenter.HideModel();
        statPresenter.Hide();
        itemInfoPresenter.Hide();
        enemyInfoPresenter.Hide();
        actionMenuPresenter.Hide();

        HideAllInfoPanels();
    }

    private void HideDetailViews()
    {
        actionMenuPresenter.Hide();
        HideCurrentInfoPanel();
    }

    private void HideAllInfoPanels()
    {
        itemPanelAnimator.Hide();
        enemyPanelAnimtor.Hide();
        towerStatPanelAnimator.Hide();
        towerGradePanelAnimator.Hide();
        coverPanelAnimator.Hide();

        currentInfoPanel = null;
        currentInfoPanelType = StageInfoPanelType.None;
    }

    private void HideCurrentInfoPanel()
    {
        if (isInfoPanelTransitioning)
            return;

        if (currentInfoPanel == null)
            return;

        isInfoPanelTransitioning = true;

        InfoPanelController prev = currentInfoPanel;

        currentInfoPanel = null;
        currentInfoPanelType = StageInfoPanelType.None;

        coverPanelAnimator.PlayCoverClose(() =>
        {
            prev.Hide();
            isInfoPanelTransitioning = false;
        });
    }
    #endregion

    private InfoPanelController GetInfoPanelAnimator(StageInfoPanelType type)
    {
        switch (type)
        {
            case StageInfoPanelType.Item:
                return itemPanelAnimator;
            case StageInfoPanelType.Enemy:
                return enemyPanelAnimtor;
            case StageInfoPanelType.TowerStatUpgrade:
                return towerStatPanelAnimator;
            case StageInfoPanelType.TowerGradeUpgrade:
                return towerGradePanelAnimator;
            case StageInfoPanelType.Cover:
                return coverPanelAnimator;
            default:
                return null;
        }
    }

    private void ShowInfoPanel(StageInfoPanelType type, Action setModel, bool forceRefresh = false)
    {
        InfoPanelController target = GetInfoPanelAnimator(type);
         

        if (target == null)
            return;

        if (currentInfoPanel == target && !forceRefresh)
            return;

        bool isFirstOpen = currentInfoPanel == null;

        isInfoPanelTransitioning = true;

        if (isFirstOpen)
        {
            currentInfoPanel = target;
            currentInfoPanelType = type;

            setModel?.Invoke();

            coverPanelAnimator.PlayCoverOpen(
                onArrived: () =>
                {
                    target.Show();
                },
                onCompleted: () =>
                {
                    isInfoPanelTransitioning = false;
                }
            );

            return;
        }

        InfoPanelController prev = currentInfoPanel;

        currentInfoPanel = target;
        currentInfoPanelType = type;

        if(prev == target)
        {
            target.PlayNextPageNoFillout(() =>
            {
                target.Show();
                setModel?.Invoke();
                isInfoPanelTransitioning = false;
            });

            return;
        }

        target.Show();
        setModel?.Invoke();

        prev.PlayNextPage(() =>
        {
            isInfoPanelTransitioning = false;
        });
    }

    private void OnClickGradeUpgrade(Tower tower)
    {
        if (tower == null)
            return;

        towerCtr.SetTowerGradeUpgradeMode();

        ShowInfoPanel(StageInfoPanelType.TowerGradeUpgrade, () => gradePresenter.SetModel(tower));
    }

    private void OnClickStatUpgrade(Tower tower)
    {
        if (tower == null)
            return;

        ShowInfoPanel(StageInfoPanelType.TowerStatUpgrade, () => statPresenter.SetModel(tower));
    }

    private void OnClickItemInfo(ItemData item, int index)
    {
        if (item == null)
            return;

        actionMenuPresenter.Hide();

        ShowInfoPanel(StageInfoPanelType.Item, () => itemInfoPresenter.SetModel(item, index), true);
    }

    private void OnClickWaveEnemyInfo(EnemyResolveInfo waveEnemy)
    {
        if (waveEnemy == null)
            return;

        actionMenuPresenter.Hide();

        ShowInfoPanel(StageInfoPanelType.Enemy, () => enemyInfoPresenter.GetModel(waveEnemy), true);
    }

    private void SetSelectedTower(Tower getTower)
    {
        selectedTower = getTower;

        HideDetailViews();
        actionMenuPresenter.SetModel(selectedTower);
    }

    private void ClearRemoveTowerSelection()
    {
        selectedTower = null;
        actionMenuPresenter.Hide();
        HideDetailViews();
    }

    private void ClearSelection()
    {
        selectedTower = null;
        actionMenuPresenter.Hide();

        if (Managers.InputData.IsPointerOverUI<TowerUIRaycastTarget>())
            return;

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

        if (!queueCtr.HasEmptySlot())
            return;

        string uid = selectedTower.TowerUID;

        if (!towerCtr.RemoveTower())
            return;

        if (!queueCtr.AddTower(uid))
        {
            Debug.LogError($"대기열로 타워 이동 실패");
        }
    }

    private void OnTowerGradeNormalUpgrade()
    {
        if (towerCtr == null)
            return;

        towerCtr.TowerGradeNormalUpgrade();
    }

    private void OnTowerGradePremiumUpgrade()
    {
        if (towerCtr == null)
            return;

        towerCtr.TowerGradePremiumUpgrade();
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

    private void OnClickedTerrainRefreshButton()
    {
        OnTerrainRerollClicked?.Invoke();
    }

    private void OnRequestSellItem(int index)
    {
        ItemData item = itemCtr.GetItem(index);

        if (item == null)
            return;

        itemInfoPresenter.Hide();
        OnRequestItemSell?.Invoke(item, index);
    }

    private bool OnGoldToTowerIntertion(GoldChangedReason reason, int value)
    {
        return OnGoldToTowerInterection?.Invoke(reason, value) ?? false;
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

    public void SetCombatText(string text)
    {
        stageCombatTextCtr.SetText(text);
    }

    public void ShowGameOver(StageResultData data)
    {
        gameOver.SetGameOverUI(data);
    }

    public int GetConvertQueueTowerToGold()
    {
        return queueCtr.GameOverCovertTowerToGold();
    }

    public void MoveToLobby()
    {
        OnMoveToLobby?.Invoke();
    }
}
