using System;
using System.Collections.Generic;

[Serializable]
public class EnemyDataRow
{
    public string Enemy_UID;
    public string Type;
    public string String_Key;
    public string Enemy_Skill_UID;
    public int Basic_HP;
    public int Increase_HP;
    public float Move_Speed;
    public int Basic_Shield;
    public int Increase_Sheild;
    public float Reward_Gold;
    public string Icon_UID;
}

[Serializable]
public class EnemyDataRowList
{
    public List<EnemyDataRow> datas = new List<EnemyDataRow>();
}

public class EnemyDataManager
{
    Dictionary<string, EnemyData> enemyData = new Dictionary<string, EnemyData>();

    private void GetDataToJson()
    {
        EnemyDataRowList rowList = JsonLoader.LoadFromResources<EnemyDataRowList>("Data/EnemyData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(EnemyDataRow row in rowList.datas)
        {
            EnemyData data = new EnemyData(row.Enemy_UID, row.String_Key, row.Enemy_Skill_UID, row.Basic_HP,
                row.Increase_HP, row.Move_Speed, row.Basic_Shield, row.Increase_Sheild, row.Reward_Gold, row.Icon_UID);

            enemyData[data.enemyUID] = data;
        }
    }

    public void Init()
    {
        enemyData.Clear();

        GetDataToJson();
    }

    public EnemyData GetEnemyData(string uid)
    {
        if (enemyData.TryGetValue(uid, out EnemyData data))
            return data;

        return null;
    }
}
