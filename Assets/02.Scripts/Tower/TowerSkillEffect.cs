using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerSkillEffect
{
    private readonly Dictionary<TowerType, int> skillStep = new Dictionary<TowerType, int>();
    public event Action<TowerType, int, float> OnChangedTowerSkillStep;

    public void Init()
    {
        skillStep.Clear();

        foreach (TowerType towerType in System.Enum.GetValues(typeof(TowerType)))
        {
            skillStep[towerType] = 0;
        }
    }

    public void ChangeTowerCount(TowerType type, int cnt)
    {
        TowerSkillData skill = Managers.TowerSkill.GetTowerSkillDataByTypeAndCount(type, cnt);

        if (skill == null)
        {
            skillStep[type] = 0;
            OnChangedTowerSkillStep?.Invoke(type, 0, 0);
            return;
        }

        if (skillStep[type] != skill.step)
        {
            skillStep[type] = skill.step;
            OnChangedTowerSkillStep?.Invoke(type, skill.step, skill.effectValue);
        }
    }
}
