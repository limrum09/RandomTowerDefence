using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum SpeedModityType
{
    SkillSpeed,
    TowerAttackSpeed
}

/// <summary>
/// 적의 기본 데이터를 관리하는 클래스
/// EnemyData를 기반으로 적 정보 초기화
/// HP, Shield, MoveSpeed 계산
/// 회복, 보호막, 이동속도 변화처리
/// 데이지, 사망 처리
/// </summary>
public class Enemy : MonoBehaviour
{
    [Header("Compoenents")]
    [SerializeField]
    private EnemyMove move;     // 적 이동 담당 컴포넌트
    [SerializeField]
    private EnemyAnim anim;     // 적 애니메이션 담당 컴포넌트
    [SerializeField]
    private EnemySkill skill;   // 적 스킬 담당 컴포넌트

    [Header("Info")]
    [SerializeField]
    private string enemyUID;    // 적 UID
    [SerializeField]
    private int level;          // 적 레벨
    [SerializeField]
    private string stringKey;   // 적 이름 로컬라이징
    [SerializeField]
    private string enemySkillUID;// 적 스킬 UID
    [SerializeField]
    private string iconPath;    // 아이콘 경로

    [Header("HP Bar")]
    [SerializeField]
    private Image currentHPBar;
    [SerializeField]
    private Image currentShieldBar;

    [Header("Runitme Stat")]
    [SerializeField]
    private float maxHP;          // 최대 체력
    [SerializeField]
    private float currentHP;      // 현재 체력
    [SerializeField]
    private float maxShield;      // 최대 보호막
    [SerializeField]
    private float currentShield;  // 현재 보호막
    [SerializeField]
    private float currentSpeed; // 현재 이동 속도

    private bool isStealth;     // 은신상태 체크
    private bool isDead;        // 죽었는지 판단
    private float rewardGold;   // 처치시 지급 골드
    private Tween hpTween;
    private Tween shieldTween;

    private readonly Dictionary<SpeedModityType, float> speedModify = new Dictionary<SpeedModityType, float>();

    public UnityEvent onDead;

    public string EnemyUID => enemyUID;
    public int Level => level;
    public string StringKey => stringKey;
    public string EnemySkillUID => enemySkillUID;
    public float MaxHP => maxHP;
    public float MaxShield => maxShield;
    public float RewardGold => rewardGold;
    public bool IsTargetable => !isDead && !isStealth;
    public bool IsDead => isDead;
    /// <summary>
    /// 실제 이동속도
    /// 시본 속도 + 스킬 / 버프 보정값
    /// </summary>
    public float MoveSpeed
    {
        get
        {
            float totalSpeedModify = 0.0f;

            foreach(float speed in speedModify.Values)
                totalSpeedModify += speed;

            return currentSpeed + totalSpeedModify;
        }
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

        // 사망 시, UnityEvent동작
        onDead?.Invoke();

        // 1초뒤 오브젝트 제거 함수 동작
        Invoke("Dead", 1f);
    }

    /// <summary>
    /// 적 오브젝트 제거 
    /// </summary>
    private void Dead()
    {
        hpTween?.Kill();
        shieldTween?.Kill();
        Destroy(this.gameObject);
    }

    /// <summary>
    /// HPBar, Shieldbar 새로고침
    /// </summary>
    private void SetHP()
    {
        hpTween = RefreshBar(hpTween, currentHPBar, currentHP, MaxHP);
    }

    private void SetShield()
    {
        shieldTween = RefreshBar(shieldTween, currentShieldBar, currentShield, MaxShield);
    }

    private Tween RefreshBar(Tween tween, Image bar, float current, float max)
    {
        if (bar == null)
            return null;

        float targetFill = current <= 0f ? 0f : Mathf.Clamp01(current / max);

        tween?.Kill();

        return bar.DOFillAmount(targetFill, 0.2f).SetEase(Ease.OutQuad);
    }

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

        EnemyResolveInfo info = EnemyInfoCal.Create(enemyUID, level);

        if(info == null)
        {
            Destroy(gameObject);
            Debug.Log("Enemy Resolve Info가 없음");
            return;
        }
        
        // EnemyData 값 복사
        stringKey = info.stringKey;
        enemySkillUID = info.skillUID;

        maxHP = currentHP = info.maxHP;
        maxShield = currentShield = info.maxShield;
        currentSpeed = info.moveSpeed;

        rewardGold = info.rewardGold;
        iconPath = info.itemPath;
        // 생성 후에는 살아있는 상태
        isDead = false;

        foreach (SpeedModityType type in Enum.GetValues(typeof(SpeedModityType))){
            speedModify[type] = 0;
        }

        // 적 스킬 초기화
        skill.Init(this, enemySkillUID);
        // 적 애니메이션 초기화
        anim.SetAnim(uid);
    }

    public void SetMove(bool pause)
    {
        move.SetMove(pause);
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

        if (currentHP <= 0)
            return;

        // 현제 체력이 최대 체력이 되지 않도록 조치
        currentHP = Mathf.Min(MaxHP, currentHP + value);
        SetHP();
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

        if (currentHP <= 0)
            return;

        // 최대 보호막을 넘지 않도록 증가
        currentShield = Mathf.Min(MaxShield, currentShield + value);
        SetShield();
    }

    /// <summary>
    /// 이동속도 보정 값 증가
    /// 스킬로 인한 이동속도 변경
    /// 양수는 속도증가, 음수는 속도 감소
    /// </summary>
    /// <param name="type"></param>
    /// <param name="perValue"></param>
    public void SetMoveSpeedModify(SpeedModityType type, float perValue)
    {
        float modify = currentSpeed * (Mathf.Abs(perValue) / 100f);

        if (perValue < 0)
            modify *= -1f;

        speedModify[type] = modify;
    }

    /// <summary>
    /// 이동 속도 증가 지속시간 이후, 이동속도 감소
    /// 해당 타입의 값을 0으로 만듬
    /// </summary>
    /// <param name="type"></param>
    public void RemoveMoveSpeedModify(SpeedModityType type)
    {
        speedModify[type] = 0;
    }

    /// <summary>
    /// 적 데미지 적용
    /// 보호막이 있으면 보호막 부터 감소, 보호막이 0이하가 되면 남은 데미지를 HP에 적용
    /// </summary>
    /// <param name="damage">받을 데미지</param>
    public void EnemyGeTakeDamage(float damage)
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
            SetShield();

            // 보호막이 남아 있으면 종료 
            if (currentShield > 0)
            {
                return;
            }

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

        SetHP();

        // HP가 0 이하면 사망 처리
        if (currentHP <= 0)
        {
            Die();
            return;
        }
    }

    public void SetStealth(bool val)
    {
        Color color = Color.white;
        color.a = val ? 0.3f : 1f;
        GetComponentInChildren<SpriteRenderer>().color = color;
        isStealth = val;
    }
}
