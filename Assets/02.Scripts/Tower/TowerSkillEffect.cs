using System;
using System.Collections.Generic;

/// <summary>
/// 타워 종류별 뵤유 개수에 따른 종족/타입 스킬 단계를 관리하는 클래스
/// TowerType별 현재 스킬 단계 저장
/// 필드 위 타워 개수 변화 감지
/// 조건에 맞는 TowerSkillData 조회
/// 스킬 단계가 변경 되었을 때, 이벤트 전달
/// </summary>
public class TowerSkillEffect
{
    // TowerType별 현재 적용 중인 스킬 단계
    private readonly Dictionary<TowerType, int> skillStep = new Dictionary<TowerType, int>();
    // 타워 타입별 스킬 단계가 변경되었을 때 호출
    // TowerType - 변경된 타워 타입, int - 변경된 스킬 단계, float - 해당 단계의 효과값
    public event Action<TowerType, int, float> OnChangedTowerSkillStep;

    /// <summary>
    /// 초기화
    /// 모든 TowerType의 스킬 단계를 0으로 초기화
    /// </summary>
    public void Init()
    {
        // 기존 단계 정보 제거
        skillStep.Clear();

        // enum에 등록된 모든 TowerType을 조회하여 값을초기화
        foreach (TowerType towerType in System.Enum.GetValues(typeof(TowerType)))
        {
            // 시작단게는 0, 0은 스킬 미적용 상태로 사용
            skillStep[towerType] = 0;
        }
    }

    /// <summary>
    /// 특정 TowerType의 필드 위 타워 개수가 변경 되엇을 때 호출
    /// 개수 조건에 맞는 TowerSkillData를 조회하고, 기존 단계와 달라졌다면 변경 이벤트를 발생
    /// </summary>
    /// <param name="type">개수가 변경된 타워 타입</param>
    /// <param name="cnt">해당 타입의 현재 필드 위 타워의 개수</param>
    public void ChangeTowerCount(TowerType type, int cnt)
    {
        // 현재 타워 개수에 해당하는 스킬 데이터 조회
        TowerSkillData skill = Managers.TowerSkill.GetTowerSkillDataByTypeAndCount(type, cnt);

        // 조건에 맞는 스킬이 엇으면 스킬 단계 0으로 조회
        if (skill == null)
        {
            skillStep[type] = 0;
            // 스킬 상태 알리기
            OnChangedTowerSkillStep?.Invoke(type, 0, 0);
            return;
        }

        // 기존 단계와 새 단계가 다를 때만 호출, 같은 단계 호출 중복 방지
        if (skillStep[type] != skill.step)
        {
            // 단계 갱신
            skillStep[type] = skill.step;
            // 변경된 스킬 상태 알리기
            OnChangedTowerSkillStep?.Invoke(type, skill.step, skill.effectValue);
        }
    }
}
