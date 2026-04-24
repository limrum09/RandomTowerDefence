using System;
using System.Collections.Generic;
using UnityEngine;
public enum EnemySkillValueType
{
    None,
    Flat,
    Percent,
    Count
}
public enum EnemySkillType
{
    None,
    Heal,
    Haste,
    Sheild,
    Debuff,
    Summon,
    Stealth
}

public enum EnemySkillTarget
{
    None,
    Self,
    Area,
    Tower
}
[Serializable]
public class EnemySkillRow
{
    public string Enemy_Skill_UID;
    public string Type;
    public string Target_type;
    public float Duration;
    public float CoolDown;
    public float Tick_Interval;
    public float Range;
    public string Value_type;
    public float Basic_Value;
    public float Increasee_Value;
    public string Scale_Type;
    public int Scale_Interval;
    public int Scale_Max;
    public string String_Key;
    public string Des_String_Key;
    public string Icon_UID;
}
[Serializable]
public class EnemySkillRowList
{
    public List<EnemySkillRow> datas = new List<EnemySkillRow>();
}
public class EnemySkillData
{
    public string enemySkillUID;
    public EnemySkillType type;
    public EnemySkillTarget targetType;
    public float duration;
    public float coolDown;
    public float tickInterval;
    public float range;
    public EnemySkillValueType valueType;
    public float basicValue;
    public float increaseValue;
    public string scaleType;
    public int scaleInterval;
    public int scaleMax;
    public string stringKey;
    public string desStringKey;
    public string iconPath;

    public EnemySkillData(string uid, EnemySkillType getType, EnemySkillTarget getTargettype, float getDuration, float getCooldown, float getTickIngerval,
        float getRange, EnemySkillValueType getValueType, float getBasicValue, float getIncreaseValue, string getScaleType, int getScaleIngervael,
        int getScaleMax, string getStringKey, string getDesStringKey, string getIconPath)
    {
        enemySkillUID = uid;
        type = getType;
        targetType = getTargettype;
        duration = getDuration;
        coolDown = getCooldown;
        tickInterval = getTickIngerval;
        range = getRange;
        valueType = getValueType;
        basicValue = getBasicValue;
        increaseValue = getIncreaseValue;
        scaleType = getScaleType;
        scaleMax = getScaleMax;
        stringKey = getStringKey;
        desStringKey = getDesStringKey;
        iconPath = getIconPath;
    }
}
public class EnemySkillDataManager
{
    Dictionary<string, EnemySkillData> skillDatas = new Dictionary<string, EnemySkillData>();

    private void LoadDataToJson()
    {
        EnemySkillRowList rowList = JsonLoader.LoadFromResources<EnemySkillRowList>("Data/EnemySkillData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(EnemySkillRow row in rowList.datas)
        {
            if(!Enum.TryParse(row.Type, true, out EnemySkillType type))
                continue;

            if (!Enum.TryParse(row.Target_type, true, out EnemySkillTarget target))
                continue;

            if (!Enum.TryParse(row.Value_type, true, out EnemySkillValueType valueType))
                continue;

            EnemySkillData data = new EnemySkillData(row.Enemy_Skill_UID, type, target, row.Duration, row.CoolDown, row.Tick_Interval, row.Range, valueType,
                row.Basic_Value, row.Increasee_Value, row.Scale_Type, row.Scale_Interval, row.Scale_Max, row.String_Key, row.Des_String_Key, row.Icon_UID);

            skillDatas[data.enemySkillUID] = data;
        }
    }
    public void Init()
    {
        skillDatas.Clear();
        LoadDataToJson();
    }

    public EnemySkillData GetEnemySkillData(string uid)
    {
        if (skillDatas.TryGetValue(uid, out EnemySkillData data))
            return data;

        return null;
    }
}
