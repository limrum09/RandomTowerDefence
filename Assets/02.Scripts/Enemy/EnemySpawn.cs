using UnityEngine;
/// <summary>
/// 적 스폰 관리
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    private StageManager stage;
    [SerializeField]
    private GameObject testEnemey;
    [SerializeField]
    private int spawnCount;

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

    public void SetInitalized()
    {
        grid = stage.Grid;
        spawnPoint = grid.CellToWorldCenter(grid.SpawnPos.x, grid.SpawnPos.y);
    }

    public void EnemySpawnStart()
    {
        SpawnOneEnemy();
    }

    private void SpawnOneEnemy()
    {
        GameObject enemyObj = Instantiate(testEnemey, spawnPoint, Quaternion.identity);

        EnemyMove enemyMove = enemyObj.GetComponent<EnemyMove>();

        if (enemyMove != null)
        {
            enemyMove.Initialize(stage, grid.SpawnPos, grid.GoalPos, 10f);
        }
    }
}
