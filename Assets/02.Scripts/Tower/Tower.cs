using UnityEngine;
using UnityEngine.U2D.Animation;

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
    [SerializeField]
    private SpriteLibrary spriteLibrary;
    [SerializeField]
    private RuntimeAnimatorController commonController;


    private int index;
    private RunStatUpgradeManager statUpgrade;

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

    private float increaseAtkDamage;
    private float increaseAtkSpeed;

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
    public int CurrentDamage => baseAtk + (int)(increaseAtkDamage * (statUpgrade.GetAtkDamageStep(towerType) 
        + statUpgrade.GetItemAtkDamageStep(towerType) + statUpgrade.GetSkillAtkDamageStep(towerType)));
    public float CurrentAtkSpeed => baseAtkSpeed + (increaseAtkSpeed * (statUpgrade.GetAtkSpeedStep(towerType) 
        + statUpgrade.GetItemAtkSpeedStep(towerType) + statUpgrade.GetSkillAtkSpeedStep(towerType)));
    public RunStatUpgradeManager StatUpgrade => statUpgrade;
    public string TowerName()
    {
        return Managers.Local.GetString(stringKey);
    }
    public string SkillName()
    {
        skillName = string.Empty;

        TowerSkillData tempData = Managers.TowerSkill.GetTowerSkillData(skillID);

        string skillUID = tempData.Stringkey;
        skillName = Managers.Local.GetString(skillUID);

        return skillName;
    }
    public string SkillDes()
    {
        skillDes = string.Empty;

        TowerSkillData tempData = Managers.TowerSkill.GetTowerSkillData(skillID);
        
        string skillDesUID = tempData.DesStringKey;
        skillDes = Managers.Local.GetString(skillDesUID);

        return skillDes;
    }


    private void SetAnimation()
    {
        if (anim == null)
            return;

        anim.runtimeAnimatorController = commonController;

        SpriteLibraryAsset library = Resources.Load<SpriteLibraryAsset>($"Tower/SpriteLibrary/{iconPath}/{iconPath}_{grade}");

        if(spriteLibrary == null)
        {
            Debug.LogWarning("Sprite Library 로드 실패 : ");
            return;
        }

        if(library == null)
        {
            Debug.LogWarning("Library 로드 실패 : ");
            return;
        }

        spriteLibrary.spriteLibraryAsset = library;

        SetAnimatorTypeParameters();
    }

    private void SetAnimatorTypeParameters()
    {
        if (anim == null)
            return;

        anim.SetBool("IsBow", false);
        anim.SetBool("IsMagic", false);

        switch (towerType)
        {
            case TowerType.Elf:
                anim.SetBool("IsBow", true);
                break;
            case TowerType.Dragonian:
                anim.SetBool("IsMagic", true);
                break;
        }
    }

    private void SetRangeVisiual()
    {
        if (attackRangeIndicator == null)
            return;

        float d = AtkRange * 2f;
        attackRangeIndicator.localScale = new Vector3(d, d, 1f);
    }

    public void Init(string getTowerUID, int getIndex, RunStatUpgradeManager getStatManager)
    {
        statUpgrade = getStatManager;
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

        increaseAtkDamage = Managers.SessionTowerUpgrade.GetUpgradeStepData(towerUID, UpgradeType.Damge).increaseValue;
        increaseAtkSpeed = Managers.SessionTowerUpgrade.GetUpgradeStepData(towerUID, UpgradeType.Speed).increaseValue;

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
