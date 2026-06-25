using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스테이지 진행을 총괄하는 루트 관리자
/// Grid, Path, 세션 데이터, 웨이브, 적 스폰, 타워 강화, 아이템 효과, 장애물 비용 처리를 연결한다
/// 각 시스템의 세부 로직을 직접 수행하지 않고 스테이지 안에서 발생하는 이벤트를 연결하고 흐름을 제어한다.
/// </summary>
public class StageManager : MonoBehaviour
{
    public event Action OnAfterSettingsInit;
    public event Action OnWaveStart;
    public event Action OnWaveEndRefreshStore;
    public event Action<int> OnChangedGameSpeed;
    public event Action<StageResultData> OnGameOver;
    public event Action<string> OnCombatText;

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
    private ObstacleBuilder obstacleBuilder;
    [SerializeField]
    private DrawGridLine line;

    [Header("UIs")]
    [SerializeField]
    private StageUIController stageUICtr;
    [SerializeField]
    private ItemSlotUIController itemCtr;
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

    private bool isResetTerrain;
    private bool isSpawning;
    private bool isStagePlaying;
    private bool isGameOver;
    private bool isGameEnd;
    private bool isReportedStageEndAchievement;
    private int enemyKillGold;
    private int aliveEnemyCnt;
    private int waveTotalEnemyCount;
    private int waveRemoveEnemyCount;
    private int currentWave;
    private int speedIndex;
    private int[] speedValue = { 1, 2, 3, 4 };
    private int maxBuildTowerCount;


    public GridManager Grid { get; private set; }
    public PathFinder Path { get; private set; }
    private FieldTowerManager fieldTowerManager;
    private RunSessionDataManager sessionManager;
    private RunEffectDataManager effectDataManager;
    private RunStatUpgradeManager statUpgradeManager;
    private TowerSkillEffect towerSkillEffect;
    private EnemyFactory factory;

    public Vector2Int SpawnPos => Grid.SpawnPos;
    public Vector2Int GoalPos => Grid.GoalPos;

    public FieldTowerManager FieldTowerManager => fieldTowerManager;
    public RunSessionDataManager RunSession => sessionManager;
    public RunEffectDataManager EffectDataManager => effectDataManager;
    public RunStatUpgradeManager StatUpgradeManager => statUpgradeManager;
    public StageUIController StageUICtr => stageUICtr;
    public int Wave => currentWave;

    /// <summary>
    /// 런타임 매니저들을 생성하고 초기화
    /// </summary>
    private void Awake()
    {
        Grid = new GridManager();
        Path = new PathFinder();
        sessionManager = new RunSessionDataManager();
        effectDataManager = new RunEffectDataManager();
        statUpgradeManager = new RunStatUpgradeManager();
        fieldTowerManager = new FieldTowerManager();
        towerSkillEffect = new TowerSkillEffect();

        TowerStatCalculator.Init(statUpgradeManager);

        Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);
        fieldTowerManager.Init(Grid);
        Path.Init(Grid);

        int startGold = GetPublicMetaValue(MetaUpgradeType.StartingGold);
        int freeObstacle = GetPublicMetaValue(MetaUpgradeType.FreeObstacle);
        int terrainRefresh = GetPublicMetaValue(MetaUpgradeType.FreeTerrainRefresh);
        enemyKillGold = GetPublicMetaValue(MetaUpgradeType.DropGold, 10);

        sessionManager.Init(1, life, startGold + increaseGold, increaseFreeRollCnt,
            freeObstacle + increaseFreeObstacleCnt, terrainRefresh + 5);
        
        towerSkillEffect.Init();
        statUpgradeManager.Init();
        effectDataManager.Init(this, sessionManager, statUpgradeManager);
    }

    /// <summary>
    /// 필요한 이벤트 연결 및 초기 웨이브, 라인, 지형 상태를 설정
    /// </summary>
    void Start()
    {
        enemySpawn.SetInitialized(Grid, Path);

        BindEvents();

        currentWave = 1;
        isResetTerrain = true;
        isGameOver = false;
        isGameEnd = false;
        isReportedStageEndAchievement = false;
        speedIndex = 0;
        waveTotalEnemyCount = 0;
        waveRemoveEnemyCount = 0;

        waveManager.Init(Managers.Game.selectDifficultyLevel + "_W001");
        sessionManager.SetWave(currentWave);
        line.Init(gridWidth, gridHeight, cellSize, mapPlane.transform.position);
        towerCntSkill.Init(sessionManager, FieldTowerManager);

        AfterSettingsInit();
        LoadSceneManager.Instance.NotifySceneManagerReady();
    }

    /// <summary>
    /// 파괴시 연결된 이벤트들 해제
    /// </summary>
    private void OnDestroy()
    {
        UnBindEvents();
    }

    /// <summary>
    /// 스테이지 내부 시스템들의 이벤트 열결
    /// </summary>
    #region Bind Events
    private void BindEvents()
    {
        BindStatUpgradeEvents();
        BindGridEvents();
        BindItemEvents();
        BindUIEvents();
        BindFieldEvents();
        BindSessionEvents();
        BindEnemySpawnEvents();
        BindTowerSkillEvents();
        BindObstacleEvents();
    }

    private void BindSessionEvents()
    {
        if(sessionManager != null)
        {
            sessionManager.OnLevelChanged += towerCntSkill.ChangeUserLevel;
        }
    }

    private void BindStatUpgradeEvents()
    {
        statUpgradeManager.OnChangedTowerStat += fieldTowerManager.RefreshAllTowerDamageStats;
    }

    private void BindGridEvents()
    {
        if (Grid != null)
        {
            Grid.OnSetSpawnAndGoalPoint += SetSpawnAndGoalPointSetting;
            Grid.OnSetSpawnAndGoalPoint += sessionManager.TerrainRoll;
        }
    }

    private void BindItemEvents()
    {
        if (itemCtr != null)
        {
            itemCtr.OnItemAdd += ItemAddHandler;
        }
    }

    private void BindUIEvents()
    {
        if (stageUICtr != null)
        {
            StageUICtr.BindSessionDataManager(sessionManager);
            stageUICtr.OnTowerStatUpgrade += TowerStatUpgradeHandler;
            stageUICtr.OnGoldToTowerInterection += UsingGold;
            stageUICtr.OnTerrainRerollClicked += TerrainRefreshHandler;
            stageUICtr.OnRequestItemSell += ItemSellHandler;
            stageUICtr.OnClickAccelerateButton += OnClickAccelerateButton;
            stageUICtr.OnStageContinue += StageContinueHideOption;
            stageUICtr.OnStagePause += ShowOptionPanel;
            stageUICtr.OnMoveToLobby += MoveToLobby;
            OnCombatText += stageUICtr.SetCombatText;
            OnChangedGameSpeed += stageUICtr.ChangeGameSpeed;
            OnGameOver += stageUICtr.ShowGameOver;
        }
    }

    private void BindFieldEvents()
    {
        if (fieldTowerManager != null)
        {
            fieldTowerManager.OnFieldTowerChanged += towerCntSkill.ChangeFieldTower;
            fieldTowerManager.OnFieldTowerChanged += TowerSkillCountChanged;
        }
    }

    private void BindEnemySpawnEvents()
    {
        if (enemySpawn != null)
        {
            OnWaveStart += enemySpawn.EnemySpawnStart;
            enemySpawn.OnSpawnEnd += EnemySpawnEnd;

            factory = enemySpawn.EnemyFactory;

            factory.OnEnemySummon += RegisterSummonEnemy;
            factory.OnEnemySpawn += RegisterSpawnEnemy;
            factory.OnEnemyReached += RegisterReachedEnemy;
            factory.OnEnemyDead += RegisterDeadEnemy;
        }
    }

    private void BindTowerSkillEvents()
    {
        if (towerSkillEffect != null)
        {
            towerSkillEffect.OnChangedTowerSkillStep += TowerSkillStepChangeHandler;
        }
    }

    private void BindObstacleEvents()
    {
        if (obstacleBuilder != null)
        {
            obstacleBuilder.OnRequestUseFreeObstacle += UseFreeObstacleHandler;
            obstacleBuilder.OnRequestPayObstacleCost += PayObstacleCostHandler;
            obstacleBuilder.OnRequestRefundObstacleCost += RefundObstacleCostHandler;
            obstacleBuilder.OnRequestRestoreFreeObstacle += RestoreFreeObstacleHandler;
        }
    }
    #endregion

    /// <summary>
    /// 스테이지 내부 시스템들의 이벤트 연결 해제
    /// </summary>
    #region UnBind Events
    private void UnBindEvents()
    {
        UnBindStatUpgradeEvents();
        UnBindGridEvents();
        UnBindItemEvents();
        UnBindUIEvents();
        UnBindFieldEvents();
        UnBindSessionEvents();
        UnBindEnemySpawnEvents();
        UnBindTowerSkillEvents();
        UnBindObstacleEvents();
    }

    private void UnBindSessionEvents()
    {
        if (sessionManager != null)
        {
            sessionManager.OnLevelChanged -= towerCntSkill.ChangeUserLevel;
        }
    }
    private void UnBindStatUpgradeEvents()
    {
        statUpgradeManager.OnChangedTowerStat -= fieldTowerManager.RefreshAllTowerDamageStats;
    }

    private void UnBindGridEvents()
    {
        if (Grid != null)
        {
            Grid.OnSetSpawnAndGoalPoint -= SetSpawnAndGoalPointSetting;
            Grid.OnSetSpawnAndGoalPoint -= sessionManager.TerrainRoll;
        }

    }

    private void UnBindItemEvents()
    {
        if (itemCtr != null)
        {
            itemCtr.OnItemAdd -= ItemAddHandler;
        }
    }

    private void UnBindUIEvents()
    {
        if (stageUICtr != null)
        {
            stageUICtr.OnTowerStatUpgrade -= TowerStatUpgradeHandler;
            stageUICtr.OnGoldToTowerInterection -= UsingGold;
            stageUICtr.OnTerrainRerollClicked -= TerrainRefreshHandler;
            stageUICtr.OnRequestItemSell -= ItemSellHandler;
            stageUICtr.OnClickAccelerateButton -= OnClickAccelerateButton;
            stageUICtr.OnStageContinue -= StageContinueHideOption;
            stageUICtr.OnStagePause -= ShowOptionPanel;
            stageUICtr.OnMoveToLobby -= MoveToLobby;
            OnCombatText -= stageUICtr.SetCombatText;
            OnChangedGameSpeed -= stageUICtr.ChangeGameSpeed;
            OnGameOver -= stageUICtr.ShowGameOver;
        }
    }

    private void UnBindFieldEvents()
    {
        if (fieldTowerManager != null)
        {
            fieldTowerManager.OnFieldTowerChanged -= towerCntSkill.ChangeFieldTower;
            fieldTowerManager.OnFieldTowerChanged -= TowerSkillCountChanged;
        }
    }

    private void UnBindEnemySpawnEvents()
    {
        if (enemySpawn != null)
        {
            OnWaveStart -= enemySpawn.EnemySpawnStart;
            enemySpawn.OnSpawnEnd -= EnemySpawnEnd;
        }

        if(factory != null)
        {
            factory.OnEnemySpawn -= RegisterSpawnEnemy;
            factory.OnEnemySummon -= RegisterSummonEnemy;
            factory.OnEnemyReached -= RegisterReachedEnemy;
            factory.OnEnemyDead -= RegisterDeadEnemy;
        }
    }

    private void UnBindTowerSkillEvents()
    {
        if (towerSkillEffect != null)
        {
            towerSkillEffect.OnChangedTowerSkillStep -= TowerSkillStepChangeHandler;
        }
    }

    private void UnBindObstacleEvents()
    {
        if (obstacleBuilder != null)
        {
            obstacleBuilder.OnRequestUseFreeObstacle -= UseFreeObstacleHandler;
            obstacleBuilder.OnRequestPayObstacleCost -= PayObstacleCostHandler;
            obstacleBuilder.OnRequestRefundObstacleCost -= RefundObstacleCostHandler;
            obstacleBuilder.OnRequestRestoreFreeObstacle -= RestoreFreeObstacleHandler;
        }
    }
    #endregion

    /// <summary>
    /// 가속 버튼 클릭
    /// </summary>
    private void OnClickAccelerateButton()
    {
        speedIndex++;

        if (speedIndex >= speedValue.Length)
            speedIndex = 0;

        Time.timeScale = speedValue[speedIndex];

        OnChangedGameSpeed?.Invoke(speedValue[speedIndex]);
    }

    /// <summary>
    /// 옵션 패널이 보여지면서 게임 일시 중지
    /// </summary>
    private void ShowOptionPanel()
    {
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// 옵션 패널이 숨겨지고, 게임 계속하기
    /// </summary>
    private void StageContinueHideOption()
    {
        StartCoroutine("RestoreTimeScale");
    }

    private void MoveToLobby()
    {
        StopAllCoroutines();

        Time.timeScale = 1f;
        speedIndex = 0;

        isStagePlaying = false;
        isSpawning = false;
        aliveEnemyCnt = 0;

        LoadSceneManager.Instance.OnLoadStringScene("LobbyScene");
    }

    /// <summary>
    /// 지형 새로고침을 한다음 UI 상태 변경
    /// </summary>
    private void TerrainRefreshHandler()
    {
        stageUICtr.SetTerrainRerollCount(ResetTerrain());
    }

    /// <summary>
    /// 적 스폰 지점과 적 목표 지점 다시 정하기
    /// 게임이 시작하지 않고, 새로고침 가능 횟수가 1이상일 경우 동작
    /// </summary>
    /// <returns>지형 변환이 가능한 횟수 반환</returns>
    private int ResetTerrain()
    {
        if (!isResetTerrain)
            return 0;

        if (sessionManager.SessionState.CurrentLife <= 0)
            return 0;

        Grid.SetRerollPoints();

        if (sessionManager.SessionState.TerrainRollCnt <= 0)
        {
            isResetTerrain = false;
        }

        return sessionManager.SessionState.TerrainRollCnt;
    }

    /// <summary>
    /// 스폰 지점과 목표 지점의 월드 위치를 Grid 좌표 기준으로 갱신하여 이동
    /// </summary>
    private void SetSpawnAndGoalPointSetting()
    {
        spawnPoint.position = Grid.CellToWorldCenter(SpawnPos.x, SpawnPos.y);
        goalPoint.position = Grid.CellToWorldCenter(GoalPos.x, GoalPos.y);
    }

    /// <summary>
    /// 스테이지 초기 설정이 끝난 뒤, 각 시스템에 초기 데이터를 전달하고 초기 UI를 갱신
    /// </summary>
    private void AfterSettingsInit()
    {
        stageUICtr.SetTerrainRerollCount(sessionManager.SessionState.TerrainRollCnt);
        SetSpawnAndGoalPointSetting();
        PrepareWaveInit();
        OnChangedGameSpeed?.Invoke(speedValue[speedIndex]);
        obstacleBuilder.Initialized(Grid, Path, fieldTowerManager);
        OnAfterSettingsInit?.Invoke();
    }

    private int GetPublicMetaValue(MetaUpgradeType type, int fallback = 0)
    {
        StageStartOptionBaseData baseData = Managers.StartOption.GetStartOptionData(type);
        MetaResearchUpgradeData metaData = Managers.ResearchUpgrade.GetMetaResearchDataToPublic(MetaUpgradeTarget.Public, type);

        if(baseData == null || metaData == null)
            return fallback;

        int level = Managers.PublicMetaUpgrade.GetPublicMetaDataLevel(type);
        float value = metaData.CalculateValue(baseData.baseValue, level);

        return Mathf.Max(0, Mathf.FloorToInt(value));
    }

    /// <summary>
    /// 시작 시, 웨이브 값 호출
    /// </summary>
    private void PrepareWaveInit()
    {
        WavePrepareResult result = waveManager.TryPrepareNextWave();

        if (result.IsFailed)
        {
            WaveDataLoadFailed(result.message);
            return;
        }

        if (result.IsEnd)
        {
            ClearStage();
            return;
        }

        stageUICtr.SetWaveEnemyInfo(result.rosterData);
        enemySpawn.SetSpawnEnemyInfo(result.rosterData);
        SetWaveRosterData(result.rosterData);
    }

    /// <summary>
    /// 현재 웨이브가 종료 가능한 상태인지 확인
    /// 적 스폰이 끝나고 생존 적이 없다면 다음 웨이브로 진행
    /// </summary>
    private void CheckWaveEnd()
    {
        if (!CanCompleteWave())
            return;

        CompleteCurrentWave();
    }

    /// <summary>
    /// 웨이브를 완료 할수 있는지 검수
    /// </summary>
    /// <returns></returns>
    private bool CanCompleteWave()
    {
        if (isSpawning)
            return false;

        if (aliveEnemyCnt > 0)
            return false;

        if (sessionManager.SessionState.CurrentLife <= 0)
        {
            UserDead();
            return false;
        }

        if (isGameOver)
            return false;

        return true;
    }

    /// <summary>
    /// 현재 웨이브를 완료 했다면 호출
    /// </summary>
    private void CompleteCurrentWave()
    {
        isStagePlaying = false;

        WavePrepareResult result = waveManager.TryPrepareNextWave();

        if (result.IsFailed)
        {
            WaveDataLoadFailed(result.message);
            return;
        }

        if (result.IsEnd)
        {
            ClearStage();
            return;
        }

        AdvanceWave(result.rosterData);
        GiveWaveClearReward();
        RefreshStoreOnWaveClear();
    }

    /// <summary>
    /// 스테이지를 완료 하였을 때 호출
    /// </summary>
    private void ClearStage()
    {
        Managers.QuestMgr.QuestRecieveReport(QuestCategory.ClearStage, Managers.Game.clearStageTargetUID, 1);

        GameEnd();
    }

    /// <summary>
    /// 다음 웨이브를 준비
    /// </summary>
    /// <param name="rosterDatas"></param>
    private void AdvanceWave(List<WaveEnemyRosterData> rosterDatas)
    {
        currentWave++;

        waveRemoveEnemyCount = 0;
        waveTotalEnemyCount = 0;

        sessionManager.SetWave(currentWave);

        stageUICtr.SetWaveEnemyInfo(rosterDatas);
        enemySpawn.SetSpawnEnemyInfo(rosterDatas);
        SetWaveRosterData(rosterDatas);
    }

    /// <summary>
    /// 웨이브 완료 시, 완료 경험치 지급
    /// </summary>
    private void GiveWaveClearReward()
    {
        if(!Managers.StageRules.GetRuleData(StageRules.WaveClearRewardExp, out int exp))
        {
            Debug.LogError("Wave clear reward exp missing");
            return;
        }

        RunSession.AddExp(exp);
        GetInterestGold();
    }

    private void RefreshStoreOnWaveClear()
    {
        OnWaveEndRefreshStore?.Invoke();
    }

    /// <summary>
    /// 웨이브 종료 시, 획득하는 이자율 계산
    /// </summary>
    private void GetInterestGold()
    {
        if(!Managers.StageRules.GetRuleData(StageRules.WaveClearInterestRate, out int baseInterest))
            return;

        if(!Managers.StageRules.GetRuleData(StageRules.MaxInterestGold, out int baseMaxInterest))
            return;

        float increaseInterest = statUpgradeManager.IncreaseInterserPer;
        float totalInterestRate = baseInterest + increaseInterest;

        int currentGold = sessionManager.SessionState.Gold;
        int interest = Mathf.FloorToInt(currentGold * (totalInterestRate / 100f));

        float maxInterestRate = statUpgradeManager.MaxInterestValue;
        int maxInterest = Mathf.FloorToInt(baseMaxInterest * (1f + maxInterestRate / 100f));

        interest = Mathf.Clamp(interest, 0, maxInterest);

        if(interest > 0)
            UsingGold(GoldChangedReason.GAIN, interest);
    }

    /// <summary>
    /// 현제 적 처시지 획득하는 드랍 골드를 계산
    /// </summary>
    /// <returns></returns>
    private int GetcurrentEnemyKillGold()
    {
        return Mathf.Max(0, enemyKillGold + statUpgradeManager.GoldDropValue);
    }

    /// <summary>
    /// 적 스폰이 모두 끝났음을 기록
    /// 이후 행존 적 수가 0이 되면 웨이브 종료가 가능해짐
    /// </summary>
    private void EnemySpawnEnd()
    {
        isSpawning = false;
        CheckWaveEnd();
    }

    /// <summary>
    /// 적이 생성될 때, 생존 적 수를 증가
    /// </summary>
    private void RegisterSpawnEnemy()
    {
        aliveEnemyCnt++;
    }

    /// <summary>
    /// 적이 사망 했을 때, 보상 골드와 킬 카운트를 지급하고 생존 적 수를 감소
    /// </summary>
    /// <param name="reward">처치 보상 골드</param>
    private void RegisterDeadEnemy()
    {
        UsingGold(GoldChangedReason.KILL, GetcurrentEnemyKillGold());
        sessionManager.AddkillCount(1);
        aliveEnemyCnt--;

        WaveRemoveEnemy();
        CheckWaveEnd();
    }

    /// <summary>
    /// 적이 목표 지점에 도달햇을 때 생존 적 수와 현재 라이프를 감소
    /// </summary>
    private void RegisterReachedEnemy()
    {
        aliveEnemyCnt--;
        sessionManager.ChangeLife(-1);

        if(sessionManager.SessionState.CurrentLife <= 0)
        {
            UserDead();
            return;
        }

        WaveRemoveEnemy();
        CheckWaveEnd();
    }

    /// <summary>
    /// WaveManager가 다음 Wave의 데이터를 가져오지 못했을 때 호출`
    /// </summary>
    /// <param name="message"></param>
    private void WaveDataLoadFailed(string message)
    {
        Debug.LogError(message);

        isSpawning = false;
        isStagePlaying = false;
        isGameOver = true;

        if (enemySpawn != null)
            enemySpawn.GameOver();
    }

    /// <summary>
    /// 해당 웨이브의 적 정보와 스폰 개수들을 저장
    /// </summary>
    /// <param name="datas"></param>
    private void SetWaveRosterData(List<WaveEnemyRosterData> datas)
    {
        waveTotalEnemyCount = 0;
        waveRemoveEnemyCount = 0;

        for(int i = 0; i < datas.Count; i++)
        {
            waveTotalEnemyCount += datas[i].enemyCount;
        }

        sessionManager.RemainEnemyCount(waveTotalEnemyCount - waveRemoveEnemyCount, waveTotalEnemyCount);
    }

    /// <summary>
    /// 적이 죽거나, 도착지점에 도달했을 때 동작
    /// </summary>
    private void WaveRemoveEnemy()
    {
        waveRemoveEnemyCount++;
        sessionManager.RemainEnemyCount(waveTotalEnemyCount - waveRemoveEnemyCount, waveTotalEnemyCount);
    }

    private void RegisterSummonEnemy(int spwanCnt)
    {
        waveTotalEnemyCount += spwanCnt;
        sessionManager.RemainEnemyCount(waveTotalEnemyCount - waveRemoveEnemyCount, waveTotalEnemyCount);
    }

    /// <summary>
    /// 타워 새션 강화 요청 처리
    /// 강화 타입에 따라 공격력 강화 또는 공격속도 강화로 분류
    /// </summary>
    /// <param name="tower">스탯을 강화할 타워</param>
    /// <param name="type">업그레이드할 타입. 공격력과 공격속도가 있음</param>
    private void TowerStatUpgradeHandler(Tower tower, UpgradeType type)
    {
        TowerSessionUpgradeData upgradeData = Managers.SessionTowerUpgrade.GetUpgradeStepData(tower.TowerUID, type);

        if (upgradeData == null)
            return;

        if (type == UpgradeType.Damge)
            DamageStatUpgrade(upgradeData, tower);
        else if(type == UpgradeType.Speed)
            SpeedStatUpgrade(upgradeData, tower);
    }

    /// <summary>
    /// 타워 보유 수 조건에 따라 발동된 종족별 타워 스킬 단계를 처리
    /// </summary>
    /// <param name="type">개수가 변화한 타워 타입</param>
    /// <param name="step">변화된 타워 개수 스탭</param>
    /// <param name="value">타워 개수 변화에 의한 변경된 스킬의 값</param>
    private void TowerSkillStepChangeHandler(TowerType type,int step, float value)
    {
        if (TowerType.Orc == type && step == 4)
        {
            statUpgradeManager.AddSkillEnemySlowPer(type, value);
            return;
        }
        else if(TowerType.Orc == type && step != 4)
        {
            statUpgradeManager.AddSkillEnemySlowPer(type, 0f);
        }

            switch (type)
            {
                case TowerType.Elf:
                case TowerType.Orc:
                case TowerType.Dragonian:
                    statUpgradeManager.AddSkillAtkDamage(type, (int)value);
                    break;
                case TowerType.Human:
                    statUpgradeManager.SetIncreseInterest((int)value);
                    break;
                case TowerType.Werebeast:
                    statUpgradeManager.AddSkillCriticalPer(type, value);
                    break;
            }
    }

    /// <summary>
    /// 스킬 사용에 필요한 타워 개수를 줄이는 아이템 획득, 판매 시
    /// 요구사항을 줄이도록 계산하지 않고, 현재 타워의 개수를 증가 시키는 것으로 계산
    /// </summary>
    /// <param name="type"></param>
    /// <param name="count"></param>
    private void TowerSkillCountChanged(TowerType type, int count)
    {
        int effectCount = GetEffectiveTowerSkillCount(count);
        towerSkillEffect.ChangeTowerCount(type, effectCount);
    }

    /// <summary>
    /// 아이템이 있다면 해당 값을 더해서 반환
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    private int GetEffectiveTowerSkillCount(int count)
    {
        int request = statUpgradeManager.AbilityTriggerRequest;
        return Mathf.Max(0, count - request);
    }

    /// <summary>
    /// 모든 타워 스킬의 단계를 재설정
    /// </summary>
    private void RefreshAllTowerSkill()
    {
        foreach(TowerType type in Enum.GetValues(typeof(TowerType)))
        {
            int realCount = fieldTowerManager.GetTowerCount(type);
            TowerSkillCountChanged(type, realCount);
        }
    }

    /// <summary>
    /// 공격력 세션 강화 비용을 확인하고, 골드가 충분하면 해당 타입의 타워의 공격력 강화 단계를 증가
    /// </summary>
    /// <param name="upgradeData">타워 업그레이드 세션 데이터</param>
    /// <param name="tower">강화한 타워</param>
    private void DamageStatUpgrade(TowerSessionUpgradeData upgradeData,Tower tower)
    {
        int cost = upgradeData.baseCost + (upgradeData.increaseCost * statUpgradeManager.GetAtkDamageStep(tower.Type));
        if (sessionManager.SessionState.Gold < cost)
            return;

        if (!UsingGold(GoldChangedReason.UPGRADE, -cost))
            return;
            

        statUpgradeManager.AddStatAtkDamage(tower.Type, 1);
    }

    /// <summary>
    /// 공격속도 세션 강화 비용을 확인하고, 골드가 충분하다면 해당 타입의 타워의 공격속도 강화 단계 증가
    /// </summary>
    /// <param name="upgradeData">타워 업그레이드 세션 데이터</param>
    /// <param name="tower">강화할 타입의 타워</param>
    private void SpeedStatUpgrade(TowerSessionUpgradeData upgradeData, Tower tower)
    {
        int cost = upgradeData.baseCost + (upgradeData.increaseCost * statUpgradeManager.GetAtkSpeedStep(tower.Type));
        if (sessionManager.SessionState.Gold < cost)
            return;

        if (!UsingGold(GoldChangedReason.UPGRADE, -cost))
            return;        

        statUpgradeManager.AddStatAtkSpeed(tower.Type, 1);
    }

    /// <summary>
    /// 아이템 획득 시 아이템 효과를 적용하고 구매 비용을 처리
    /// </summary>
    /// <param name="item">획득한 아이템의 데이터</param>
    private void ItemAddHandler(ItemData item)
    {
        effectDataManager.ApplyItemEffect(item);
        RefreshAllTowerSkill();
    }

    /// <summary>
    /// 아이템 판매 요청을 처리
    /// 아이템 효과를 제거하고 판매 골드를 지급한 뒤 슬롯에서 아이템을 제거
    /// </summary>
    /// <param name="item">제거할 아이템 데이터</param>
    /// <param name="index">아이템이 위치한 인벤토리 슬롯의 위치</param>
    private void ItemSellHandler(ItemData item, int index)
    {
        if (item == null)
            return;

        effectDataManager.RemoveItemEffect(item);
        RefreshAllTowerSkill();

        UsingGold(GoldChangedReason.SELL, item.salePrice);
        itemCtr.RemoveItem(index);
    }

    /// <summary>
    /// 장애물 설치 시 무료 장애물 설치권 사용 처리
    /// </summary>
    /// <returns></returns>
    private bool UseFreeObstacleHandler()
    {
        return sessionManager.UsingFreeObstacle();
    }

    /// <summary>
    /// 장애물 설치 비용을 골드로 지불 확인
    /// </summary>
    /// <param name="cost">지불할 비용</param>
    /// <returns>비용이 부족하다면 실패하고 false를 리턴한다</returns>
    private bool PayObstacleCostHandler(int cost)
    {
        return UsingGold(GoldChangedReason.BUILD, -cost);
    }

    /// <summary>
    /// 유로로 설치한 장애물 제거 시 설치 비용을 환급
    /// </summary>
    /// <param name="cost">환급받을 비용</param>
    private void RefundObstacleCostHandler(int cost)
    {
        UsingGold(GoldChangedReason.REMOVE, cost);
    }

    /// <summary>
    /// 무료 설치권으로 설치한 장애물 제거 시 무료 설치권을 복구
    /// </summary>
    private void RestoreFreeObstacleHandler()
    {
        sessionManager.GetFreeObstacle(1);
    }

    private void SetScoreCalculate()
    {
        StageResultData stageResult = new StageResultData();

        stageResult.stageLevel = Managers.Game.selectDifficultyLevel;
        stageResult.clearWave = RunSession.SessionState.CurrentWave;
        stageResult.currentLife = RunSession.SessionState.CurrentLife;
        stageResult.remainGold = RunSession.SessionState.Gold;
        stageResult.queueGold = stageUICtr.GetConvertQueueTowerToGold();

        stageResult.towers = fieldTowerManager.GetTowerResultData();
        stageResult.items = itemCtr.GetItemResultData();

        stageResult = Managers.ScoreCal.ScoreCalculator(stageResult);

        GiveStageResultReward(stageResult);

        OnGameOver?.Invoke(stageResult);
    }

    private void GiveStageResultReward(StageResultData result)
    {
        if (result == null)
            return;

        if (result.rewardCurrency <= 0)
            return;

        Managers.Player.AddCurrency(result.rewardCurrency);
        Managers.Save.MarkPlayerDirty();

        _ = Managers.Save.SavePlayerProgressData();
    }


    private void ReportStageEndAchievement()
    {
        if (isReportedStageEndAchievement)
            return;

        isReportedStageEndAchievement = true;

        foreach(TowerType type in Enum.GetValues(typeof(TowerType)))
        {
            ReportBuildTowerAchievement(type);
        }
    }

    private void ReportBuildTowerAchievement(TowerType type)
    {
        int cnt = fieldTowerManager.GetTowerCount(type);

        if (cnt <= 0)
            return;

        Managers.QuestMgr.QuestRecieveReport(QuestCategory.BuildTower, $"BUILD_TOWER_{type.ToString().ToUpper()}", cnt);
    }

    /// <summary>
    /// 일시정지가 풀리고 난 후, 특정 시간동안 속도가 천천히 돌아온다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RestoreTimeScale()
    {
        float duration = 3.5f;
        float elapsed = 0f;
        float targetSpeed = speedValue[speedIndex];

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(0f, targetSpeed, elapsed / duration);

            yield return null;
        }

        Time.timeScale = targetSpeed;
    }

    /// <summary>
    /// 웨이브 시작 요청을 처리
    /// 이미 웨이브가 진행 중이면 시작하지 않음
    /// 웨이브 시작이 가능하면 적 스폰을 시작
    /// </summary>
    public void WaveStart()
    {
        if (isGameOver || sessionManager.SessionState.CurrentLife <= 0)
            return;

        if (isStagePlaying)
        {
            CheckWaveEnd();
            Debug.Log("웨이브 시작 불가능");
            Debug.Log(isSpawning);
            Debug.Log(aliveEnemyCnt);
            Debug.Log(sessionManager.SessionState.CurrentLife <= 0);
            return;
        }

        isResetTerrain = false;
        isStagePlaying = true;
        isSpawning = true;
        aliveEnemyCnt = 0;

        OnWaveStart?.Invoke();
    }

    /// <summary>
    /// 첫 타워설치 성공 이후 지형 리롤을 비활성화하고 UI를 갱신
    /// </summary>
    public void SuccessBuildTower()
    {
        isResetTerrain = false;
        stageUICtr.SetTerrainRerollCount(ResetTerrain());
    }

    /// <summary>
    /// 유저 패배 상태를 처리
    /// 현재는 웨이브 값을 0으로 처리하여 웨이브를 진행 못하게 막음
    /// </summary>
    public void UserDead()
    {
        isGameOver = true;
        currentWave = 0;

        enemySpawn.GameOver();

        GameEnd();
    }

    public void GameEnd()
    {
        if (isGameEnd)
            return;

        isGameEnd = true;
        ReportStageEndAchievement();

        isGameOver = true;
        Invoke(nameof(SetScoreCalculate), 1f);
    }

    /// <summary>
    /// 골드를 변경
    /// 양수는 골드 획득, 음수는 골드 소비이며 성공 여부를 반환
    /// </summary>
    /// <param name="value">변경될 골드 값</param>
    /// <returns></returns>
    public bool UsingGold(GoldChangedReason reason, int value)
    {
        bool isCompleted = sessionManager.ChangeGold(value);

        if (isCompleted)
        {
            string reasonText = Managers.Local.GetString("UI", $"UI_GOLD_CHANGED_REASON_{reason.ToString()}");
            string setText = $"{reasonText} {value.ToString("+0;-0")}";
            OnCombatText?.Invoke(setText);
        }
        else
        {
            string ressonText = Managers.Local.GetString("UI", "UI_GOLD_LACK");
            OnCombatText?.Invoke(ressonText);
        }

            return isCompleted;
    }
}
