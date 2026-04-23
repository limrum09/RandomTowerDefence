using System;
using Unity.VisualScripting;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Action OnAfterSettingsInit;
    public Action OnStageStart;

    [Header("Grid Settings")]
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize = 2f;

    [Header("Map Settings")]
    [SerializeField]
    private Transform mapPlane;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Transform goalPoint;

    [Header("Controllers")]
    [SerializeField]
    private StageUIController stageUICtr;
    [SerializeField]
    private ItemSlotController itemCtr;
    [SerializeField]
    private TowerCntSkillInfoController towerCntSkill;

    [Header("Temp Settings Value")]
    [SerializeField]
    private int increaseGold;
    [SerializeField]
    private int life;
    [SerializeField]
    private int increaseFreeRollCnt;
    [SerializeField]
    private int increaseFreeObstacleCnt;

    private bool isSpawning;
    private bool isStagePlaying;
    private int aliveEnemyCnt;


    public GridManager Grid { get; private set; }
    public PathFinder Path { get; private set; }
    private FieldTowerManager fieldTowerManager;
    private RunSessionDataManager sessionManager;
    private RunEffectDataManager effectDataManager;
    private RunStatUpgradeManager statUpgradeManager;

    public Vector2Int SpawnPos => Grid.SpawnPos;
    public Vector2Int GoalPos => Grid.GoalPos;

    public FieldTowerManager FieldTowerManager => fieldTowerManager;
    public RunSessionDataManager RunSession => sessionManager;
    public RunEffectDataManager EffectDataManager => effectDataManager;
    public RunStatUpgradeManager StatUpgradeManager => statUpgradeManager;
    public StageUIController StageUICtr => stageUICtr;

    private void Awake()
    {
        Grid = new GridManager();
        Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);

        sessionManager = new RunSessionDataManager();
        sessionManager.Init(1, life, increaseGold, increaseFreeRollCnt, increaseFreeObstacleCnt);

        effectDataManager = new RunEffectDataManager();
        statUpgradeManager = new RunStatUpgradeManager();

        fieldTowerManager = new FieldTowerManager();
        fieldTowerManager.Init(Grid);

        statUpgradeManager.Init();
        effectDataManager.Init(sessionManager, statUpgradeManager);

        Path = new PathFinder(Grid);
    }
    void Start()
    {
        SetSettings();

        if(itemCtr != null)
        {
            itemCtr.OnItemAdd += ItemAddHandler;
            itemCtr.OnItemSell += ItemSellHander;
        }

        if(stageUICtr != null)
        {
            StageUICtr.BindSessionDataManager(sessionManager);
            stageUICtr.OnTowerStatUpgrade += TowerStatUpgradeHandler;
            stageUICtr.onGoldToTowerInterection += UsingGold;
        }

        if(towerCntSkill != null)
        {
            towerCntSkill.BindManager(fieldTowerManager);
        }
    }

    private void OnDestroy()
    {
        if(itemCtr != null)
        {
            itemCtr.OnItemAdd -= ItemAddHandler;
            itemCtr.OnItemSell -= ItemSellHander;
        }

        if (stageUICtr != null)
        {
            stageUICtr.OnTowerStatUpgrade -= TowerStatUpgradeHandler;
            stageUICtr.onGoldToTowerInterection -= UsingGold;
        }
    }

    public void StageStart()
    {
        if (isStagePlaying)
            return;

        isStagePlaying = true;
        isSpawning = true;
        aliveEnemyCnt = 0;

        OnStageStart?.Invoke();
    }

    private void AfterSettingsInit()
    {
        OnAfterSettingsInit?.Invoke();
    }

    public void SetSettings()
    {
        spawnPoint.position = Grid.CellToWorldCenter(SpawnPos.x, SpawnPos.y);
        goalPoint.position = Grid.CellToWorldCenter(GoalPos.x, GoalPos.y);

        AfterSettingsInit();
    }

    private void CheckWaveEnd()
    {
        if (isSpawning)
            return;

        if (aliveEnemyCnt > 0)
            return;

        if(sessionManager.SessionState.CurrentLife <= 0)
        {
            UserDead();
            return;
        }

        isStagePlaying = false;
    }
    public void EnemySpawnEnd()
    {
        isSpawning = false;
    }

    public void RegisterSpawnEnemy()
    {
        aliveEnemyCnt++;
    }

    public void RegisterDeadEnemy()
    {
        aliveEnemyCnt--;
        CheckWaveEnd();
    }

    public void RegisterReachedEnemy()
    {
        aliveEnemyCnt--;
        sessionManager.ChangeLife(-1);
        CheckWaveEnd();
    }

    public void UserDead()
    {

    }

    public void TowerStatUpgradeHandler(Tower tower, UpgradeType type)
    {
        TowerSessionUpgradeData upgradeData = Managers.SessionTowerUpgrade.GetUpgradeStepData(tower.TowerUID, type);

        if (upgradeData == null)
            return;

        if (type == UpgradeType.Damge)
            DamageStatUpgrade(upgradeData, tower);
        else if(type == UpgradeType.Speed)
            SpeedStatUpgrade(upgradeData, tower);
    }

    private void DamageStatUpgrade(TowerSessionUpgradeData upgradeData,Tower tower)
    {
        int cost = upgradeData.baseCost + (upgradeData.increaseCost * statUpgradeManager.GetAtkDamageStep(tower.Type));
        if (sessionManager.SessionState.Gold < cost)
            return;

        sessionManager.ChangeGold(cost);

        statUpgradeManager.AddStatAtkDamage(tower.Type, 1);
    }

    private void SpeedStatUpgrade(TowerSessionUpgradeData upgradeData, Tower tower)
    {
        int cost = upgradeData.baseCost + (upgradeData.increaseCost * statUpgradeManager.GetAtkSpeedStep(tower.Type));
        if (sessionManager.SessionState.Gold < cost)
            return;

        sessionManager.ChangeGold(cost);

        statUpgradeManager.AddStatAtkSpeed(tower.Type, 1);
    }

    public void UsingGold(int value)
    {
        sessionManager.ChangeGold(value);
    }

    public void ItemAddHandler(ItemData item)
    {
        effectDataManager.ApplyItemEffect(item);
        sessionManager.ChangeGold(item.buyPrice);
    }

    public void ItemSellHander(ItemData item, int index)
    {
        effectDataManager.RemoveItemEffect(item);
        sessionManager.ChangeGold(item.salePrice);
    }
}
