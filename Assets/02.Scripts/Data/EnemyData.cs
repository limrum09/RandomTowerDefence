using UnityEngine;

public class EnemyData
{
    public string enemyUID;
    public string stringKey;
    public string enemySkillUID;
    public int basicHp;
    public int increaseHP;
    public float moveSpeed;
    public int basicShield;
    public int increaseShield;
    public float rewardGold;
    public string iconPath;

    public EnemyData(string enemyUID, string stringKey, string enemySkillUID, int basicHp, 
        int increaseHP, float moveSpeed, int basicShield, int increaseShield, float rewardGold, string iconPath)
    {
        this.enemyUID = enemyUID;
        this.stringKey = stringKey;
        this.enemySkillUID = enemySkillUID;
        this.basicHp = basicHp;
        this.increaseHP = increaseHP;
        this.moveSpeed = moveSpeed;
        this.basicShield = basicShield;
        this.increaseShield = increaseShield;
        this.rewardGold = rewardGold;
        this.iconPath = iconPath;
    }
}
