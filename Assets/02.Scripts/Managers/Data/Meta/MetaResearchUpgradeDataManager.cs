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
    FreeTerrainRefresh
}

public enum CostIncreaseType
{
    Percent,
    Flat
}

[Serializable]
public class MetaResearchUpgradeDataRow
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
public class MetaResearchUpgradeDataRowList
{
    public List<MetaResearchUpgradeDataRow> datas = new List<MetaResearchUpgradeDataRow>();
}

public class MetaResearchUpgradeData
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

    public MetaResearchUpgradeData(string getUID, MetaUpgradeTarget getTargetType, string getTargetUID, MetaUpgradeType getUpgradeType,
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

public class MetaResearchUpgradeDataManager
{
    Dictionary<string, MetaResearchUpgradeData> metaDatas = new Dictionary<string, MetaResearchUpgradeData>();

    private void GetDataToJson()
    {
        MetaResearchUpgradeDataRowList rowList = JsonLoader.LoadFromResources<MetaResearchUpgradeDataRowList>("Data/MetaResearchUpgradeData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(MetaResearchUpgradeDataRow row in rowList.datas)
        {
            if (!Enum.TryParse(row.Target_Type, true, out MetaUpgradeTarget metaUpgradeTarget))
                continue;

            if (!Enum.TryParse(row.Upgrade, true, out MetaUpgradeType metaUpgradeType))
                continue;

            if (!Enum.TryParse(row.Type, true, out CostIncreaseType costIncreaseType))
                continue;

            MetaResearchUpgradeData data = new MetaResearchUpgradeData(row.UID, metaUpgradeTarget, row.Target_UID, metaUpgradeType, row.String_Key, costIncreaseType, row.Max, row.Cost_Base, row.Cost_Grow, row.Value_Level_Per);

            metaDatas[data.uid] = data;
        }
    }

    public void Init()
    {
        GetDataToJson();
    }


    public MetaResearchUpgradeData GetMetaResearchDataToTower(string getUID, MetaUpgradeTarget target, MetaUpgradeType upgrade)
    {
        string type = upgrade == MetaUpgradeType.AttackSpeed ? "ASPD" : "DMG";
        string uid = $"META_TOWER_{getUID}_{type}";
        if (!metaDatas.TryGetValue(uid, out MetaResearchUpgradeData data))
            return null;

        return data;
    }

    public MetaResearchUpgradeData GetMetaResearchDataToPublic(MetaUpgradeTarget target, MetaUpgradeType upgrade)
    {
        string uid = $"META_PUBLIC_{upgrade.ToString().ToUpper()}";
        if (!metaDatas.TryGetValue(uid, out MetaResearchUpgradeData data))
            return null;

        return data;
    }
}
