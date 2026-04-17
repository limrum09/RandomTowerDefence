using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerDataRow
{
    public string TowerUID;
    public string TowerType;
    public string StringKey;
    public int Grade;
    public int BaseAtk;
    public float BaseAtkSpeed;
    public float Range;
    public string CostType;
    public int BuyPrice;
    public int SellPrice;
    public string SkillID;
    public string IconPath;
    public string NextGradeUID;
}

[Serializable]
public class TowerDataRowList
{
    public List<TowerDataRow> datas = new List<TowerDataRow>();
}