using JetBrains.Annotations;
using System;
using System.Collections.Generic;
public class TowerDataManager
{
    Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
    Dictionary<int, string[]> tempTowerGradeUID = new Dictionary<int, string[]>();

    public void Init()
    {
        towerDatas.Clear();

        towerDatas["T0011"] = new TowerData("T0011", TowerType.Human, "TOWER_NAME_001", 1, 12, 1.5f, 4f, CostType.Gold, 50, 25, "S001", "Human", "T0012");
        towerDatas["T0012"] = new TowerData("T0012", TowerType.Human, "TOWER_NAME_001", 2, 20, 1.5f, 4f, CostType.Gold, 200, 100, "S001", "Human", "Master");
        towerDatas["T0021"] = new TowerData("T0021", TowerType.Elf, "TOWER_NAME_002", 1, 20, 2.0f, 4f, CostType.Gold, 100, 50, "S002", "Elf", "T0022");
        towerDatas["T0022"] = new TowerData("T0022", TowerType.Elf, "TOWER_NAME_002", 2, 20, 2.0f, 4f, CostType.Gold, 100, 50, "S002", "Elf", "Master");
        towerDatas["T0031"] = new TowerData("T0031", TowerType.Dwarf, "TOWER_NAME_003", 1, 15, 1.5f, 3f, CostType.Gold, 150, 75, "S003", "Dwarf", "T0032");
        towerDatas["T0032"] = new TowerData("T0032", TowerType.Dwarf, "TOWER_NAME_003", 2, 15, 1.5f, 3f, CostType.Gold, 150, 75, "S003", "Dwarf", "Master");
        towerDatas["T0041"] = new TowerData("T0041", TowerType.Orc, "TOWER_NAME_004", 1, 40, 1.5f, 2f, CostType.Gold, 150, 75, "S004", "Orc", "T0042");
        towerDatas["T0042"] = new TowerData("T0042", TowerType.Orc, "TOWER_NAME_004", 2, 40, 1.5f, 2f, CostType.Gold, 150, 75, "S004", "Orc", "Master");
        towerDatas["T0051"] = new TowerData("T0051", TowerType.Dragonian, "TOWER_NAME_005", 1, 50, 1.5f, 3f, CostType.Gold, 200, 100, "S005", "Dragonian", "T0052");
        towerDatas["T0052"] = new TowerData("T0052", TowerType.Dragonian, "TOWER_NAME_005", 2, 50, 1.5f, 3f, CostType.Gold, 200, 100, "S005", "Dragonian", "Master");
        towerDatas["T0061"] = new TowerData("T0061", TowerType.WereBeast, "TOWER_NAME_006", 1, 30, 1.5f, 4f, CostType.Gold, 100, 50, "S006", "Werebeast", "T0062");
        towerDatas["T0062"] = new TowerData("T0062", TowerType.WereBeast, "TOWER_NAME_006", 2, 30, 1.5f, 4f, CostType.Gold, 100, 50, "S006", "Werebeast", "Master");

        tempTowerGradeUID[1] = new string[] { "T0012", "T0022", "T0032", "T0042", "T0052", "T0062" };
    }

    public TowerData GetTowerData(string towerId)
    {
        if(towerDatas.TryGetValue(towerId, out TowerData towerData))
                return towerData;

        return null;
    }

    public string[] GetTowerNextGradeUID(int currnetGrade)
    {
        if (tempTowerGradeUID.TryGetValue(currnetGrade, out string[] uids))
            return uids;

        return null;
    }
}
