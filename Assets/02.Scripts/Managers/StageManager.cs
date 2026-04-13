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

    private bool isSpawning;
    private bool isStagePlaying;
    private int aliveEnemyCnt;

    public GridManager Grid { get; private set; }
    public PathFinder Path { get; private set; }

    public Vector2Int SpawnPos => Grid.SpawnPos;
    public Vector2Int GoalPos => Grid.GoalPos;

    private void Awake()
    {
        Grid = new GridManager();
        Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);

        Path = new PathFinder(Grid);
    }
    void Start()
    {
        Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);
        SetSettings();
    }
    public void StageStart()
    {
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

    public void StartStage()
    {
        if (isStagePlaying)
            return;

        isStagePlaying = true;
        isSpawning = true;
        aliveEnemyCnt = 0;
    }

    private void CheckWaveEnd()
    {
        if (isSpawning)
            return;

        if (aliveEnemyCnt > 0)
            return;
    }

    public void EnemySpawning()
    {

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
        CheckWaveEnd();
    }
}
