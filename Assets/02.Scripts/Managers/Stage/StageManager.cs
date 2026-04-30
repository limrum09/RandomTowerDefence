using System;
using System.Security.Cryptography;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public event Action OnAfterSettingsInit;
    public event Action OnWaveStart;
    public event Action OnWaveEnd;

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

    [Header("Field Ctrs")]
    [SerializeField]
    private EnemySpawn enemySpawn;
    [SerializeField]
    private StageWaveManager waveManager;
    [SerializeField]
    private DrawGridLine line;
    

    [Header("UIs")]
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
    private int currentWave;


    public GridManager Grid { get; private set; }
    public PathFinder Path { get; private set; }
    private FieldTowerManager fieldTowerManager;
    private RunSessionDataManager sessionManager;
    private RunEffectDataManager effectDataManager;
    private RunStatUpgradeManager statUpgradeManager;
    private TowerSkillEffect towerSkillEffect;

    public Vector2Int SpawnPos => Grid.SpawnPos;
    public Vector2Int GoalPos => Grid.GoalPos;

    public FieldTowerManager FieldTowerManager => fieldTowerManager;
    public RunSessionDataManager RunSession => sessionManager;
    public RunEffectDataManager EffectDataManager => effectDataManager;
    public RunStatUpgradeManager StatUpgradeManager => statUpgradeManager;
    public StageUIController StageUICtr => stageUICtr;

    public int Wave => currentWave;
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

        towerSkillEffect = new TowerSkillEffect();
        towerSkillEffect.Init();

        statUpgradeManager.Init();
        effectDataManager.Init(sessionManager, statUpgradeManager);

        Path = new PathFinder(Grid);
    }
    void Start()
    {
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

        if(fieldTowerManager != null)
        {
            fieldTowerManager.OnFieldTowerChanged += towerCntSkill.ChangeFiledTower;
            fieldTowerManager.OnFieldTowerChanged += towerSkillEffect.ChangeTowerCount;
        }

        if(waveManager != null)
        {
            OnWaveEnd += waveManager.WaveEnd;
            waveManager.onWaveRosterData += StageUICtr.SetWaveEnemyInfo;
        }

        if(enemySpawn != null)
        {
            OnWaveStart += enemySpawn.EnemySpawnStart;
            enemySpawn.OnEnemySpawn += RegisterSpawnEnemy;
            enemySpawn.OnSpawnEnd += EnemySpawnEnd;
            enemySpawn.OnEnemyReached += RegisterReachedEnemy;
            enemySpawn.OnEnemyDead += RegisterDeadEnemy;
            waveManager.onWaveRosterData += enemySpawn.SetSpawnEnemyInfo;
        }

        if(towerSkillEffect != null)
        {
            towerSkillEffect.OnChangedTowerSkillStep += TowerSkillStepChangeHandler;
        }

        currentWave = 1;
        sessionManager.SetWave(currentWave);
        line.Init(gridWidth, gridHeight, cellSize, mapPlane.transform.position);
        SetSettings();
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

        if(fieldTowerManager != null)
        {
            fieldTowerManager.OnFieldTowerChanged -= towerCntSkill.ChangeFiledTower;
            fieldTowerManager.OnFieldTowerChanged -= towerSkillEffect.ChangeTowerCount;
        }

        if(waveManager != null)
        {
            OnWaveEnd -= waveManager.WaveEnd;
            waveManager.onWaveRosterData -= StageUICtr.SetWaveEnemyInfo;
        }
        if (enemySpawn != null)
        {
            OnWaveStart -= enemySpawn.EnemySpawnStart;
            enemySpawn.OnEnemySpawn -= RegisterSpawnEnemy;
            enemySpawn.OnSpawnEnd -= EnemySpawnEnd;
            enemySpawn.OnEnemyReached -= RegisterReachedEnemy;
            enemySpawn.OnEnemyDead -= RegisterDeadEnemy;
            waveManager.onWaveRosterData -= enemySpawn.SetSpawnEnemyInfo;
        }

        if (towerSkillEffect != null)
        {
            towerSkillEffect.OnChangedTowerSkillStep -= TowerSkillStepChangeHandler;
        }
    }

    public void StageStart()
    {
        if (isStagePlaying)
        {
            CheckWaveEnd();
            Debug.Log("웨이브 시작 불가능");
            Debug.Log(isSpawning);
            Debug.Log(aliveEnemyCnt);
            Debug.Log(sessionManager.SessionState.CurrentLife <= 0);
            return;
        }

        isStagePlaying = true;
        isSpawning = true;
        aliveEnemyCnt = 0;

        OnWaveStart?.Invoke();
    }

    private void AfterSettingsInit()
    {
        enemySpawn.SetInitalized(Grid, Path);
        OnWaveEnd?.Invoke(); 
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
        currentWave++;
        sessionManager.SetWave(currentWave);
        RunSession.AddExp(3);
        OnWaveEnd?.Invoke();
    }
    public void EnemySpawnEnd()
    {
        isSpawning = false;
    }

    public void RegisterSpawnEnemy()
    {
        aliveEnemyCnt++;
    }

    public void RegisterDeadEnemy(int reward)
    {
        UsingGold(reward);
        sessionManager.AddkillCount(1);
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
        currentWave = 0;
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

    public void TowerSkillStepChangeHandler(TowerType type,int step, float value)
    {
        if(TowerType.Orc == type && step == 4)
        {
            return;
        }

        switch (type)
        {
            case TowerType.Elf:
            case TowerType.Orc:
            case TowerType.Dragonian:
                statUpgradeManager.AddSkillAtkDamage(type, (int)value);
                break;
            case TowerType.Human:
                HumanTowerSkill();
                break;
            case TowerType.Werebeast:
                WerebeastTowerSkill();
                break;
        }
    }

    private void HumanTowerSkill()
    {

    }

    private void WerebeastTowerSkill()
    {

    }

    private void DamageStatUpgrade(TowerSessionUpgradeData upgradeData,Tower tower)
    {
        int cost = upgradeData.baseCost + (upgradeData.increaseCost * statUpgradeManager.GetAtkDamageStep(tower.Type));
        if (sessionManager.SessionState.Gold < cost)
            return;

        UsingGold(-cost);

        statUpgradeManager.AddStatAtkDamage(tower.Type, 1);
    }

    private void SpeedStatUpgrade(TowerSessionUpgradeData upgradeData, Tower tower)
    {
        int cost = upgradeData.baseCost + (upgradeData.increaseCost * statUpgradeManager.GetAtkSpeedStep(tower.Type));
        if (sessionManager.SessionState.Gold < cost)
            return;

        UsingGold(-cost);

        statUpgradeManager.AddStatAtkSpeed(tower.Type, 1);
    }

    public void UsingGold(int value)
    {
        sessionManager.ChangeGold(value);
    }

    public void ItemAddHandler(ItemData item)
    {
        effectDataManager.ApplyItemEffect(item);
        UsingGold(item.buyPrice);
    }

    public void ItemSellHander(ItemData item, int index)
    {
        effectDataManager.RemoveItemEffect(item);
        UsingGold(item.salePrice);
    }
}
