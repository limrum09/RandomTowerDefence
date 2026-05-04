using Assets.PixelFantasy.PixelTileEngine.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    public string enemyUID;
    public int spawnOrder;
    public int level;
    public int spawnCnt;
    public float startTime;
    public float spawnInterval;
    public EnemySpawnInfo(string enemyUID, int spawnOrder, int level, int spawnCnt, float startTime, float spawnInterval)
    {
        this.enemyUID = enemyUID;
        this.spawnOrder = spawnOrder;
        this.level = level;
        this.spawnCnt = spawnCnt;
        this.startTime = startTime;
        this.spawnInterval = spawnInterval;
    }
}
/// <summary>
/// 적 스폰 관리
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    private Enemy baseEnemy;
    [SerializeField]
    private List<EnemySpawnInfo> spawnEnemeys = new List<EnemySpawnInfo>();
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float spawnDelay = 1.0f;

    private GridManager grid;
    private PathFinder path;
    private Vector3 spawnPoint;

    public event Action OnEnemySpawn;
    public event Action OnSpawnEnd;
    public event Action OnEnemyReached;
    public event Action<int> OnEnemyDead;

    private void Start()
    {
        spawnEnemeys.Clear();
    }
    public void SetSpawnEnemyInfo(List<WaveEnemyRosterData> enemyRoster)
    {
        spawnEnemeys.Clear();

        for (int i = 0; i < enemyRoster.Count; i++)
        {
            EnemySpawnInfo newEnemy = new EnemySpawnInfo(enemyRoster[i].enemyUID,enemyRoster[i].spawnOrder, enemyRoster[i].enemyLevel,
                enemyRoster[i].enemyCount, enemyRoster[i].startTime, enemyRoster[i].spawnInterval);
            // spawn start등 아직 추가할 것들이 많음

            spawnEnemeys.Add(newEnemy);
        }
    }

    public void SetInitalized(GridManager getGrid, PathFinder getPath)
    {
        grid = getGrid;
        path = getPath;
        spawnPoint = grid.CellToWorldCenter(grid.SpawnPos.x, grid.SpawnPos.y);
    }

    public void EnemySpawnStart()
    {
        StartCoroutine(StartWave());
    }

    private void EnemyReachGoal() => OnEnemyReached?.Invoke();
    private void EnemyDead(int rewardGold) => OnEnemyDead?.Invoke(rewardGold);

    private void SpawnOneEnemy(EnemySpawnInfo spawnInfo)
    {
        Enemy enemyObj = Instantiate(baseEnemy, spawnPoint, Quaternion.identity);
        enemyObj.Init(spawnInfo.enemyUID, spawnInfo.level);

        EnemyMove enemyMove = enemyObj.GetComponent<EnemyMove>();
        enemyMove.onReachGoal += EnemyReachGoal;
        enemyMove.onDead += EnemyDead;

        if (enemyMove != null)
        {
            enemyMove.Initialize(grid, path, enemyObj, grid.SpawnPos, grid.GoalPos);
        }

        OnEnemySpawn?.Invoke();
    }

    IEnumerator StartWave()
    {
        spawnEnemeys.Sort((x,y) => x.spawnOrder.CompareTo(y.spawnOrder));
        float waveStartTime = Time.time;

        foreach(EnemySpawnInfo info in spawnEnemeys)
        {
            float targetStartTime = waveStartTime + info.startTime;
            float waitTime = targetStartTime - Time.time;

            if(waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(StartEnemySpawn(info));
        }

        OnSpawnEnd?.Invoke();
        yield return null;
    }

    private IEnumerator StartEnemySpawn(EnemySpawnInfo info)
    {
        for(int i = 0; i < info.spawnCnt; i++)
        {
            SpawnOneEnemy(info);

            yield return new WaitForSeconds(info.spawnInterval);
        }
    }
}
