using System.Runtime.CompilerServices;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Sprite baseSprite;
    [SerializeField]
    private Transform attackRangeIndicator;
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
    public string nextGradeUID;

    private string towerName;
    private string skillName;
    private string skillDes;

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
    public string NextGradeUID => nextGradeUID;
    public int CurrentDamage => baseAtk;
    public float CurrentAtkSpeed => baseAtkSpeed;
    public string TowerName => towerName;
    public string SkillName => skillName;
    public string SkillDes => skillDes;


    private void SetAnimation()
    {
        string path = iconPath;

        AnimatorDatas aniDatas = Resources.Load<AnimatorDatas>("Anis/AnimatorDatas");

        if (aniDatas == null)
        {
            spriteRenderer.sprite = baseSprite;
            anim.runtimeAnimatorController = null;
            return;
        }

        RuntimeAnimatorController aniController = aniDatas.FindByCode(path + "Ani");

        if (aniController == null)
        {
            spriteRenderer.sprite = baseSprite;
            anim.runtimeAnimatorController = null;
            return;
        }

        anim.runtimeAnimatorController = aniController;
    }

    /*private void SetTowerStringInfo()
    {
        towerName;
        skillName;
        skillDes;
    }*/

    private void SetRangeVisiual()
    {
        if (attackRangeIndicator == null)
            return;

        float d = AtkRange * 2f;
        attackRangeIndicator.localScale = new Vector3(d, d, 1f);
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
        nextGradeUID = data.nextGradeUID;

        SetAnimation();
        SetRangeVisiual();
        ShowAttackRange(false);
    }

    /// <summary>
    /// 타워가 공격 중
    /// </summary>
    /// <param name="isAttack"></param>
    public void Attack(bool isAttack)
    {
        if (anim == null)
            return;

        anim.SetBool("IsAttack", isAttack);
    }

    public void ShowAttackRange(bool show)
    {
        if (attackRangeIndicator == null)
            return;

        attackRangeIndicator.gameObject.SetActive(show);
    }
}
