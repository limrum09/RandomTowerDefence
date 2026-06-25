using System;
using UnityEngine;

public class EnemyFactory
{
    private Enemy baseEnemy;
    private Transform enemyParent;
    private GridManager grid;
    private PathFinder path;
    private bool canSpawn;

    public event Action OnEnemySpawn;   // 적 1마리가 생성될 때 호출
    public event Action OnEnemyReached; // 적이 목표지점에 도달하면 호출
    public event Action OnEnemyDead;// 적이 사망시 호출, int = 보상 골드
    public event Action<int> OnEnemySummon;


    private void OnEnemyReachGoal()
    {
        OnEnemyReached?.Invoke();
    }

    private void EnemyDead()
    {
        OnEnemyDead?.Invoke();
    }

    public EnemyFactory(Enemy getBaseEnemy, Transform getEnemyParent, GridManager getGrid, PathFinder getPath)
    {
        baseEnemy = getBaseEnemy;
        enemyParent = getEnemyParent;
        grid = getGrid;
        path = getPath;
    }

    public void SetCanSpawn(bool val)
    {
        canSpawn = val;
    }

    public int SpawnSummonedEnemies(string eneymUID, int level, Vector3 centerPos, int count)
    {
        int successCount = 0;
        float radius = 1.0f;

        for(int i = 0; i < count; i++)
        {
            Vector3 dir = UnityEngine.Random.insideUnitCircle.normalized;
            Vector3 offset = new Vector3(dir.x, dir.y, 0f) * radius;
            Vector3 spawnPos = centerPos + offset;

            if (SpawnEnemy(eneymUID, level, spawnPos))
                successCount++;
        }

        if (successCount > 0)
            OnEnemySummon?.Invoke(successCount);

        return successCount;
    }

    /// <summary>
    /// 오로지 적 1마리를 생성하고 입력 경로 초기화
    /// </summary>
    /// <param name="spawnInfo">생성할 적 정보</param>
    public bool SpawnEnemy(string enemyUID, int level, Vector3 worldPos)
    {
        if (!canSpawn)
            return false;

        // 기본 Enemy Prefab을 스폰 위치에 생성
        Enemy enemyObj = UnityEngine.Object.Instantiate(baseEnemy, worldPos, Quaternion.identity, enemyParent);
        // 적 데이터 초기화, enemyUID와 level에 따라 스탯/스킬 들이 설정
        enemyObj.Init(enemyUID, level);

        // 컴포넌트 가져오기
        EnemyMove enemyMove = enemyObj.GetComponent<EnemyMove>();
        // enemyMove가 없으면 정상 적인 적이 아니기에 방어처리
        if (enemyMove == null)
        {
            UnityEngine.Object.Destroy(enemyObj.gameObject);
            return false;
        }

        EnemySkill skill = enemyObj.GetComponent<EnemySkill>();
        if(skill == null)
        {
            UnityEngine.Object.Destroy(enemyObj.gameObject);
            return false;
        }

        // 이벤트 추가
        enemyMove.onReachGoal += OnEnemyReachGoal;
        enemyMove.onDead += EnemyDead;

        Vector2Int startCell = grid.WorldToCell(worldPos);
        if(!enemyMove.Initialize(grid, path, enemyObj, startCell, grid.GoalPos))
        {
            UnityEngine.Object.Destroy(enemyObj.gameObject);
            return false;
        }

        skill.SetEenmyFactory(this);

        // 스폰 이벤트 호출
        OnEnemySpawn?.Invoke();

        return true;
    }
}
