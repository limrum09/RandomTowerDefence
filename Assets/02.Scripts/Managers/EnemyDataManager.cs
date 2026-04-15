using System.Collections.Generic;
using UnityEngine;

public class EnemyDataManager
{
    Dictionary<string, EnemyData> enemyData = new Dictionary<string, EnemyData>();

    public void Init()
    {
        enemyData.Clear();

        enemyData["E001"] = new EnemyData("E001", "ENEMY_NAME_001", "ES0000", 100, 20, 4, 0, 0, 1, "Slime");
        enemyData["E003"] = new EnemyData("E003", "ENEMY_NAME_003", "ES0000", 200, 40, 5, 10, 2, 1, "");
        enemyData["E005"] = new EnemyData("E005", "ENEMY_NAME_005", "ES0021", 150, 20, 8, 5, 1, 1, "Skeleton");
    }

    public EnemyData GetEnemyData(string uid)
    {
        if (enemyData.TryGetValue(uid, out EnemyData data))
            return data;

        return null;
    }
}
