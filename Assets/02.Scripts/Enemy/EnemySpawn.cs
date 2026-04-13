using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    private GameObject testEnemey;
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private Transform spawnPoint;
    void Start()
    {
        Managers.Game.OnGameStart += EnemySpawnStart;
        Managers.Game.OnAfterSettingsInit += SetInitalized;
    }
    private void OnDestroy()
    {
        Managers.Game.OnGameStart -= EnemySpawnStart;
        Managers.Game.OnAfterSettingsInit -= SetInitalized;
    }

    public void SetInitalized()
    {
        spawnPoint.position = new Vector3(Managers.Grid.SpawnPos.x, Managers.Grid.SpawnPos.y, 0f);
    }

    public void EnemySpawnStart()
    {

    }
}
