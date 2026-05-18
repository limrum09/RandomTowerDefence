using System;
using System.Collections.Generic;

[Serializable]
public class TowerUpgradeSaveData
{
    public TowerType type;
    public int grade;
    public int damageLevel;
    public int attackSpeedLevel;
}

/// <summary>
/// 타워 메타 강화 저장 데이터
/// SaveDataManager가 Json을 저장/로드할 때 사용
/// </summary>
[Serializable]
public class TowerMetaUpgradeData
{
    public List<TowerUpgradeSaveData> upgrades = new List<TowerUpgradeSaveData>();
}

/// <summary>
/// 타워 메타 강화를 관리
/// 타워 타입과 등급별로 공격력 강화 레벨, 공격속도 강화 레벨을 조회/증가
/// 실제 저장은 SaveDataManager가 잠당, 이 클래스는 저장 대상 데어티의 변경만 담당
/// </summary>
public class TowerMetaUpgradeManager
{
    private TowerMetaUpgradeData upgradeData = new TowerMetaUpgradeData();

    /// <summary>
    /// 특정 타워 타입과 등급에 해당하는 저장 데이터를 가져온다.
    /// 저장 데이터가 없다면 0레벨 데이터로 새로 생성
    /// </summary>
    /// <param name="getType">찾고 싶은 데이터 타입</param>
    /// <param name="getGrade">찾고 싶은 데이터 타입의 등급</param>
    /// <returns>찾은 데이터 또는 새로 만든 데이터</returns>
    private TowerUpgradeSaveData GetSaveData(TowerType getType, int getGrade)
    {
        TowerUpgradeSaveData data = upgradeData.upgrades.Find(x => x.type == getType && x.grade == getGrade);

        if(data == null)
        {
            data = new TowerUpgradeSaveData
            {
                type = getType,
                grade = getGrade,
                attackSpeedLevel = 0,
                damageLevel = 0
            };

            upgradeData.upgrades.Add(data);
        }

        return data;
    }

    /// <summary>
    /// 저장 데이터를 기반으로 타워 메타 강화 데이터를 초기화
    /// 저장 데이터가 없다면 새 데이터를 생성
    /// </summary>
    /// <param name="getUpgradeData">저장한 데이터</param>
    public void Init(TowerMetaUpgradeData getUpgradeData)
    {
        upgradeData = getUpgradeData != null ? getUpgradeData : new TowerMetaUpgradeData();
    }

    /// <summary>
    /// 특정 타워 타입과 등급의 공격력 강화 레벨을 반환
    /// </summary>
    /// <param name="getType">찾고 싶은 타워의 타입</param>
    /// <param name="getGrade">찾고 싶은 타워의 등급</param>
    /// <returns>타워의 공격력 레벨</returns>
    public int GetDamageLevel(TowerType getType, int getGrade)
    {
        var data = GetSaveData(getType, getGrade);

        return data.damageLevel;
    }

    /// <summary>
    /// 특정 타워 타입과 등급의 공격속도 강화 레벨을 반환
    /// </summary>
    /// <param name="getType">찾고 싶은 타워의 타입</param>
    /// <param name="getGrade">찾고 싶은 타워의 등급</param>
    /// <returns>타워의 공격속도 레벨</returns>
    public int GetAttackSpeedLevel(TowerType getType, int getGrade)
    {
        var data = GetSaveData(getType, getGrade);

        return data.attackSpeedLevel;
    }

    /// <summary>
    /// 특정 타워 타입과 등급의 공격력 강화 레벨을 증가
    /// </summary>
    /// <param name="getType">레벨을 올리고 싶은 타워의 타입</param>
    /// <param name="getGrade">레벨을 올리고 싶은 타워의 등급</param>
    /// <param name="upValue">올릴 레벨의 값</param>
    /// <returns></returns>
    public bool TowerDamageUpgrade(TowerType getType, int getGrade, int upValue)
    {
        var data = GetSaveData(getType, getGrade);
        data.damageLevel += upValue;
        
        return true;
    }

    /// <summary>
    /// 특정 타워 타입과 등급의 공격속도 강화 레벨 증가
    /// </summary>
    /// <param name="getType">레벨을 올리고 싶은 타워의 타입</param>
    /// <param name="getGrade">레벨을 올리고 싶은 타워의 등급</param>
    /// <param name="upValue">올릴 레벨의 값</param>
    /// <returns></returns>
    public bool TowerAttackSpeedUpgrade(TowerType getType, int getGrade, int upValue)
    {
        var data = GetSaveData(getType, getGrade);
        data.attackSpeedLevel += upValue;

        return true;
    }

    /// <summary>
    /// 저장용 타워 메타 강화 데이터를 반환
    /// SaveDataManager가 저장할 때 사용
    /// </summary>
    /// <returns>현제 메타 강화 데이터</returns>
    public TowerMetaUpgradeData GetTowerUpgradeSaveData()
    {
        return upgradeData;
    }
}
