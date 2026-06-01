using System;
using System.Collections.Generic;

public class RunStatUpgradeManager
{
    private readonly Dictionary<TowerType, int> atkDamageTowerStep = new Dictionary<TowerType, int>();
    private readonly Dictionary<TowerType, int> atkSpeedTowerStep = new Dictionary<TowerType, int>();
    private readonly Dictionary<TowerType, int> atkDamageItemTowerStep = new Dictionary<TowerType, int>();
    private readonly Dictionary<TowerType, int> atkSpeedItemTowerStep = new Dictionary<TowerType, int>();
    private readonly Dictionary<TowerType, int> atkDamageSkillStep = new Dictionary<TowerType, int>();
    private readonly Dictionary<TowerType, int> atkSpeedSkillStep = new Dictionary<TowerType, int>();

    private readonly Dictionary<TowerType, float> criticalPerSkill = new Dictionary<TowerType, float>();
    private readonly Dictionary<TowerType, float> enemySlowSkill = new Dictionary<TowerType, float>();

    private int GlobalAtkDamageStep;
    private int GlobalAtkSpeedStep;

    public int GoldDropValue { get; private set; }
    public int AbilityTriggerRequest { get; private set; }
    public float MaxInterestValue { get; private set; }
    public float IncreaseInterserPer { get; private set; }

    public event Action OnChangedTowerStat;
    public void Init()
    {
        Reset();
    }

    public void Reset()
    {
        atkDamageTowerStep.Clear();
        atkSpeedTowerStep.Clear();
        atkDamageItemTowerStep.Clear();
        atkSpeedItemTowerStep.Clear();
        atkDamageSkillStep.Clear();
        atkSpeedSkillStep.Clear();
        criticalPerSkill.Clear();
        enemySlowSkill.Clear();

        GlobalAtkDamageStep = 0;
        GlobalAtkSpeedStep = 0;

        GoldDropValue = 0;
        AbilityTriggerRequest = 0;
        MaxInterestValue = 0;

        foreach (TowerType towerType in System.Enum.GetValues(typeof(TowerType)))
        {
            atkDamageTowerStep[towerType] = 0;
            atkSpeedTowerStep[towerType] = 0;
            atkDamageItemTowerStep[towerType] = 0;
            atkSpeedItemTowerStep[towerType] = 0;
            atkDamageSkillStep[towerType] = 0;
            atkSpeedSkillStep[towerType] = 0;
            criticalPerSkill[towerType] = 0;
            enemySlowSkill[towerType] = 0;
        }
    }

    public void AddStatAtkDamage(TowerType type, int value)
    {
        atkDamageTowerStep[type] += value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddStatAtkSpeed(TowerType type, int value)
    {
        atkSpeedTowerStep[type] += value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddItemAtkDamage(ScopeRange scope, int value)
    {
        if(scope == ScopeRange.AllTower)
        {
            GlobalAtkDamageStep += value;
            OnChangedTowerStat?.Invoke();
            return;
        }

        if(TryConvertScopeToTowerType(scope, out TowerType type))
            atkDamageItemTowerStep[type] += value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddItemAtkSpeed(ScopeRange scope, int value)
    {
        if(scope == ScopeRange.AllTower)
        {
            GlobalAtkSpeedStep += value;
            OnChangedTowerStat?.Invoke();
            return;
        }

        if (TryConvertScopeToTowerType(scope, out TowerType type))
            atkSpeedItemTowerStep[type] += value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddSkillAtkDamage(TowerType type, int value)
    {
        // 현제는 단계별로 증가 값이 정해져 있음
        // 공격력이나 속도 같은 값들은 서로의 타워에 영향을 주지 않기에 값만 변경한다.
        atkDamageSkillStep[type] = value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddSkillAtkSpeed(TowerType type, int value)
    {
        atkSpeedSkillStep[type] = value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddSkillCriticalPer(TowerType type, float value)
    {
        if (type != TowerType.Werebeast)
            return;

        criticalPerSkill[type] = value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddSkillEnemySlowPer(TowerType type, float value)
    {
        if (type != TowerType.Orc)
            return;

        enemySlowSkill[type] = value;
        OnChangedTowerStat?.Invoke();
    }

    public void AddGoldDropIncrease(int value) => GoldDropValue += value;

    public void ChangeMaxInterest(float value) => MaxInterestValue += value;

    public void SetIncreseInterest(float value) => IncreaseInterserPer = value;

    public int GetAtkDamageStep(TowerType tower)
    {
        int local = atkDamageTowerStep.TryGetValue(tower, out int value) ? value : 0;

        return GlobalAtkDamageStep + local;
    }

    public int GetAtkSpeedStep(TowerType tower)
    {
        int local = atkSpeedTowerStep.TryGetValue(tower, out int value) ? value : 0;

        return GlobalAtkSpeedStep + local;
    }
    
    public int GetItemAtkDamageStep(TowerType tower)
    {
        return atkDamageItemTowerStep.TryGetValue(tower, out int value) ? value : 0;
    }

    public int GetItemAtkSpeedStep(TowerType tower)
    {
        return atkSpeedItemTowerStep.TryGetValue(tower, out int value) ? value : 0;
    }

    public int GetSkillAtkDamageStep(TowerType tower)
    {
        return atkDamageSkillStep.TryGetValue(tower, out int value) ? value : 0;
    }

    public int GetSkillAtkSpeedStep(TowerType tower)
    {
        return atkSpeedSkillStep.TryGetValue(tower, out int value) ? value : 0;
    }

    public float GetCriticalPer(TowerType tower)
    {
        if (tower != TowerType.Werebeast)
            return 0.0f;

        return criticalPerSkill.TryGetValue(tower, out float value) ? value : 0.0f;
    }

    public float GetEnemySlowPer(TowerType tower)
    {
        if(tower != TowerType.Orc)
            return 0.0f;

        return enemySlowSkill.TryGetValue(tower, out float value) ? value : 0.0f;
    }

    private bool TryConvertScopeToTowerType(ScopeRange scope, out TowerType towerType)
    {
        towerType = default;

        switch(scope)
        {
            case ScopeRange.HumanTower:
                towerType = TowerType.Human;
                return true;
            case ScopeRange.ElfTower:
                towerType = TowerType.Elf;
                return true;
            case ScopeRange.OrcTower:
                towerType = TowerType.Orc;
                return true;
            case ScopeRange.BeastTower:
                towerType = TowerType.Werebeast;
                return true;
            case ScopeRange.DragonTower:
                towerType = TowerType.Dragonian;
                return true;
            case ScopeRange.DwarfTower:
                towerType = TowerType.Dwarf;
                return true;
            default:
                return false;
        }
    }
}
