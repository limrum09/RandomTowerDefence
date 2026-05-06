using System;
using System.Collections;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    [SerializeField]
    private Enemy root;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private LayerMask towerLayer;
    private EnemySkillData data;

    private string skillUID;
    private bool isGetSkill;
    private bool isSpeedSkill;

    private float skillTimer;
    private float skillCoolTime;
    private float duration;
    private float skillTick;
    private float skillRange;
    private float skillValue;

    private EnemySkillValueType valueType;
    private EnemySkillType skillType;
    private EnemySkillTarget target;

    public  void Init(Enemy enemy,  string getSkillUID = null)
    {
        isGetSkill = true;
        isSpeedSkill = false;
        root = enemy;

        if (getSkillUID == "ES0000" || getSkillUID == null)
        {
            isGetSkill = false;
            return;
        }

        skillUID = getSkillUID;

        data = Managers.EnemySkillData.GetEnemySkillData(skillUID);
        skillTimer = 0.0f;
        skillCoolTime = data.coolDown;
        duration = data.duration;
        skillRange = data.range;

        skillValue = data.basicValue;
        if(root.Level >= data.scaleInterval && data.scaleInterval != 0)
        {
            if(data.scaleMax <= root.Level)
            {
                int value = data.scaleMax / data.scaleInterval;
                skillValue += (data.increaseValue * value);
            }
            else
            {
                int value = root.Level / data.scaleInterval;
                skillValue += (data.increaseValue * value);
            }
        }

        valueType = data.valueType;
        skillType = data.type;
        skillTick = data.tickInterval;
        target = data.targetType;
    }

    public void ApplySkillEffect(EnemySkillType getType, float value, float getDuration, float getTick)
    {
        switch (getType)
        {
            case EnemySkillType.Heal:
                ApplyHeal((int)value, getDuration, getTick);
                break;
            case EnemySkillType.Haste:
                ApplySpeed(value, getDuration);
                break;
            case EnemySkillType.Shield:
                ApplyShield((int)value, getDuration, getTick);
                break;
            case EnemySkillType.Summon:
                break;
            case EnemySkillType.Stealth:
                break;
        }
    }

    private void Update()
    {
        if (!isGetSkill)
            return;

        skillTimer += Time.deltaTime;

        if(skillTimer > skillCoolTime)
        {
            UsingSkill();
            skillTimer = 0.0f;
        }
    }

    private void UsingSkill()
    {
        Managers.Effect.Play("Enemy" + skillType.ToString(), root.transform, PoolCategory.Stage, true);

        if (target == EnemySkillTarget.Self)
            ApplySkillEffect(skillType, skillValue, duration, skillTick);
        else if (target == EnemySkillTarget.Area)
            CheckEnemyAreaFindEnemy();
        else if (target == EnemySkillTarget.Tower)
            CheckEnemyAreaFindTower();
    }

    private void CheckEnemyAreaFindEnemy()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, skillRange, enemyLayer);

        foreach(Collider2D col in cols)
        {
            EnemySkill enemy = col.GetComponent<EnemySkill>();
            if (enemy == null)
                continue;

            enemy.ApplySkillEffect(skillType, skillValue, duration, skillTick);
        }
    }

    private void CheckEnemyAreaFindTower()
    {
        if (skillType != EnemySkillType.Debuff)
            return;

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, skillRange, towerLayer);

        foreach(Collider2D col in cols)
        {
            Tower enemy = col.GetComponent<Tower>();
            if (enemy == null)
                continue;
        }
    }

    private void ApplyHeal(int value, float getDuration, float tick = 0.0f)
    {
        if (getDuration <= 0.0f)
        {
            root.EnemeyHeal(value);
            return;
        }
            

        if (getDuration > 0.0f)
        {
            StartCoroutine(TickSkill(getDuration, tick, () =>
            {
                root.EnemeyHeal(value);
            }));
        }
    }

    private void ApplySpeed (float speed, float getDuration, float tick = 0.0f)
    {
        if (getDuration <= 0.0f)
        {
            root.MoveSpeedChange(speed);
            return;
        }
            

        if(getDuration > 0.0f)
        {
            if (!isSpeedSkill)
                StartCoroutine(SpeedDuration(speed, getDuration));
        }
    }

    private void ApplyShield(int value, float getDuration, float tick = 0.0f)
    {
        if (getDuration <= 0.0f)
        {
            root.ShieldValueChange(value);
            return;
        }
            

        if (getDuration > 0.0f)
        {
            StartCoroutine(TickSkill(getDuration, tick, () =>
            {
                root.ShieldValueChange(value);
            }));
        }
    }
    
    IEnumerator TickSkill(float getDuration, float getInterval, Action tickAction)
    {
        if(getInterval <= 0.0f)
        {
            tickAction?.Invoke();
            yield break;

        }
        float timer = 0.0f;
        float skillInterval = getInterval;

        while(timer < getDuration)
        {
            timer += Time.deltaTime;

            if(timer > skillInterval)
            {
                tickAction?.Invoke();
                skillInterval += timer;
            }

            yield return null;
        }
    }

    IEnumerator SpeedDuration(float value, float duration)
    {
        root.MoveSpeedChange(value);
        isSpeedSkill = true;
        yield return new WaitForSeconds(duration);

        isSpeedSkill = false;
        root.MoveSpeedChange(-value);
    }
}
