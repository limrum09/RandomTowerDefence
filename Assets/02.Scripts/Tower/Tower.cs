using System.Runtime.CompilerServices;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private int index;

    private string towerUID;
    private TowerType towerType;
    private string stringKey;
    private int grade;
    private int baseAtk;
    private float baseAtkSpeed;
    private float range;
    private CostType costType;
    private int buyPrice;
    private int sellPrice;
    private string skillID;
    private string iconPath;

    public string TowerUID => towerUID;
    public int Index => index;
    public TowerType Type => towerType;
    public string StringKey => stringKey;
    public int Grade => grade;
    public int BaseAtk => baseAtk;
    public float BaseAtkSpeed => baseAtkSpeed;
    public float AtkRange => range;
    public CostType CostTY => costType;
    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
    public string SkillID => skillID;
    public string IconPath => iconPath;

    private void Update()
    {
        
    }

    private void Attack()
    {

    }

    public void Init(string getTowerUID, int getIndex)
    {
        towerUID = getTowerUID;
        index = getIndex;

        TowerData data = Managers.TowerData.GetTowerData(towerUID);

        towerType = data.towerType;
        stringKey = data.stringKey;
        grade = data.grade;
        baseAtk = data.baseAtk;
        baseAtkSpeed = data.baseAtkSpeed;
        range = data.range;
        costType = data.costType;
        buyPrice = data.buyPrice;
        sellPrice = data.sellPrice;
        skillID = data.skillID ;
        iconPath = data.iconPath;
    }
}
