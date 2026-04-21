using System;
using System.Collections;
using System.Collections.Generic;
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
    private StageManager stage;
    [SerializeField]
    private Enemy baseEnemy;
    [SerializeField]
    private List<EnemySpawnInfo> spawnEnemeys = new List<EnemySpawnInfo>();
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float spawnDelay = 1.0f;

    private GridManager grid;
    private Vector3 spawnPoint;
    void Awake()
    {
        stage.OnStageStart += EnemySpawnStart;
        stage.OnAfterSettingsInit += SetInitalized;
    }
    private void OnDestroy()
    {
        stage.OnStageStart -= EnemySpawnStart;
        stage.OnAfterSettingsInit -= SetInitalized;
    }

    private void Start()
    {
        spawnEnemeys.Clear();
        SetSpawnEnemyInfo("E001", 5, 10);
        SetSpawnEnemyInfo("E005", 5, 10);
    }

    public void SetSpawnEnemyInfo(string enemyUID, int level, int spawnCnt)
    {
        EnemySpawnInfo newEnemy = new EnemySpawnInfo(enemyUID, level, spawnCnt);
        spawnEnemeys.Add(newEnemy);
    }

    public void SetInitalized()
    {
        grid = stage.Grid;
        spawnPoint = grid.CellToWorldCenter(grid.SpawnPos.x, grid.SpawnPos.y);
    }

    public void EnemySpawnStart()
    {
        StartCoroutine(StartEnemySpawn(5,2f));
    }

    private void SpawnOneEnemy(EnemySpawnInfo spawnInfo)
    {
        Enemy enemyObj = Instantiate(baseEnemy, spawnPoint, Quaternion.identity);
        enemyObj.Init(spawnInfo.enemyUID, spawnInfo.level);

        EnemyMove enemyMove = enemyObj.GetComponent<EnemyMove>();

        if (enemyMove != null)
        {
            enemyMove.Initialize(stage, grid.SpawnPos, grid.GoalPos, enemyObj.MoveSpeed);
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
                stage.RegisterSpawnEnemy();
            }

            cnt--;

            yield return new WaitForSeconds(delayTime);
        }

        stage.EnemySpawnEnd();
        yield return null;
    }
}
