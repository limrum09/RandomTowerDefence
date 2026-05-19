using System;
using System.Collections.Generic;

public enum StageRules
{
    WaveClearInterestRate,
    MaxInterestGold,
    EnemyReachLifeDamage,
    ObstacleInstallBaseCost,
    WaveClearRewardGold,
    WaveClearRewardExp,
    StageClearMetaCurrency,
    BossWaveRewardMultiplier,
    MaxWaveInterestCount,
    FreeTowerMoveCount,
}

[Serializable]
public class StageRuleRow
{
    public string UID;
    public string String_Key;
    public int Value;
    public string Description;
}

[Serializable]
public class StageRuleRowList
{
    public List<StageRuleRow> datas = new List<StageRuleRow>();
}

public class StageRuleData
{
    public StageRules uid;
    public string stringKey;
    public int value;

    public StageRuleData(StageRules getUID, string getStringKey, int getValue)
    {
        uid = getUID;
        stringKey = getStringKey;
        value = getValue;
    }
}

public class StageRuleDataManager
{
    Dictionary<StageRules, StageRuleData> stageRules = new Dictionary<StageRules, StageRuleData>();
    private void GetDataToJaon()
    {
        StageRuleRowList rowList = JsonLoader.LoadFromResources<StageRuleRowList>("Data/StageRuleData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(StageRuleRow row in rowList.datas)
        {
            if (!Enum.TryParse(row.UID, true, out StageRules uid))
                continue;

            StageRuleData data = new StageRuleData(uid, row.String_Key, row.Value);

            stageRules[data.uid] = data;
        }
    }
    
    public void Init()
    {
        GetDataToJaon();
    }

    public int GetRuleData(StageRules rule)
    {
        if (!stageRules.TryGetValue(rule, out StageRuleData data))
            return -1;

        return data.value;
    }
}
