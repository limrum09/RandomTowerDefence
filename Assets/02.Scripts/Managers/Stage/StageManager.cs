using System;
using System.Net.NetworkInformation;
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
    private RunSessionDataManager sessionManager;
    private RunEffectDataManager effectDataManager;
    private RunStatUpgradeManager statUpgradeManager;

    public Vector2Int SpawnPos => Grid.SpawnPos;
    public Vector2Int GoalPos => Grid.GoalPos;

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
        }
    }

    private void OnDestroy()
    {
        if(itemCtr != null)
        {
            itemCtr.OnItemAdd -= ItemAddHandler;
            itemCtr.OnItemSell -= ItemSellHander;
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

        if (upgradeData != null)
            return;

        if (type == UpgradeType.Damge)
            DamageStatUpgrade(upgradeData, tower);
        else if(type == UpgradeType.Speed)
        {

        }        
    }

    private void DamageStatUpgrade(TowerSessionUpgradeData upgradeData,Tower tower)
    {
        int cost = upgradeData.baseCost + (upgradeData.increaseCost * statUpgradeManager.GetAtkDamageStep(tower.Type));
        if (sessionManager.SessionState.Gold < cost)
            return;

        sessionManager.AddGold(cost);

        statUpgradeManager.AddStatAtkDamage(tower.Type, 1);
    }

    public void ItemAddHandler(ItemData item)
    {
        if(item.itemOption == ItemOptions.AtkDamageUP)
            statUpgradeManager.AddItemAtkDamage(item.scopeRange, item.value);
        else if(item.itemOption == ItemOptions.AtkSpeedUp)
            statUpgradeManager.AddItemAtkSpeed(item.scopeRange, item.value);
    }

    public void ItemSellHander(ItemData item, int index)
    {
        sessionManager.AddGold(item.salePrice);
    }
}
