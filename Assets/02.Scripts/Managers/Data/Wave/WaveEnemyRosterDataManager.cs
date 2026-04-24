using System;
using System.Collections.Generic;

[Serializable]
public class WaveEnemyRosterRow
{
    public string WaveID;
    public int SpawnOrder;
    public string Enemy_UID;
    public int EnemyLevel;
    public int SpawnCount;
    public float StartTime;
    public float SpawnInterval;
    public string SpawnType;
}
[Serializable]
public class WaveEnemyRosterRowList
{
    public List<WaveEnemyRosterRow> datas = new List<WaveEnemyRosterRow>();
}
public class WaveEnemyRosterData
{
    public string waveUID;
    public int spawnOrder;
    public string enemyUID;
    public int enemyLevel;
    public int enemyCount;
    public float startTime;
    public float spawnInterval;
    public string spawnType;

    public WaveEnemyRosterData(string waveUID, int spawnOrder, string enemyUID, int enemyLevel, int enemyCount, float startTime, float spawnInterval, string spawnType)
    {
        this.waveUID = waveUID;
        this.spawnOrder = spawnOrder;
        this.enemyUID = enemyUID;
        this.enemyLevel = enemyLevel;
        this.enemyCount = enemyCount;
        this.startTime = startTime;
        this.spawnInterval = spawnInterval;
        this.spawnType = spawnType;
    }
}
public class WaveEnemyRosterDataManager
{
    private Dictionary<string, List<WaveEnemyRosterData>> rosterDatas = new Dictionary<string, List<WaveEnemyRosterData>>();

    private void GetLoadDataToJson()
    {
        WaveEnemyRosterRowList rowList = JsonLoader.LoadFromResources<WaveEnemyRosterRowList>("Data/WaveEnemyRosterData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(WaveEnemyRosterRow row in rowList.datas)
        {
            WaveEnemyRosterData data = new WaveEnemyRosterData(row.WaveID, row.SpawnOrder, row.Enemy_UID, row.EnemyLevel,
                row.SpawnCount, row.StartTime, row.SpawnInterval, row.SpawnType);

            if(!rosterDatas.TryGetValue(data.waveUID, out List<WaveEnemyRosterData> dataList)){
                dataList = new List<WaveEnemyRosterData>();
                rosterDatas[data.waveUID] = dataList;
            }

            dataList.Add(data);
        }
    }

    public void Init()
    {
        rosterDatas.Clear();
        GetLoadDataToJson();
    }

    public List<WaveEnemyRosterData> GetWaveRosterData(string waveUID)
    {
        if (rosterDatas.TryGetValue(waveUID, out List<WaveEnemyRosterData> data))
            return data;

        return null;
    }
}
