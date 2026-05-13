using System;
using System.Collections.Generic;

[Serializable]
public class TowerUpgradeSaveData
{
    public TowerType type;
    public int grade;
    public int damageLevel;
    public int attackSpeedLevel;
}

[Serializable]
public class TowerMetaUpgradeData
{
    public List<TowerUpgradeSaveData> upgrades = new List<TowerUpgradeSaveData>();
}

public class TowerMetaUpgradeManager
{
    private TowerMetaUpgradeData upgradeData = new TowerMetaUpgradeData();

    private TowerUpgradeSaveData GetSaveData(TowerType getType, int getGrade)
    {
        TowerUpgradeSaveData data = upgradeData.upgrades.Find(x => x.type == getType && x.grade == getGrade);

        if(data == null)
        {
            data = new TowerUpgradeSaveData
            {
                type = getType,
                grade = getGrade,
                attackSpeedLevel = 0,
                damageLevel = 0
            };

            upgradeData.upgrades.Add(data);
        }

        return data;
    }

    public void Init(TowerMetaUpgradeData getUpgradeData)
    {
        upgradeData = getUpgradeData != null ? getUpgradeData : new TowerMetaUpgradeData();
    }

    public int GetDamageLevel(TowerType getType, int getGrade)
    {
        var data = GetSaveData(getType, getGrade);

        return data.damageLevel;
    }

    public int GetAttakSpeedLevel(TowerType getType, int getGrade)
    {
        var data = GetSaveData(getType, getGrade);

        return data.attackSpeedLevel;
    }

    public void TowerDamageUpgrade(TowerType getType, int getGrade, int upValue)
    {
        var data = GetSaveData(getType, getGrade);
        data.damageLevel += upValue;
    }

    public void TowerAttackSpeedUpgrade(TowerType getType, int getGrade, int upValue)
    {
        var data = GetSaveData(getType, getGrade);
        data.attackSpeedLevel += upValue;
    }

    public TowerMetaUpgradeData GetTowerUpgradeSaveData()
    {
        return upgradeData;
    }
}
