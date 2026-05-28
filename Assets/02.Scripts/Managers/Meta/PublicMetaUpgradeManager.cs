using Firebase.Firestore;
using System;
using System.Collections.Generic;

[Serializable]
[FirestoreData]
public class PublicMetaSaveData
{
    [FirestoreProperty] public int type { get; set; }
    [FirestoreProperty] public int level { get; set; }
}

/// <summary>
/// 저장에 사용되는 데이터
/// </summary>
[Serializable]
[FirestoreData]
public class PublicMetaUpgradeData
{
    [FirestoreProperty] public List<PublicMetaSaveData> upgrades { get; set; } = new List<PublicMetaSaveData>();
}

/// <summary>
/// 공용 메타 강화를 관리
/// 시작 골드, 드랍 골드, 무료 장애물, 지형 재설정 같은 타워에 직접 종속되지 않는 영구 강화 데이터를 담당한다.
/// </summary>
public class PublicMetaUpgradeManager
{
    private PublicMetaUpgradeData upgradeData = new PublicMetaUpgradeData();

    /// <summary>
    /// 특정 공용 강화 타입의 저장 데이터를 가져온다.
    /// 저장 데이터가 없으면 0레벨 데이터를 새로 생성한다.
    /// </summary>
    /// <param name="getType">가져올 데이터의 타입</param>
    /// <returns>찾거나 새로 만든 데이터</returns>
    private PublicMetaSaveData GetMetaSaveData(MetaUpgradeType getType)
    {
        PublicMetaSaveData data = upgradeData.upgrades.Find(x => (MetaUpgradeType)x.type == getType);

        if(data == null)
        {
            data = new PublicMetaSaveData
            {
                type = (int)getType,
                level = 0
            };

            upgradeData.upgrades.Add(data);
        }

        return data;
    }

    /// <summary>
    /// 특정 공용 메타 강화의 레벨을 증가시킨다.
    /// </summary>
    /// <param name="getType">증가시킬 데이터의 타입</param>
    /// <param name="upValue">증가 시킬 값</param>
    /// <returns></returns>
    public bool PublicMetaUpgrade(MetaUpgradeType getType, int upValue)
    {
        PublicMetaSaveData data = GetMetaSaveData(getType);
        data.level += upValue;

        return true;
    }

    /// <summary>
    /// 문자열을 MetaUpgradeType으로 변환
    /// Json, UI, UI 문자열 등을 enum으로 변환할 때 사용한다.
    /// </summary>
    /// <param name="value">변환하고 싶은 문자열</param>
    /// <param name="type">내보낼 Enum</param>
    /// <returns></returns>
    public bool GetPublicMetaType(string value, out MetaUpgradeType type)
    {
        return System.Enum.TryParse(value, true, out type);
    }

    /// <summary>
    /// 특정 공용 메타 강화의 현재 레벨을 반환
    /// </summary>
    /// <param name="getType">반환하고 싶은 타입</param>
    /// <returns>현재 레벨</returns>
    public int GetPublicMetaDataLevel(MetaUpgradeType getType)
    {
        return GetMetaSaveData(getType).level;
    }

    /// <summary>
    /// UI에 표시할 공용 강화 이름을 반환
    /// </summary>
    /// <param name="getType"></param>
    /// <returns></returns>
    public string GetTypeName(MetaUpgradeType getType)
    {
        switch (getType)
        {
            case MetaUpgradeType.StartingGold:
                return "시작 시 골드 획득량";
            case MetaUpgradeType.FreeObstacle:
                return "무료 장애물 설치";
            case MetaUpgradeType.FreeTerrainReroll:
                return "지형 재설정";
            case MetaUpgradeType.DropGold:
                return "몬스터 드랍 골드";
            default:
                return "찾을 수 없음";
        }
    }

    /// <summary>
    /// UI에 표시할 공용 강화 설명을 반환
    /// </summary>
    /// <param name="getType"></param>
    /// <returns></returns>
    public string GetTypeInfoStr(MetaUpgradeType getType)
    {
        switch (getType)
        {
            case MetaUpgradeType.StartingGold:
                return "스테이지 시작 시 획득하는 골드 증가";
            case MetaUpgradeType.FreeObstacle:
                return "무료 장애물 설치권 개수 증가";
            case MetaUpgradeType.FreeTerrainReroll:
                return "지형 재설정 횟수 증가";
            case MetaUpgradeType.DropGold:
                return "몬스터가 드랍하는 골드의 개수 증가";
            default:
                return "찾을 수 없음";
        }
    }

    /// <summary>
    /// UI에서 수치 할목 이름으로 표시할 문자열 반환
    /// </summary>
    /// <param name="getType"></param>
    /// <returns></returns>
    public string GetTypeCountStr(MetaUpgradeType getType)
    {
        switch (getType)
        {
            case MetaUpgradeType.StartingGold:
                return "획득 골드";
            case MetaUpgradeType.FreeObstacle:
                return "무료 개수";
            case MetaUpgradeType.FreeTerrainReroll:
                return "재설정횟수";
            case MetaUpgradeType.DropGold:
                return "드랍 골드";
            default:
                return "찾을 수 없음";
        }
    }

    /// <summary>
    /// 저장 데이터를 기반으로 공용 메타 강화 데이터를 초기화
    /// 저장 데이터가 없다면 새 데이터를 생성
    /// </summary>
    /// <param name="getUpgradeData"></param>
    public void LoadSaveData(PublicMetaUpgradeData getUpgradeData)
    {
        upgradeData = getUpgradeData != null ? getUpgradeData : new PublicMetaUpgradeData();
    }

    /// <summary>
    /// 저장용 공용 메타 강화 데이터를 반환
    /// SaveDataManager가 저장할 때 사용
    /// </summary>
    /// <returns></returns>
    public PublicMetaUpgradeData GetPublicMetaSaveData()
    {
        return upgradeData;
    }
}
