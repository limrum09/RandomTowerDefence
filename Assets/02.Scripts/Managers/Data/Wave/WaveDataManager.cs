using System;
using System.Collections.Generic;

[Serializable]
public class WaveDataRow
{
    public string WaveNo;
    public string NextWave;
    public string Wave_Type;
    public string IsBossWave;
}

[Serializable]
public class WaveDataRowList
{
    public List<WaveDataRow> datas = new List<WaveDataRow>();
}

public class WaveData
{
    public string waveUID;
    public string nextWave;
    public bool isBoss;

    public WaveData(string getUID, string getNextWaveUID, string isBossStr)
    {
        waveUID = getUID;
        nextWave = getNextWaveUID;
        isBoss = isBossStr == "N" ? false : true;
    }
}

public class WaveDataManager
{
    Dictionary<string, WaveData> waveDatas = new Dictionary<string, WaveData>();
    private void GetWaveDatasToJson()
    {
        WaveDataRowList rowList = JsonLoader.LoadFromResources<WaveDataRowList>("Data/WaveData");

        if (rowList == null && rowList.datas == null)
            return;

        foreach(WaveDataRow row in rowList.datas)
        {
            WaveData data = new WaveData(row.WaveNo, row.NextWave, row.IsBossWave);

            waveDatas[data.waveUID] = data;
        }
    }
    public void Init()
    {
        waveDatas.Clear();
        GetWaveDatasToJson();
    }

    public WaveData GetWaveData(string uid)
    {
        if (waveDatas.TryGetValue(uid, out WaveData data))
            return data;

        return null;
    }
}
