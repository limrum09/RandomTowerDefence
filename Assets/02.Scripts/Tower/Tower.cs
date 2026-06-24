using System.Collections;
using UnityEngine;
using UnityEngine.U2D.Animation;

/// <summary>
/// 필드에 배치되는 타워 객체
/// TowerData를 기반으로 타워 기본 정보 초기화
/// 현재 공격력 / 공격속도 계산
/// 타워 애니메이션 및 SpriteLibrary 설정
/// 공격 상태 애니메이션 제어
/// 공격 사거리 표시 제어
/// </summary>
public class Tower : MonoBehaviour
{
    [SerializeField]
    private Animator anim;                              // 타워 애니메이션 제어용 Animator
    [SerializeField]
    private SpriteRenderer spriteRenderer;              // 타워 Sprite 표시용 Renderer
    [SerializeField]
    private Sprite baseSprite;                          // 기본 Sprite
    [SerializeField]
    private Transform attackRangeIndicator;             // 공격 사거리 표시 오브젝트
    [SerializeField]
    private SpriteLibrary spriteLibrary;                // 타워 외형 교체용 SpriteLibrary
    [SerializeField]
    private RuntimeAnimatorController commonController; // 공통 타워 애니메이터 컨트롤러
    [SerializeField]
    private GitSpriteGlowCtr outlineEffectCtr;

    private TowerData data;
    private int index;                                  // 필드에 배치된 타워 고유 인덱스
    private string towerUID;            // 타워 UID
    private TowerType towerType;        // 타워 타입            
    private string stringKey;           // 타워 이름 로컬라이징
    private int grade;                  // 타워 등급
    private float baseAtk;              // 타워 기본 공격력
    private float baseAtkSpeed;         // 타워 기본 공격 속도
    private float range;                // 타워 공격 범위
    private CostType costType;          // 구매 비용 타입
    private int buyPrice;               // 구매 비용
    private int sellPrice;              // 판매 비용
    private string skillID;             // 타워 스킬 ID
    private string iconPath;            // 아이콘/스프라이트 위치
    public string nextGradeUID;         // 다음 등급 타워 UID

    private float currentDamage;        // 현제 공격력
    private float currentSpeed;         // 현제 공격 속도
    private float criticalPer;          // 크리티컬 확율
    private float enemySlowPer;         // 적 타격시 해당 적 이동속도 감소
    private float damageDebuffPer;
    private float attackSpeedDebuffPer;
    private string skillName;           // 타워 스킬이름 캐시용
    private string skillDes;            // 타워 설명 캐시용

    public string TowerUID => towerUID;
    public int Index => index;
    public TowerType Type => towerType;
    public string StringKey => stringKey;
    public int Grade => grade;
    public float BaseAtk => baseAtk;
    public float BaseAtkSpeed => baseAtkSpeed;
    public float AtkRange => range;
    public CostType CostTY => costType;
    public int BuyPrice => buyPrice;
    public int SellPrice => sellPrice;
    public string SkillID => skillID;
    public string IconPath => iconPath;
    public string NextGradeUID => nextGradeUID;
    public Sprite TowerBlockSprite => spriteLibrary.GetSprite("Block", "0");
    public float CriticalPer => criticalPer;
    public float EnemySlowPer => enemySlowPer;
    /// <summary>
    /// 현제 공격력
    /// 기본 공격력 + (공격력 증가 값 * 현제 런 강화 단계)
    /// </summary>
    public float CurrentDamage => currentDamage * (1f - damageDebuffPer / 100f);
    /// <summary>
    /// 현제 공격 속도
    /// 기본 공겨 속도 + (공격 속도 증가 값 * 현제 런 강화 단계)
    /// </summary>
    public float CurrentAtkSpeed => currentSpeed * (1f - attackSpeedDebuffPer / 100f);
    /// <summary>
    /// 스킬 이름 반환
    /// TowerSkillData의 StringKey를 로컬라이징 키로 사용하여 반환
    /// </summary>
    /// <returns>스킬 이름이 있다면 반환 없다면 string.Empty 반환</returns>
    public string SkillName()
    {
        skillName = string.Empty;

        TowerSkillData tempData = Managers.TowerSkill.GetTowerSkillData(skillID);

        string skillUID = tempData.Stringkey;
        skillName = Managers.Local.GetString("Tower", skillUID);

        return skillName;
    }
    /// <summary>
    /// 스킬 설명 반환
    /// 스킬 이름과 이하 동문
    /// </summary>
    /// <returns></returns>
    public string SkillDes()
    {
        skillDes = string.Empty;

        TowerSkillData tempData = Managers.TowerSkill.GetTowerSkillData(skillID);
        
        string skillDesUID = tempData.DesStringKey;
        skillDes = Managers.Local.GetString("Tower", skillDesUID);

        return skillDes;
    }

    /// <summary>
    /// 타워 애니메이션과 SpriteLibrary를 설정
    /// 공통 AnimatorController를 사용하고, 타워 타입 / 등급에 맞는 SpriteLibraryAsset을 Resources에서 로드
    /// </summary>
    private void SetAnimation()
    {
        if (anim == null)
            return;

        // 모든 타워가 사용하는 공통 AnimatorController 적용
        anim.runtimeAnimatorController = commonController;

        // 타워 외형 SpriteLibrary 로드
        SpriteLibraryAsset library = ResourceCache.Load<SpriteLibraryAsset>($"Tower/SpriteLibrary/{iconPath}/{iconPath}_{grade}");

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

        // 로드한 SpriteLibrary적용
        spriteLibrary.spriteLibraryAsset = library;

        // 타워 타입에 따른 Animator 파라미터 설정
        SetAnimatorTypeParameters();
    }

    /// <summary>
    /// 타워 타입에 따라 Animator 파라미터 설정
    /// Elf => 3등급 이상, 활 공격 애니메이션
    /// Dragonian => 마법 공격 애니메이션
    /// 나머지 => 기본 공격 애니메이션
    /// </summary>
    private void SetAnimatorTypeParameters()
    {
        if (anim == null)
            return;

        anim.SetFloat("AttackType", 0.0f);

        // 타입에 따른 특수 공격
        switch (towerType)
        {
            case TowerType.Elf:
                float isBow = grade >= 3 ? 1.0f : 0.0f;
                anim.SetFloat("AttackType", isBow);
                break;
            case TowerType.Dragonian:
                anim.SetFloat("AttackType", 2.0f);
                break;
        }
    }

    /// <summary>
    /// 공격 사거리 표시 오브젝트의크기를 타워의 사거리에 맞추기
    /// </summary>
    private void SetRangeVisiual()
    {
        if (attackRangeIndicator == null)
            return;

        // 원형 사거리기에 사거리 * 2 로 해서 지름 설정
        float d = AtkRange * 2f;
        attackRangeIndicator.localScale = new Vector3(d, d, 1f);
    }

    private void GetDamageDebuff(float debuffPer, float duration)
    {
        StopCoroutine(nameof(DamageDebuff));
        StartCoroutine(DamageDebuff(debuffPer, duration));
    }

    private void GetAttackSpeedDebuff(float debuffPer, float duration)
    {
        StopCoroutine(nameof(AttackSpeedDebuff));
        StartCoroutine(AttackSpeedDebuff(debuffPer, duration));
    }

    IEnumerator DamageDebuff(float debuffPer, float duration)
    {
        damageDebuffPer = Mathf.Clamp(debuffPer, 0f, 100f);
        yield return new WaitForSeconds(duration);
        damageDebuffPer = 0.0f;
    }

    IEnumerator AttackSpeedDebuff(float debuffPer, float duration)
    {
        attackSpeedDebuffPer = Mathf.Clamp(debuffPer, 0f, 100f);
        yield return new WaitForSeconds(duration);
        attackSpeedDebuffPer = 0.0f;
    }

    /// <summary>
    /// 타워 초기화
    /// TowerDataManager에서 towerUID에 해당하는 데이터를 가져와 타워의 기본 정보와 강화 증가값을 설정
    /// </summary>
    /// <param name="getTowerUID"></param>
    /// <param name="getIndex"></param>
    /// <param name="getStatManager"></param>
    public void Init(string getTowerUID, int getIndex)
    {
        // 타워 UID와 고유 인덱스 저장
        towerUID = getTowerUID;
        index = getIndex;

        // 타워 정적 데이터 조회
        data = Managers.TowerData.GetTowerData(towerUID);

        if (data == null)
        {
            Debug.LogError($"Tower Data Missing : {towerUID}");
            return;
        }   

        // TowerData값 복사
        towerType = data.towerType;
        stringKey = data.stringKey;
        grade = data.grade;
        baseAtk = Managers.Game.GetTowerDisplayData(data).currentValue2;
        baseAtkSpeed = Managers.Game.GetTowerDisplayData(data).currentValue1;
        range = data.range;
        costType = data.costType;
        buyPrice = data.buyPrice;
        sellPrice = data.sellPrice;
        skillID = data.skillID ;
        iconPath = data.iconPath;
        nextGradeUID = data.nextGradeUID;

        criticalPer = 0.0f;

        // 공격력 및 공격속도 가져오기
        RefreshStats();
        // 외형 / 애니메이션 설정
        SetAnimation();
        // 사거리 표시 크기 설정
        SetRangeVisiual();
        // 기본 상태에서는 사거리 표시 숨김
        ShowAttackRange(false);
    }

    /// <summary>
    /// 타워의 공격력 및 공격속도 새로고침
    /// </summary>
    public void RefreshStats()
    {
        TowerStatPreview stat = TowerStatCalculator.Calculate(data);

        currentDamage = stat.damage;
        currentSpeed = stat.attackSpeed;
        criticalPer = stat.criticalPer;
        enemySlowPer = stat.enemySlow;
    }

    /// <summary>
    /// 타워가 공격 상태 변경
    /// IsAttack이 true이면 공격 애니메이션 상태로 전환, 현재 공격속도를 Animator 파라미터에 저장
    /// </summary>
    /// <param name="isAttack"></param>
    public void Attack()
    {
        if (anim == null)
            return;

        anim.SetTrigger("IsAttack");
        // 공격 속도에 맞춰 공격 애니메이션 속도 조정
        anim.SetFloat("AttackSpeed", CurrentAtkSpeed);
    }

    /// <summary>
    /// 타워 공격 사거리 표시 여부 결정
    /// </summary>
    /// <param name="show">true면 표시, false면 숨김</param>
    public void ShowAttackRange(bool show)
    {
        if (attackRangeIndicator == null)
            return;

        attackRangeIndicator.gameObject.SetActive(show);

        if (show)
            outlineEffectCtr.ShowGlowEffect();
        else
            outlineEffectCtr.HideGlowEffect();
    }

    public void GetDubuff(EnemySkillType skillType, float debuffPer, float duration)
    {
        if(skillType == EnemySkillType.AttackDebuff)
        {
            GetDamageDebuff(debuffPer, duration);
        }
        else if(skillType == EnemySkillType.SpeedDebuff)
        {
            GetAttackSpeedDebuff(debuffPer, duration);
        }
    }
    
}
