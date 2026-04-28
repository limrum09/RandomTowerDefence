using System.IO;
using System.Net.NetworkInformation;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyMove move;
    [SerializeField]
    private EnemyAnim anim;


    [SerializeField]
    private string enemyUID;
    [SerializeField]
    private int level;
    [SerializeField]
    private string stringKey;
    [SerializeField]
    private string enemySkillUID;
    [SerializeField]
    private int basicHp;
    [SerializeField]
    private int increaseHP;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private int basicShield;
    [SerializeField]
    private int increaseShield;
    [SerializeField]
    private float rewardGold;
    [SerializeField]
    private string iconPath;

    [SerializeField]
    private int maxHP;
    [SerializeField]
    private int currentHP;
    [SerializeField]
    private int maxShield;
    [SerializeField]
    private int currentShield;
    private bool isDead;

    public int Level => level;
    public string StringKey => stringKey;
    public string EnemySkillUID => enemySkillUID;
    public int MaxHP => maxHP;
    public int MaxShield => maxShield;
    public float MoveSpeed => moveSpeed;
    public float RewardGold => rewardGold;
    public bool IsDead => isDead;

    public void Init(string uid, int getLevel)
    {
        level = getLevel;
        enemyUID = uid;

        EnemyData data = Managers.EnemyData.GetEnemyData(enemyUID);

        if(data == null)
        {
            Destroy(gameObject);
        }

        enemyUID = data.enemyUID;
        stringKey = data.stringKey;
        enemySkillUID = data.enemySkillUID;
        basicHp = data.basicHp;
        increaseHP = data.increaseHP;
        moveSpeed = data.moveSpeed;
        basicShield = data.basicShield;
        increaseShield = data.increaseShield;
        rewardGold = data.rewardGold;
        iconPath = data.iconPath;
        isDead = false;

        SetState();
        anim.SetAnim(uid);
    }

    private void SetState()
    {
        maxHP = currentHP = basicHp + (increaseHP * level);
        maxShield = currentShield = basicShield + (increaseShield * level);
    }

    private void Die()
    {
        move.IsDead((int)RewardGold);
        anim.Die();

        Invoke("Dead", 1f);
    }

    private void Dead()
    {
        Destroy(this.gameObject);
    }

    public void EnemyGeTakeDamage(int damage)
    {
        if(currentShield > 0)
        {
            currentShield -= damage;

            if (currentShield > 0)
                return;
            else
            {
                currentHP += currentShield;
                return;
            }
        }

        currentHP -= damage;

        if(currentHP <= 0)
        {
            Die();
            return;
        }
    }
}
