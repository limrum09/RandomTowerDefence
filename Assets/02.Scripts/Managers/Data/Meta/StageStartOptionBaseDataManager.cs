using System;
using System.Collections.Generic;

[Serializable]
public class StageStartOptionBaseRow
{
    public string UID;
    public string String_Key;
    public float Base_Value;
}

[Serializable]
public class StageStartOptionBaseRowList
{
    public List<StageStartOptionBaseRow> datas = new List<StageStartOptionBaseRow>();
}

public class StageStartOptionBaseData
{
    public string uid;
    public string stringKey;
    public float baseValue;

    public StageStartOptionBaseData(string getUid, string getStringKey, float getBaseValue)
    {
        uid = getUid;
        stringKey = getStringKey;
        baseValue = getBaseValue;
    }
}

public class StageStartOptionBaseDataManager
{
    private Dictionary<MetaUpgradeType, StageStartOptionBaseData> startDatas = new Dictionary<MetaUpgradeType, StageStartOptionBaseData>();

    private void LoadDataToJson()
    {
        StageStartOptionBaseRowList rowList = JsonLoader.LoadFromResources<StageStartOptionBaseRowList>("Data/StageStartOptions");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(var row in rowList.datas)
        {
            if (!Enum.TryParse(row.UID, true, out MetaUpgradeType type))
                continue;

            StageStartOptionBaseData data = new StageStartOptionBaseData(row.UID, row.String_Key, row.Base_Value);

            startDatas[type] = data;
        }
    }

    public void Init()
    {
        LoadDataToJson();
    }

    public StageStartOptionBaseData GetStartOptionData (MetaUpgradeType type)
    {
        if (!startDatas.TryGetValue(type, out StageStartOptionBaseData data))
            return null;

        return data;
    }
}
