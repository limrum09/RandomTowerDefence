using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class EnemySpawnInfo
{
    public string enemyUID;
    public int level;
    public int spawnCnt;

    public EnemySpawnInfo(string enemyUID, int level, int spawnCnt)
    {
        this.enemyUID = enemyUID;
        this.level = level;
        this.spawnCnt = spawnCnt;
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
            SetEnemyInfo(enemyRoster[i].enemyUID, enemyRoster[i].enemyLevel, enemyRoster[i].enemyCount);
            // spawn start등 아직 추가할 것들이 많음
        }
    }

    public void SetEnemyInfo(string enemyUID, int level, int spawnCnt)
    {
        EnemySpawnInfo newEnemy = new EnemySpawnInfo(enemyUID, level, spawnCnt);
        spawnEnemeys.Add(newEnemy);
    }

    public void SetInitalized(GridManager getGrid, PathFinder getPath)
    {
        grid = getGrid;
        path = getPath;
        spawnPoint = grid.CellToWorldCenter(grid.SpawnPos.x, grid.SpawnPos.y);
    }

    public void EnemySpawnStart()
    {
        StartCoroutine(StartEnemySpawn(5,2f));
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
            enemyMove.Initialize(grid, path, grid.SpawnPos, grid.GoalPos, enemyObj.MoveSpeed);
        }
    }

    IEnumerator StartEnemySpawn(int spawnCount, float delayTime)
    {
        int cnt = spawnCount;
        while (cnt > 0)
        {
            for(int i = 0; i< spawnEnemeys.Count; i++)
            {
                SpawnOneEnemy(spawnEnemeys[i]);
                OnEnemySpawn?.Invoke();
            }

            cnt--;

            yield return new WaitForSeconds(delayTime);
        }

        OnSpawnEnd?.Invoke();
        yield return null;
    }
}
