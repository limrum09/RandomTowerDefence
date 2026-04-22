using System;
using System.Collections.Generic;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Windows.Speech;

public enum UpgradeType
{
    None,
    Damge,
    Speed
}

[Serializable]
public class TowerSessionUpgradeDataRow
{
    public string Tower_UID;
    public int Tower_Grade;
    public string Upgrade_Type;
    public float Increase_Value;
    public int Base_Cost;
    public int Increase_Cost;
}

[Serializable]
public class TowerSessionUpgradeDataRowList
{
    public List<TowerSessionUpgradeDataRow> datas = new List<TowerSessionUpgradeDataRow>();
}

public class TowerSessionUpgradeData
{
    public string towerUID;
    public int towerGrade;
    public UpgradeType upgradeType;
    public float increaseValue;
    public int baseCost;
    public int increaseCost;

    public TowerSessionUpgradeData(string towerUID, int towerGrade, UpgradeType upgradeType, float increaseValue, int baseCost, int increaseCost)
    {
        this.towerUID = towerUID;
        this.towerGrade = towerGrade;
        this.upgradeType = upgradeType;
        this.increaseValue = increaseValue;
        this.baseCost = baseCost;
        this.increaseCost = increaseCost;
    }
}

public class TowerSessionUpgradeManager
{
    Dictionary<string, Dictionary<UpgradeType, TowerSessionUpgradeData>> sessionUpgradeDatas =
        new Dictionary<string, Dictionary<UpgradeType, TowerSessionUpgradeData>>();

    private void GetTowerSessionUpgradeDataToJson()
    {
        TowerSessionUpgradeDataRowList rowList = 
            JsonLoader.LoadFromResources<TowerSessionUpgradeDataRowList>("Data/TowerSessionUpgradeData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(TowerSessionUpgradeDataRow row in rowList.datas)
        {
            if (!Enum.TryParse(row.Upgrade_Type, true, out UpgradeType upgradeType))
                continue;

            TowerSessionUpgradeData data = new TowerSessionUpgradeData(row.Tower_UID, row.Tower_Grade, upgradeType
                , row.Increase_Value, row.Base_Cost, row.Increase_Cost);

            if(!sessionUpgradeDatas.TryGetValue(data.towerUID, out Dictionary<UpgradeType, TowerSessionUpgradeData> upgradeMap))
            { 
                upgradeMap = new Dictionary<UpgradeType, TowerSessionUpgradeData>();
                sessionUpgradeDatas[data.towerUID] = upgradeMap;
            }

            upgradeMap[data.upgradeType] = data;
        }
    }

    public void Inti()
    {
        sessionUpgradeDatas.Clear();
        GetTowerSessionUpgradeDataToJson();
    }

    public TowerSessionUpgradeData GetUpgradeStepData(string uid, UpgradeType type)
    {
        if(sessionUpgradeDatas.TryGetValue(uid, out Dictionary<UpgradeType, TowerSessionUpgradeData> upgradeMap))
        {
            if (upgradeMap.TryGetValue(type, out TowerSessionUpgradeData data))
                return data;
        }

        return null;
    }
}
