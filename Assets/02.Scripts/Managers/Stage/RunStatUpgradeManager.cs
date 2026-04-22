using System.Collections.Generic;
using UnityEngine;

public class RunStatUpgradeManager
{
    private readonly Dictionary<TowerType, int> atkDamageTowerStep = new Dictionary<TowerType, int>();
    private readonly Dictionary<TowerType, int> atkSpeedTowerStep = new Dictionary<TowerType, int>();

    public int GlobalAtkDamageStep;
    public int GlobalAtkSpeedStep;

    public int GoldDropValue;
    public float InterestPer;
    public int AbilityTriggerRequest;

    public void Init()
    {
        Reset();
    }

    public void Reset()
    {
        atkDamageTowerStep.Clear();
        atkSpeedTowerStep.Clear();

        GlobalAtkDamageStep = 0;
        GlobalAtkSpeedStep = 0;

        GoldDropValue = 0;
        InterestPer = 0;
        AbilityTriggerRequest = 0;

        foreach(TowerType towerType in System.Enum.GetValues(typeof(TowerType)))
        {
            atkDamageTowerStep[towerType] = 0;
            atkSpeedTowerStep[towerType] = 0;
        }
    }

    public void AddStatAtkDamage(TowerType type, int value)
    {
        atkDamageTowerStep[type] += value;
    }

    public void AddStatAtkSpeed(TowerType type, int value)
    {
        atkSpeedTowerStep[type] += value;
    }

    public void AddItemAtkDamage(ScopeRange scope, int value)
    {
        if(scope == ScopeRange.AllTower)
        {
            GlobalAtkDamageStep += value;
            return;
        }

        if(TryConvertScopeToTowerType(scope, out TowerType type))
            atkDamageTowerStep[type] += value;
    }

    public void AddItemAtkSpeed(ScopeRange scope, int value)
    {
        if(scope == ScopeRange.AllTower)
        {
            GlobalAtkSpeedStep += value;
            return;
        }

        if (TryConvertScopeToTowerType(scope, out TowerType type))
            atkSpeedTowerStep[type] += value;
    }

    public void AddGoldDropIncrease(int value) => GoldDropValue += value;
    public void AddInterestBoost(float value) => InterestPer += value;
    public void RemoveAtkDamageStep(ScopeRange scope, int value) => AddItemAtkDamage(scope, -value);
    public void RemoveAtkSpeedStep(ScopeRange scope, int value) => AddItemAtkSpeed(scope, -value);
    public void RemoveGoldDropIncrease(int value) => GoldDropValue -= value;
    public void RemoveInterestBoost(float value) => InterestPer -= value;

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
                towerType = TowerType.WereBeast;
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
