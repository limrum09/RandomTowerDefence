using UnityEngine;

public class EnemyResolveInfo
{
    public string stringKey;
    public string itemPath;
    public string enemyUID;
    public int level;

    public int maxHP;
    public int maxShield;
    public float moveSpeed;
    public float rewardGold;

    public string skillUID;
    public string skillStringKey;
    public string skillDesStringKey;
    public EnemySkillType skillType;
    public EnemySkillTarget skillTarget;
    public float basicSkillValue;
    public float skillValue;
    public float duration;
    public float cooldown;
    public float tickInterval;
    public float range;
}

public static class EnemyInfoCal
{
    public static EnemyResolveInfo Create(string uid, int level)
    {
        EnemyData data = Managers.EnemyData.GetEnemyData(uid);

        if (data == null)
            return null;

        EnemyResolveInfo info = new EnemyResolveInfo();

        info.stringKey = data.stringKey;
        info.itemPath = data.iconPath;
        info.enemyUID = uid;
        info.level = level;

        info.maxHP = data.basicHp + (data.increaseHP * level);
        info.maxShield = data.basicShield + (data.increaseShield * level);
        info.moveSpeed = data.moveSpeed;
        info.rewardGold = data.rewardGold;

        info.skillUID = data.enemySkillUID;

        EnemySkillData skillData = Managers.EnemySkillData.GetEnemySkillData(data.enemySkillUID);

        if (skillData == null)
        {
            Debug.Log("스킬 데이터를 찾지 못함 : " + data.enemySkillUID);
            return null;
        }

        info.skillStringKey = skillData.stringKey;
        info.skillDesStringKey = skillData.desStringKey;
        info.skillType = skillData.type;
        info.skillTarget = skillData.targetType;
        info.duration = skillData.duration;
        info.cooldown = skillData.coolDown;
        info.tickInterval = skillData.tickInterval;
        info.range = skillData.range;
        info.basicSkillValue = skillData.basicValue;

        float val = info.basicSkillValue;

        if(skillData.scaleInterval > 0)
        {
            int scaleLevel = Mathf.Min(level, skillData.scaleMax);
            int step = scaleLevel / skillData.scaleInterval;
            val += skillData.increaseValue * step;
        }

        info.skillValue = val;

        return info;
    }
}
