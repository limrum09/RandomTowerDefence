using System;

public enum TowerType
{
    Human,
    Elf,
    Dwarf,
    Orc,
    Dragonian,
    Werebeast
}

public enum CostType
{
    Gold,
    MetaEXP
}
[Serializable]
public class TowerData
{
    public string towerUID;
    public TowerType towerType;
    public string stringKey;
    public int grade;
    public int baseAtk;
    public float baseAtkSpeed;
    public float range;
    public CostType costType;
    public int buyPrice;
    public int sellPrice;
    public string skillID;
    public string iconPath;
    public string nextGradeUID;

    public string TowerUID => towerUID;

    public TowerData(string getUID,  TowerType getType, string getStringKey, int getGrade,
        int getBaseAtk, float getBaseAtkSpeed, float getRange, CostType getCostType, int getBuyPrice,
        int getSellPirce, string getSkillID, string getIconPath, string GetNextGradeUID)
    {
        towerUID = getUID;
        towerType = getType;
        stringKey = getStringKey;
        grade = getGrade;
        baseAtk = getBaseAtk;
        baseAtkSpeed = getBaseAtkSpeed;
        range = getRange;
        costType = getCostType;
        buyPrice = getBuyPrice;
        sellPrice = getSellPirce;
        skillID = getSkillID;
        iconPath = getIconPath;
        nextGradeUID = GetNextGradeUID;
    }
}
