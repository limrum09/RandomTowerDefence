using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
public class TowerDataManager
{
    Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
    Dictionary<int, string[]> tempTowerGradeUID = new Dictionary<int, string[]>();

    private void GetTowerDataToJson()
    {
        TowerDataRowList rowList = JsonLoader.LoadFromResources<TowerDataRowList>("Data/TowerData");

        if (rowList == null || rowList.datas == null)
            return;

        foreach(TowerDataRow row in rowList.datas)
        {
            if (!Enum.TryParse(row.TowerType, true, out TowerType towerType))
                continue;

            if (!Enum.TryParse(row.CostType, true, out CostType costType))
                continue;

            TowerData data = new TowerData(
                row.TowerUID,
                towerType,
                row.StringKey,
                row.Grade,
                row.BaseAtk,
                row.BaseAtkSpeed,
                row.Range,
                costType,
                row.BuyPrice,
                row.SellPrice,
                row.SkillID,
                row.IconPath,
                row.NextGradeUID
            );

            towerDatas[row.TowerUID] = data;
        }
    }

    public void Init()
    {
        towerDatas.Clear();
        tempTowerGradeUID.Clear();

        GetTowerDataToJson();

        Debug.Log("Tower Data Count : " + towerDatas.Count);

        tempTowerGradeUID[1] = new string[] { "T0011", "T0021", "T0031", "T0041", "T0051", "T0061" };
        tempTowerGradeUID[2] = new string[] { "T0012", "T0022", "T0032", "T0042", "T0052", "T0062" };
        tempTowerGradeUID[3] = new string[] { "T0013", "T0023", "T0033", "T0043", "T0053", "T0063" };
        tempTowerGradeUID[4] = new string[] { "T0014", "T0024", "T0034", "T0044", "T0054", "T0064" };
        tempTowerGradeUID[5] = new string[] { "T0015", "T0025", "T0035", "T0045", "T0055", "T0065" };
        tempTowerGradeUID[6] = new string[] { "T0016", "T0026", "T0036", "T0046", "T0056", "T0066" };
    }

    public TowerData GetTowerData(string towerId)
    {
        if(towerDatas.TryGetValue(towerId, out TowerData towerData))
                return towerData;

        return null;
    }

    public string[] GetTowerGradeUID(int currnetGrade)
    {
        if (tempTowerGradeUID.TryGetValue(currnetGrade, out string[] uids))
            return uids;

        return null;
    }
}
