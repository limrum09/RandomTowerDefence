using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Windows.Speech;
public enum SkillEffectType
{
    Gold,
    AtkDamage,
    Shield,
    CriticalPer,
    EnemySlow
}

public enum EffectValueUnit
{
    PercentPoint,
    Flat,
    Percent
}

[Serializable]
public class TowerSkillDataRow
{
    public string Tower_Skill_UID;
    public string String_Key;
    public string Des_String_Key;
    public string Type;
    public int Step;
    public int RequiredCount;
    public int RequiredTowerGrade;
    public string EffectType;
    public int EffectValue;
    public string EffectValueUnit;
    public float Duration;
    public bool BossApply;
    public float BossModifier;
    public string Icon_UID;
}

[Serializable]
public class TowerSkillDataRowList
{
    public List<TowerSkillDataRow> datas = new List<TowerSkillDataRow>();
}

public class TowerSkillData
{
    public string towerSkillUID;
    public string Stringkey;
    public string DesStringKey;
    public TowerType towerType;
    public int step;
    public int requiredCount;
    public int requiredTowerGrade;
    public SkillEffectType effectType;
    public int effectValue;
    public EffectValueUnit effectValueUnit;
    public float duration;
    public bool bossApply;
    public float bossModifier;
    public string icon_UID;

    public TowerSkillData(string uid, string key, string desKey, TowerType getType, int getStep,
        int reCount, int towerGrade, SkillEffectType getEffectType, int getEffectValue,
        EffectValueUnit unit, float dura, bool isBoss, float modifier, string iconPath)
    {
        towerSkillUID = uid;
        Stringkey = key;
        DesStringKey = desKey;
        towerType = getType;
        step = getStep;
        requiredCount = reCount;
        requiredTowerGrade = towerGrade;
        effectType = getEffectType;
        effectValue = getEffectValue;
        effectValueUnit = unit;
        duration = dura;
        bossApply = isBoss;
        bossModifier = modifier;
        icon_UID = iconPath;
    }
}

public class SkillValueCollect
{
    public string skillUID;
    public int towerCnt;
    public int value;

    public SkillValueCollect(string skillUID, int towerCnt, int value)
    {
        this.skillUID = skillUID;
        this.towerCnt = towerCnt;
        this.value = value;
    }
}

public class TowerSkillDataManager
{
    Dictionary<string, TowerSkillData> skillDatas = new Dictionary<string, TowerSkillData>();
    Dictionary<TowerType, List<SkillValueCollect>> skillTypes = new Dictionary<TowerType, List<SkillValueCollect>>();

    private void GetTowerSkillDataToJson()
    {
        TowerSkillDataRowList rowList = JsonLoader.LoadFromResources<TowerSkillDataRowList>("Data/TowerSkillData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach (TowerSkillDataRow row in rowList.datas)
        {
            if (!Enum.TryParse(row.Type, out TowerType towerType))
                continue;

            if (!Enum.TryParse(row.EffectType, out SkillEffectType effectType))
                continue;

            if (!Enum.TryParse(row.EffectValueUnit, true, out EffectValueUnit EffectUnit))
                continue;

            TowerSkillData data = new TowerSkillData(
                row.Tower_Skill_UID, row.String_Key, row.Des_String_Key, towerType, row.Step,
                row.RequiredCount, row.RequiredTowerGrade, effectType, row.EffectValue,
                EffectUnit, row.Duration, row.BossApply, row.BossModifier, row.Icon_UID
            );

            skillDatas[data.towerSkillUID] = data;

            SkillValueCollect collect = new SkillValueCollect(data.towerSkillUID, data.requiredCount, data.effectValue);

            if(!skillTypes.TryGetValue(data.towerType, out List<SkillValueCollect> list))
            {
                list = new List<SkillValueCollect>();
                skillTypes[data.towerType] = list;
            }

            list.Add(collect);
        }

        foreach(var data in skillTypes)
        {
            data.Value.Sort((x ,y) => x.towerCnt.CompareTo(y.towerCnt));
        }
    }

    public void Init()
    {
        skillDatas.Clear();
        skillTypes.Clear();
        GetTowerSkillDataToJson();
    }

    public TowerSkillData GetTowerSkillData(string uid)
    {
        if (skillDatas.TryGetValue(uid, out TowerSkillData data))
            return data;

        return null;
    }

    public TowerSkillData GetTowerSkillDataByTypeAndCount(TowerType type, int count)
    {
        List<SkillValueCollect> collect = GetTowerCollections(type);

        if (collect == null)
            return null;

        int index = 0;
        for(int i = 0; i < collect.Count; i++)
        {
            if (count >= collect[i].towerCnt)
            {
                index = i;
            }
            else
                break;
        }

        return GetTowerSkillData(collect[index].skillUID);
    }

    public List<SkillValueCollect> GetTowerCollections(TowerType type)
    {
        if(skillTypes.TryGetValue(type, out List<SkillValueCollect> data))
            return data;

        return null;
    }
}