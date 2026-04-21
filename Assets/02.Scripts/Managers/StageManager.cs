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

    [Header("Stage UI Controller")]
    [SerializeField]
    private StageUIController stageUICtr;

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

    public Vector2Int SpawnPos => Grid.SpawnPos;
    public Vector2Int GoalPos => Grid.GoalPos;

    public RunSessionDataManager RunSession => sessionManager;
    public StageUIController StageUICtr => stageUICtr;

    private void Awake()
    {
        Grid = new GridManager();
        Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);

        sessionManager = new RunSessionDataManager();
        sessionManager.Init(1, life, increaseGold, increaseFreeRollCnt, increaseFreeObstacleCnt);

        Path = new PathFinder(Grid);
    }
    void Start()
    {
        Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);
        SetSettings();

        StageUICtr.BindSessionDataManager(sessionManager);
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
}
