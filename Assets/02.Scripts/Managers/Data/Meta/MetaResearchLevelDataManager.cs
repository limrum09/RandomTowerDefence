using System;
using System.Collections.Generic;

[Serializable]
public class MetaResearchLevelRow
{
    public int Research_Level;
    public int Required_EXP;
    public int Unlock_Research_Grade;
    public string Note;
}

[Serializable]
public class MetaReserachLevelRowList
{
    public List<MetaResearchLevelRow> datas = new List<MetaResearchLevelRow>();
}

public class MetaResearchLevelData
{
    public int level;
    public int requiredExp;
    public int UnlockResearchGrade;

    public MetaResearchLevelData(int getLevel, int getRequiredExp, int getUnlockResearchGrade)
    {
        level = getLevel;
        requiredExp = getRequiredExp;
        UnlockResearchGrade = getUnlockResearchGrade;
    }
}

public class MetaResearchLevelDataManager
{
    private Dictionary<int, MetaResearchLevelData> researchLevel = new Dictionary<int, MetaResearchLevelData>();

    private void GetDataToJson()
    {
        MetaReserachLevelRowList rowList = JsonLoader.LoadFromResources<MetaReserachLevelRowList>("Data/MetaResearchLevelData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(MetaResearchLevelRow row in rowList.datas)
        {
            MetaResearchLevelData data = new MetaResearchLevelData(row.Research_Level, row.Required_EXP, row.Unlock_Research_Grade);

            researchLevel[data.level] = data;
        }
    }

    public void Init()
    {
        GetDataToJson();
    }

    public int GetNeedExp(int getLevel)
    {
        if (!researchLevel.TryGetValue(getLevel, out MetaResearchLevelData data))
            return -1;

        return data.requiredExp;
    }

    public int GetCurrentUnlockGrade(int getLevel)
    {
        if (!researchLevel.TryGetValue(getLevel, out MetaResearchLevelData data))
            return -1;

        return data.UnlockResearchGrade;
    }

    public int GetNeedUnlockLevel(int getGrade)
    {
        int minLevel = -1;

        foreach(MetaResearchLevelData data in researchLevel.Values)
        {
            if (getGrade != data.UnlockResearchGrade)
                continue;

            if (minLevel > data.level || minLevel == -1)
                minLevel = data.level;
        }

        return minLevel;
    }
}
