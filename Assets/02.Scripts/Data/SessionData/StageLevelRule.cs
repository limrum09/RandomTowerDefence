using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class StageLevelRuleDataRow
{
    public int User_Level;
    public int Need_EXP;
    public int Max_Tower_Count;
}

[Serializable]
public class StageLevelRuleDataRowList
{
    public List<StageLevelRuleDataRow> datas = new List<StageLevelRuleDataRow>();
}

[Serializable]
public class StageLevelRuleData
{
    public int userLevel;
    public int needEXP;
    public int maxTowerCount;

    public StageLevelRuleData(int getUserLevel, int getNeedEXP, int getMaxTowerCount)
    {
        userLevel = getUserLevel;
        needEXP = getNeedEXP;
        maxTowerCount = getMaxTowerCount;
    }
}

public class StageLevelRule
{
    private readonly Dictionary<int, int> needExpToLevel = new Dictionary<int, int>();
    private readonly Dictionary<int, int> limitTowerCountToLevel = new Dictionary<int, int>();

    private int maxLevel;
    private int maxTowerCountForMaxLevel;
    private int minTowerCount;

    private void GetDataToJson()
    {
        StageLevelRuleDataRowList rowList = JsonLoader.LoadFromResources<StageLevelRuleDataRowList>("Data/StageLevelRule");

        if (rowList == null || rowList.datas.Count == 0)
            return;


        limitTowerCountToLevel.Clear();
        needExpToLevel.Clear();

        foreach(StageLevelRuleDataRow row in rowList.datas)
        {
            limitTowerCountToLevel[row.User_Level] = row.Max_Tower_Count;
            needExpToLevel[row.User_Level] = row.Need_EXP;
        }

        maxLevel = limitTowerCountToLevel.Keys.Max();
        maxTowerCountForMaxLevel = limitTowerCountToLevel[maxLevel];
        minTowerCount = limitTowerCountToLevel.Values.Min();
    }

    public void Init()
    {
        GetDataToJson();
    }

    public int GetNeedEXP(int level)
    {
        if (needExpToLevel.TryGetValue(level, out int needExp))
        {
            return needExp;
        }

        return 999999;
    }

    public int limitTowerCnt(int level)
    {
        if (level <= 0)
            return 0;

        if (level >= maxLevel)
            return maxTowerCountForMaxLevel;

        if(limitTowerCountToLevel.TryGetValue(level,out int towerLimit))
            return towerLimit;

        return minTowerCount;
    }
}