using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 적 스킬 발동과 효과 적용을 담당하는 클래스
/// EnemySkillData를 기반으로 스킬 정보 초기화
/// 쿨타임에 따라 스킬 자동 동족
/// Self, Area, Tower 대상 구분 및 Heal, Haste, Shield 효과 적용
/// 스킬 이펙트 재생 요청
/// </summary>
public class EnemySkill : MonoBehaviour
{
    [SerializeField]
    private Enemy root;             // 스킬을 사용하는 적 본체
    [SerializeField]
    private LayerMask enemyLayer;   // Area 스킬 대상 탐색용 Enemy Layer
    [SerializeField]
    private LayerMask towerLayer;   // Tower 대상 스킬 탕색용 Tower Layer
    private EnemySkillData data;    // 현재 스킬 데이터

    private string skillUID;        // 현재 적이 사용하는 스킬 UID
    private bool isGetSkill;        // 스킬이 가져왔는지 확인
    private bool isSpeedSkill;      // 이동속도 스킬은 로직이 달라 중복 방지 확인 필요

    private float skillTimer;       // 스킬 쿨타임 계산용 타이머
    private float skillCoolTime;    // 스킬 쿨타임
    private float duration;         // 스킬 지속시간
    private float skillTick;        // 스킬 지속 틱 간격
    private float skillRange;       // Area, Tower 대상 탐색 범위
    private float skillValue;       // 최종 스킬 효과 수치

    private EnemySkillValueType valueType;  // 스킬 값 타입
    private EnemySkillType skillType;       // Heal, Haste, Shield 등 스킬 타입
    private EnemySkillTarget target;        // 스킬 적용되는 Self, Area, Tower 대상 타입

    /// <summary>
    /// 적 스킬 초기화
    /// 스킬 UID를 기준으로 EnemySkillData를 가져오고, 레벨에 따른 스킬 수치를 계산하여 적용
    /// </summary>
    /// <param name="enemy">스킬을 사용하는 적</param>
    /// <param name="getSkillUID">적이 사용할 스킬 UID</param>
    public  void Init(Enemy enemy,  string getSkillUID = null)
    {
        // 기본적으로 가진 상태로 시작
        isGetSkill = true;
        // Haste 중복 적용 방지 플래그 초기화
        isSpeedSkill = false;
        // 스킬 사용자 저장
        root = enemy;

        // 스킬이 없으면 스킬 로직 비활성화
        if (getSkillUID == "ES0000" || getSkillUID == null)
        {
            isGetSkill = false;
            return;
        }

        // UID 저장
        skillUID = getSkillUID;

        // 스킬 데이터 가져오기
        data = Managers.EnemySkillData.GetEnemySkillData(skillUID);

        // 가져온 data가 없으면 스킬이 없는 것이기에 스킬 로직 비활성화
        if(data == null)
        {
            isGetSkill = false;
            return;
        }

        // 기본 값 입력
        skillTimer = 0.0f;
        skillCoolTime = data.coolDown;
        duration = data.duration;
        skillRange = data.range;

        // 기본 스킬 수치 설정
        skillValue = data.basicValue;

        // 레벨에 따른 스케일링 적용
        if(root.Level >= data.scaleInterval && data.scaleInterval != 0)
        {
            // 적의 현제 레벨이 스케일 레벨을 넘기면 scaleMax 기준으로 적용
            if(data.scaleMax <= root.Level)
            {
                int value = data.scaleMax / data.scaleInterval;
                skillValue += (data.increaseValue * value);
            }
            // 최대 레벨보다 작다면 현제 레벨 기준으로 계산
            else
            {
                int value = root.Level / data.scaleInterval;
                skillValue += (data.increaseValue * value);
            }
        }

        // 스킬 정보 타입 저장
        valueType = data.valueType;
        skillType = data.type;
        skillTick = data.tickInterval;
        target = data.targetType;
    }

    /// <summary>
    /// 스킬 타입에 따른 스킬 효과 적용
    /// Self인 경우, 본인의 Script안에서 호출, Area인 경우 외부에서 호출
    /// </summary>
    /// <param name="getType">적용되는 스킬 타입</param>
    /// <param name="value">변동되는 스킬 값</param>
    /// <param name="getDuration">스킬 지속시간</param>
    /// <param name="getTick">지속시간 동안 동작될 틱 간격</param>
    public void ApplySkillEffect(EnemySkillType getType, float value, float getDuration, float getTick)
    {
        switch (getType)
        {
            case EnemySkillType.Heal:
                ApplyHeal((int)value, getDuration, getTick);
                break;
            case EnemySkillType.Haste:
                ApplySpeed(value, getDuration);
                break;
            case EnemySkillType.Shield:
                ApplyShield((int)value, getDuration, getTick);
                break;
            case EnemySkillType.Summon:
                // 추후 소환 로직 추가 예정
                break;
            case EnemySkillType.Stealth:
                // 추후 은신 로직 추가 예정
                break;
        }
    }

    private void Update()
    {
        // 받은 스킬이 없다면 종료
        if (!isGetSkill)
            return;

        // 스킬 타이머 증가
        skillTimer += Time.deltaTime;

        // 스킬 쿨타임을 체크하여 스킬 사용
        if(skillTimer > skillCoolTime)
        {
            UsingSkill();
            skillTimer = 0.0f;
        }
    }

    /// <summary>
    /// 스킬 사용 처리
    /// 스킬 이펙트를 재생한 뒤, 대상 타입에 따라 효과 적용 방식을 나눈다
    /// </summary>
    private void UsingSkill()
    {
        // 스킬 타입에 따른 이펙트 재생
        Managers.Effect.Play("Enemy" + skillType.ToString(), root.transform, PoolCategory.Stage, true);

        if (target == EnemySkillTarget.Self)
            ApplySkillEffect(skillType, skillValue, duration, skillTick);
        else if (target == EnemySkillTarget.Area)
            CheckEnemyAreaFindEnemy();
        else if (target == EnemySkillTarget.Tower)
            CheckEnemyAreaFindTower();
    }

    /// <summary>
    /// 스킬 적용 범위 안에 있는 Enemy Layer들을 찾아 스킬 적용
    /// </summary>
    private void CheckEnemyAreaFindEnemy()
    {
        // 현재 위치에서 skillRange만큼의 원형 범위안에서 enemyLayer들의 Colliser2D들을 전부 가져오기
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, skillRange, enemyLayer);

        foreach(Collider2D col in cols)
        {
            EnemySkill enemy = col.GetComponent<EnemySkill>();
            // 해당 Collider에서 EnemySkill 컴포넌트가 없다면 넘기기
            if (enemy == null)
                continue;

            // 스킬 적용
            enemy.ApplySkillEffect(skillType, skillValue, duration, skillTick);
        }
    }

    /// <summary>
    /// 스킬 적용 범위 안에 있는 Tower Layer의 정보를 가져와 디버프 스킬 적용
    /// 현재는 Debuff타입만 통과하도록 되어있음
    /// 실제 타워에 들어가는 로직은 추후 추가 필요
    /// </summary>
    private void CheckEnemyAreaFindTower()
    {
        // 디버프만 가능
        if (skillType != EnemySkillType.Debuff)
            return;

        // 현재 위치 기준 skillRange 안의 Tower Layer 탐색
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, skillRange, towerLayer);

        foreach(Collider2D col in cols)
        {
            // Tower 컴포넌트 가져오기
            Tower enemy = col.GetComponent<Tower>();
            if (enemy == null)
                continue;

            // 타워 적용 로직 추가
        }
    }

    /// <summary>
    /// 회복 효과 적용
    /// duration이 0이하면 죽시회복, duration이 있으면 tick간격마다 회복
    /// </summary>
    /// <param name="value">스킬 회복량</param>
    /// <param name="getDuration">스킬 지속시간</param>
    /// <param name="tick">스킬 반복 실행 시간</param>
    private void ApplyHeal(int value, float getDuration, float tick = 0.0f)
    {
        // 즉시 회복
        if (getDuration <= 0.0f)
        {
            root.EnemeyHeal(value);
            return;
        }
            
        // 지속 회복
        if (getDuration > 0.0f)
        {
            StartCoroutine(TickSkill(getDuration, tick, () =>
            {
                root.EnemeyHeal(value);
            }));
        }
    }

    /// <summary>
    /// 이동속도 효과 적용
    /// duration이 0 이하면 불가능, duration이 0 이상만 적용
    /// duration이 0 이하는 영구 적용이기에 불가
    /// </summary>
    /// <param name="speed">스킬 증가 속도</param>
    /// <param name="getDuration">지속시간</param>
    /// <param name="tick">스킬 재사용 간견</param>
    private void ApplySpeed (float speed, float getDuration, float tick = 0.0f)
    {
        // 적용 취소
        if (getDuration <= 0.0f)
            return;


        if (getDuration > 0.0f)
        {
            if (!isSpeedSkill)
                StartCoroutine(SpeedDuration(speed, getDuration));
        }
    }

    /// <summary>
    /// 보호막 회복 / 증가 효과 적용
    /// duration이 0 이하면 일회성 즉발 적용, 0이상이면 tick간격마다 반복 적용
    /// </summary>
    /// <param name="value">보호막 회복량</param>
    /// <param name="getDuration">지속시간</param>
    /// <param name="tick">스킬 반복 시간</param>
    private void ApplyShield(int value, float getDuration, float tick = 0.0f)
    {
        // 즉발
        if (getDuration <= 0.0f)
        {
            root.ShieldValueChange(value);
            return;
        }
            
        // 지속시간 동안 스킬 사용
        if (getDuration > 0.0f)
        {
            StartCoroutine(TickSkill(getDuration, tick, () =>
            {
                root.ShieldValueChange(value);
            }));
        }
    }
    
    /// <summary>
    /// duration동안 tickAction을 주기적으로 실행
    /// heal, shield 회복 지속 효과에 사용
    /// </summary>
    /// <param name="getDuration">지속 시간</param>
    /// <param name="getInterval">반복 시간</param>
    /// <param name="tickAction">반복마다 실행할 효과</param>
    /// <returns></returns>
    IEnumerator TickSkill(float getDuration, float getInterval, Action tickAction)
    {
        // tick간격이 없다면 한번만 진행, 무한 반복 막음
        if(getInterval <= 0.0f)
        {
            tickAction?.Invoke();
            yield break;

        }

        float timer = 0.0f;
        // 다음 스킬이 실행될 시간
        float skillInterval = getInterval;

        while(timer < getDuration)
        {
            timer += Time.deltaTime;

            // 틱 시간이 되었으면 효과 실행
            if(timer > skillInterval)
            {
                tickAction?.Invoke();
                // 다음 반복 시간 갱신
                skillInterval += timer;
            }

            yield return null;
        }
    }

    /// <summary>
    /// 속도 증가 효과 적용
    /// duration시간이 지나면 다시 속도 감소
    /// </summary>
    /// <param name="value"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    IEnumerator SpeedDuration(float value, float duration)
    {
        root.MoveSpeedChange(value);
        isSpeedSkill = true;
        yield return new WaitForSeconds(duration);

        isSpeedSkill = false;
        root.MoveSpeedChange(-value);
    }
}
