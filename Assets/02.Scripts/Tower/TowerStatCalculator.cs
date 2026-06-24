/// <summary>
/// 타워의 현재 표시용 스탯 정보
/// 공격력, 공격속도, 사거리를 담음
/// </summary>
public struct TowerStatPreview
{
    public float damage;
    public float attackSpeed;
    public float range;
    public float criticalPer;
    public float enemySlow;
}

/// <summary>
/// 타워 스탯 계산 전용 클래스
/// RunStatUpgradeManager를 주입받아 현재 런의 겅화 상태를 기준으로 타워의 공격력과 공격속도의 강화 값을 계산
/// </summary>
public static class TowerStatCalculator
{
    private static RunStatUpgradeManager statUpgrade;

    /// <summary>
    /// RunStatUpgradeManager를 주입받음
    /// </summary>
    /// <param name="getStatUpgrade"></param>
    public static void Init(RunStatUpgradeManager getStatUpgrade) => statUpgrade = getStatUpgrade;

    /// <summary>
    /// 강화 매니저 참조를 제거
    /// 스테이지 종료 또는 씬 종료 시 호출
    /// </summary>
    public static void Clear() => statUpgrade = null;

    /// <summary>
    /// 특정 타입의 타워의 총 공격력 강화 단계를 반환
    /// 기본, 아이템, 스킬 강화를 모두 합산
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetAttackStep(TowerType type)
    {
        return statUpgrade.GetAtkDamageStep(type) +
            statUpgrade.GetItemAtkDamageStep(type) +
            statUpgrade.GetSkillAtkDamageStep(type);
    }

    /// <summary>
    /// 특정 타입 타워의 총 공격솓도 강화 단계를 반환
    /// 기본, 아이템, 슼리 강화를 모두 합산
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int GetSpeedStep(TowerType type)
    {
        return statUpgrade.GetAtkSpeedStep(type) +
            statUpgrade.GetItemAtkSpeedStep(type) +
            statUpgrade.GetSkillAtkSpeedStep(type);
    }

    /// <summary>
    /// TowerData와 현재 런 강화 상태를 기준으로 타워의 공격력, 공격속도를 계산
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static TowerStatPreview Calculate(TowerData data)
    {
        if (data == null || statUpgrade == null)
            return default;

        float baseAtk = Managers.Game.GetTowerDisplayData(data).currentValue2;
        float baseAtkSpeed = Managers.Game.GetTowerDisplayData(data).currentValue1;

        TowerSessionUpgradeData damageUpgrade = Managers.SessionTowerUpgrade.GetUpgradeStepData(data.towerUID, UpgradeType.Damge);
        TowerSessionUpgradeData speedUpgrade = Managers.SessionTowerUpgrade.GetUpgradeStepData(data.towerUID, UpgradeType.Speed);

        if (damageUpgrade == null || speedUpgrade == null)
            return default;

        float increaseAtkDamage = damageUpgrade.increaseValue;
        float increaseAtkSpeed = speedUpgrade.increaseValue;

        float calDamage = baseAtk + (int)(increaseAtkDamage * GetAttackStep(data.towerType));
        float calSpeed = baseAtkSpeed + (increaseAtkSpeed * GetSpeedStep(data.towerType));

        return new TowerStatPreview
        {
            damage = calDamage,
            attackSpeed = calSpeed,
            range = data.range,
            criticalPer = statUpgrade.GetCriticalPer(data.towerType),
            enemySlow = statUpgrade.GetEnemySlowPer(data.towerType)
        };
    }
}
