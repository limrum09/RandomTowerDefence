using System;
using System.Collections.Generic;

public enum MetaUpgradeTarget
{
    Tower,
    Public
}

public enum MetaUpgradeType
{
    Damage,
    AttackSpeed,
    StartingGold,
    DropGold,
    FreeObstacle,
    FreeTerrainReroll
}

public enum CostIncreaseType
{
    Percent,
    Flat
}

[Serializable]
public class MetaResearchDataRow
{
    public string UID;
    public string Target_Type;
    public string Target_UID;
    public string Upgrade;
    public string String_Key;
    public string Type;
    public int Max;
    public int Cost_Base;
    public float Cost_Grow;
    public float Value_Level_Per;
}

[Serializable]
public class MetaResearchDataRowList
{
    public List<MetaResearchDataRow> datas = new List<MetaResearchDataRow>();
}

public class MetaResearchData
{
    public string uid;
    public MetaUpgradeTarget targetType;
    public string targetUID;
    public MetaUpgradeType upgradeType;
    public string stringKey;
    public CostIncreaseType costIncreaseType;
    public int maxLevel;
    public int costBase;
    public float costGrow;
    public float valueLevelPer;

    public MetaResearchData(string getUID, MetaUpgradeTarget getTargetType, string getTargetUID, MetaUpgradeType getUpgradeType,
        string getStringKey, CostIncreaseType getCostIncreastType, int getmaxLevel, int getCostBase, float getCostGrow, float getValueLevelPer)
    {
        uid = getUID;
        targetType = getTargetType;
        targetUID = getTargetUID;
        upgradeType = getUpgradeType;
        stringKey = getStringKey;
        costIncreaseType = getCostIncreastType;
        maxLevel = getmaxLevel;
        costBase = getCostBase;
        costGrow = getCostGrow;
        valueLevelPer = getValueLevelPer;
    }

    public float CalculateValue(float baseValue, int level)
    {
        switch (costIncreaseType)
        {
            case CostIncreaseType.Percent:
                return baseValue * (1f + (valueLevelPer * level));
            case CostIncreaseType.Flat:
                return baseValue + (valueLevelPer * level);
            default:
                return baseValue;
        }
    }
}

public class MetaResearchDataManager
{
    Dictionary<string, MetaResearchData> metaDatas = new Dictionary<string, MetaResearchData>();

    private void GetDataToJson()
    {
        MetaResearchDataRowList rowList = JsonLoader.LoadFromResources<MetaResearchDataRowList>("Data/MetaResearchUpgradeData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(MetaResearchDataRow row in rowList.datas)
        {
            if (!Enum.TryParse(row.Target_Type, true, out MetaUpgradeTarget metaUpgradeTarget))
                continue;

            if (!Enum.TryParse(row.Upgrade, true, out MetaUpgradeType metaUpgradeType))
                continue;

            if (!Enum.TryParse(row.Type, true, out CostIncreaseType costIncreaseType))
                continue;

            MetaResearchData data = new MetaResearchData(row.UID, metaUpgradeTarget, row.Target_UID, metaUpgradeType, row.String_Key, costIncreaseType, row.Max, row.Cost_Base, row.Cost_Grow, row.Value_Level_Per);

            metaDatas[data.uid] = data;
        }
    }

    public void Init()
    {
        GetDataToJson();
    }


    public MetaResearchData GetMetaResearchDataToTower(string getUID, MetaUpgradeTarget target, MetaUpgradeType upgrade)
    {
        string type = upgrade == MetaUpgradeType.AttackSpeed ? "ASPD" : "DMG";
        string uid = $"META_TOWER_{getUID}_{type}";
        if (!metaDatas.TryGetValue(uid, out MetaResearchData data))
            return null;

        return data;
    }

    public MetaResearchData GetMetaResearchDataToPublic(MetaUpgradeTarget target, MetaUpgradeType upgrade)
    {
        string uid = $"META_PUBLIC_{upgrade.ToString().ToUpper()}";
        if (!metaDatas.TryGetValue(uid, out MetaResearchData data))
            return null;

        return data;
    }
}
