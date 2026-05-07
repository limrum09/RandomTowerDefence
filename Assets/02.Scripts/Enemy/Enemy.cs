using UnityEngine;

/// <summary>
/// 적의 기본 데이터를 관리하는 클래스
/// EnemyData를 기반으로 적 정보 초기화
/// HP, Shield, MoveSpeed 계산
/// 회복, 보호막, 이동속도 변화처리
/// 데이지, 사망 처리
/// </summary>
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private EnemyMove move;     // 적 이동 담당 컴포넌트
    [SerializeField]
    private EnemyAnim anim;     // 적 애니메이션 담당 컴포넌트
    [SerializeField]
    private EnemySkill skill;   // 적 스킬 담당 컴포넌트


    [SerializeField]
    private string enemyUID;    // 적 UID
    [SerializeField]
    private int level;          // 적 레벨
    [SerializeField]
    private string stringKey;   // 적 이름 로컬라이징
    [SerializeField]
    private string enemySkillUID;// 적 스킬 UID
    [SerializeField]
    private int basicHp;        // 기본 체력
    [SerializeField]
    private int increaseHP;     // 레벨당 증가 체력
    [SerializeField]
    private float moveSpeed;    // 기본 이동 속도
    [SerializeField]
    private int basicShield;    // 기본 보호막
    [SerializeField]
    private int increaseShield; // 레벨 당 증가 보호막
    [SerializeField]
    private float rewardGold;   // 처치시 지급 골드
    [SerializeField]
    private string iconPath;    // 아이콘 경로

    [Header("Runitme Stat")]
    [SerializeField]
    private int maxHP;          // 최대 체력
    [SerializeField]
    private int currentHP;      // 현재 체력
    [SerializeField]
    private int maxShield;      // 최대 보호막
    [SerializeField]
    private int currentShield;  // 현재 보호막
    [SerializeField]
    private float currentSpeed; // 현재 이동 속도
    private bool isDead;        // 죽었는지 판단
    private float increaseSpeed;// 스킬 등으로 증가 / 감소하는 이동속도 보정값

    public int Level => level;
    public string StringKey => stringKey;
    public string EnemySkillUID => enemySkillUID;
    public int MaxHP => maxHP;
    public int MaxShield => maxShield;
    public float RewardGold => rewardGold;
    public bool IsDead => isDead;
    /// <summary>
    /// 실제 이동속도
    /// 시본 속도 + 스킬 / 버프 보정값
    /// </summary>
    public float MoveSpeed => currentSpeed + increaseSpeed;

    /// <summary>
    /// 적 초기화
    /// EnemyDataManger에서 uid에 해당하는 데이터를 가져와 적의 기본 정보, 스탯, 스킬, 애니메이션을 설정
    /// </summary>
    /// <param name="uid">샐성할 적 UID</param>
    /// <param name="getLevel">생성할 적 레벨</param>
    public void Init(string uid, int getLevel)
    {
        // 레벨과 UID 저장
        level = getLevel;
        enemyUID = uid;

        // UID로 적 데이터 조회
        EnemyData data = Managers.EnemyData.GetEnemyData(enemyUID);

        // 데이터가 없으면 잘못된 적이기에 제거
        if(data == null)
        {
            Destroy(gameObject);
            return;
        }

        // EnemyData 값 복사
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
        // 생성 후에는 살아있는 상태
        isDead = false;

        // 레벨을 반영한 스탯 정라기
        SetState();
        // 적 스킬 초기화
        skill.Init(this, enemySkillUID);
        // 적 애니메이션 초기화
        anim.SetAnim(uid);
    }

    /// <summary>
    /// 적 체력 회복
    /// 적의 보인 스킬이나 외부에서 주는 스킬에 의한 체력 회복
    /// 최대 체력을 넘지 않도록 제한
    /// </summary>
    /// <param name="value">회복 값</param>
    public void EnemeyHeal(int value)
    {
        // 회복이기에 0보다 작다면 중지
        if (value <= 0)
            return;

        // 현제 체력이 최대 체력이 되지 않도록 조치
        currentHP = Mathf.Min(MaxHP, currentHP + value);
    }

    /// <summary>
    /// 보호막 수치 증가
    /// 스킬로 인한 보호막 수치 변경
    /// 최대 보호막 수치를 넘지 않도록 제한
    /// </summary>
    /// <param name="value"></param>
    public void ShieldValueChange(int value)
    {
        // 증가값이 0보다 작다면 종료
        if (value <= 0)
            return;

        // 최대 보호막을 넘지 않도록 증가
        currentShield = Mathf.Min(MaxShield, currentShield + value);
    }

    /// <summary>
    /// 이동속도 보정값 증가
    /// 스킬로 인한 이동속도 증가가 있다면 지속시간 이후 감소가 필요함
    /// 양수는 속도증가, 음수는 속도 감소 또는 기존 증가 값을 제거로 사용
    /// </summary>
    /// <param name="value"></param>
    public void MoveSpeedChange(float value)
    {
        // value값이 0이면 나눗셈에 문제가 되기에 방어 필요
        if (value == 0f)
            return;

        // 배율을 비율처럼 사용하여 증가 속도로 계산
        float up = currentSpeed * (Mathf.Abs(value) / 100f);

        // value값이 음수면 감소 방향으로 변환
        if (value < 0)
            up *= -1;

        // 이동속도 보정값 누적
        increaseSpeed += up;
    }

    /// <summary>
    /// 레벨을 반영한 스탯 설정
    /// </summary>
    private void SetState()
    {
        // 기본값과 레벨별 증가량 체력과 보호막 계산
        maxHP = currentHP = basicHp + (increaseHP * level);
        maxShield = currentShield = basicShield + (increaseShield * level);
        // 이동속도는 오로지 스킬로만 변경
        currentSpeed = moveSpeed;
        // 이동속도 보정치 초기화
        increaseSpeed = 0.0f;
    }

    /// <summary>
    /// 적 사망 처리
    /// 이동 정지, 보상 이벤트 전달, 사망 애니메이션 실행 후, 오브젝터 제거
    /// </summary>
    private void Die()
    {
        // 이미 사망시 중복 보상 방지
        if (IsDead)
            return;

        // 사망 확인
        isDead = true;

        // EnemyMove에 사망 전달, StageManager에게 골드 이벤트 전달
        move.IsDead((int)RewardGold);

        // 사망 애니매이션 실행
        anim.Die();
        
        // 1초뒤 오브젝트 제거 함수 동작
        Invoke("Dead", 1f);
    }

    /// <summary>
    /// 적 오브젝트 제거 
    /// </summary>
    private void Dead()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 적 데미지 적용
    /// 보호막이 있으면 보호막 부터 감소, 보호막이 0이하가 되면 남은 데미지를 HP에 적용
    /// </summary>
    /// <param name="damage">받을 데미지</param>
    public void EnemyGeTakeDamage(int damage)
    {
        // 이미 죽었으면 피해 무시
        if (isDead)
            return;

        // 데미지가 0이하면 무기
        if (damage <= 0)
            return;

        // 보호막이 남아 있으면 보호막 부터 데미지 적용
        if(currentShield > 0)
        {
            currentShield -= damage;

            // 보호막이 남아 있으면 종료 
            if (currentShield > 0)
                return;

            // 보호막이 음수면 초과분을 HP에 반영
            currentHP += currentShield;

            // 이후에 보호막 회복 중 음수이면 회복이 안되기에, 보호막은 0으로 정리
            currentShield = 0;
        }
        else
        {
            // 보호막이 없다면 현제 체력에 직접 데미지 적용
            currentHP -= damage;
        }   

        // HP가 0 이하면 사망 처리
        if(currentHP <= 0)
        {
            Die();
            return;
        }
    }
}
