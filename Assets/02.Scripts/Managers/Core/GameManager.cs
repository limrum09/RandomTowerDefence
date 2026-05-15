using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MetaUpgradeDisplayData
{
    public int level1;
    public int level2;
    public float currentValue1;
    public float currentValue2;
    public float nextValue1;
    public float nextValue2;
    public int costValue1;
    public int costValue2;
    public bool useSecondValue;
}

public class GameManager
{
    /*
        전역적인 값만 사용 
        현재 선택된 Stage ID
        나중에 메인씬 → 스테이지씬 이동
        전역 재화/영구 성장
        세이브 연결
        옵션/설정
        전체 게임 상태
      */

    private TowerMetaUpgradeManager towerMetaManager;
    private PublicMetaUpgradeManager publicMetaManager;
    private MetaResearchDataManager metaData;
    private StageStartOptionBaseDataManager startOption;

    public string selectDifficultyLevel { get; private set; }

    public void Init()
    {
        selectDifficultyLevel = string.Empty;
        towerMetaManager = Managers.TowerMetaUpgrade;
        publicMetaManager = Managers.PublicMetaUpgrade;
        metaData = Managers.ResearchData;
        startOption = Managers.StartOption;
    }

    public void SelectStageDifficultyLevel(string getDifficulty)
    {
        selectDifficultyLevel = getDifficulty;
    }

    public MetaUpgradeDisplayData GetTowerDisplayData(TowerData tower)
    {
        int damageLevel = towerMetaManager.GetDamageLevel(tower.towerType, tower.grade);
        int speedLevel = towerMetaManager.GetAttackSpeedLevel(tower.towerType, tower.grade);

        MetaResearchData damageData = metaData.GetMetaResearchDataToTower(tower.towerUID, MetaUpgradeTarget.Tower, MetaUpgradeType.Damage);
        MetaResearchData speedData = metaData.GetMetaResearchDataToTower(tower.towerUID, MetaUpgradeTarget.Tower, MetaUpgradeType.AttackSpeed);

        return new MetaUpgradeDisplayData()
        {
            level1 = speedLevel,
            level2 = damageLevel,
            currentValue1 = speedData.CalculateValue(tower.baseAtkSpeed, speedLevel),
            currentValue2 = damageData.CalculateValue(tower.baseAtk, damageLevel),
            nextValue1 = speedData.CalculateValue(tower.baseAtkSpeed, speedLevel + 1),
            nextValue2 = damageData.CalculateValue(tower.baseAtk, damageLevel + 1),
            costValue1 = Mathf.CeilToInt(speedData.costBase * Mathf.Pow(speedData.costGrow, speedLevel)),
            costValue2 = Mathf.CeilToInt(damageData.costBase * Mathf.Pow(damageData.costGrow, damageLevel)),
            useSecondValue = true
        };
    }

    public MetaUpgradeDisplayData GetPublicDisplayData(MetaUpgradeType type)
    {
        int level = publicMetaManager.GetPublicMetaDataLevel(type);

        MetaResearchData publicData = metaData.GetMetaResearchDataToPublic(MetaUpgradeTarget.Public, type);
        StageStartOptionBaseData baseData = startOption.GetStartOptionData(type);

        return new MetaUpgradeDisplayData()
        {
            level1 = level,
            level2 = -9999,
            currentValue1 = publicData.CalculateValue(baseData.baseValue, level),
            currentValue2 = 0.0f,
            nextValue1 = publicData.CalculateValue(baseData.baseValue, level + 1),
            nextValue2 = 0.0f,
            costValue1 = Mathf.CeilToInt(publicData.costBase * Mathf.Pow(publicData.costGrow, level)),
            costValue2 = 99999,
            useSecondValue = false
        };
    }
}
