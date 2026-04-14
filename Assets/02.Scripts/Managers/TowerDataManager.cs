using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.AI;

public class TowerDataManager
{
    Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();

    public void Init()
    {
        towerDatas.Clear();

        towerDatas["T0011"] = new TowerData("T0011", TowerType.Human, "TOWER_NAME_001", 1, 12, 1.5f, 4f, CostType.Gold, 50, 25, "S001", "");
        towerDatas["T0012"] = new TowerData("T0012", TowerType.Human, "TOWER_NAME_001", 1, 20, 1.5f, 4f, CostType.Gold, 200, 100, "S001", "");
        towerDatas["T0021"] = new TowerData("T0021", TowerType.Elf, "TOWER_NAME_002", 1, 20, 2.0f, 4f, CostType.Gold, 100, 50, "S002", "");
        towerDatas["T0031"] = new TowerData("T0031", TowerType.Dwarf, "TOWER_NAME_003", 1, 15, 1.5f, 3f, CostType.Gold, 150, 75, "S003", "");
        towerDatas["T0041"] = new TowerData("T0041", TowerType.Orc, "TOWER_NAME_004", 1, 40, 1.5f, 2f, CostType.Gold, 150, 75, "S004", "");
        towerDatas["T0051"] = new TowerData("T0051", TowerType.Dragonian, "TOWER_NAME_005", 1, 50, 1.5f, 3f, CostType.Gold, 200, 100, "S005", "");
        towerDatas["T0061"] = new TowerData("T0061", TowerType.WereBreast, "TOWER_NAME_006", 1, 30, 1.5f, 4f, CostType.Gold, 100, 50, "S006", "");
    }

    public TowerData GetTowerData(string towerId)
    {
        if(towerDatas.TryGetValue(towerId, out TowerData towerData))
                return towerData;

        return null;
    }
}
