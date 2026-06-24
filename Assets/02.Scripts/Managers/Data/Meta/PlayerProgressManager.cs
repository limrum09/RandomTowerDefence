using Firebase.Firestore;
using System;

[Serializable]
[FirestoreData]
public class PlayerProgressData : IValidSaveData
{
    [FirestoreProperty] public int level { get; set; }
    [FirestoreProperty] public int exp { get; set; }
    [FirestoreProperty] public int metaCurrency { get; set; }

    public bool IsValid()
    {
        bool islevel = level >= 0;
        bool isexp = exp >= 0;
        bool ismetaCyurrency = metaCurrency >= 0;

        return islevel && isexp && ismetaCyurrency;
    }
}

/// <summary>
/// 유저의 영구 진행 데이터를 관리
/// 플레이어 레벨, 경험치, 메타 재화 같은 씬이 변경되어도 유지되아야 하는 값들을 담당한다
/// 실제 파일 저장은 SaveDataManger가 담당한다.
/// 해당 클래스는 저장 대상 데이터의조회와 변경만 담당한다.
/// </summary>
public class PlayerProgressManager
{
    private PlayerProgressData playerData = new PlayerProgressData();
    private const int MaxLevel = 12;

    /// <summary>
    /// 임시로 재화 제공
    /// </summary>
    private void TempAddCurrency()
    {
#if UNITY_EDITOR
        AddCurrency(50000);
#endif
    }

    /// <summary>
    /// 경험치를 추가한다.
    /// </summary>
    /// <param name="getExp"></param>
    public bool AddExp(int getExp)
    {
        if (getExp <= 0)
            return false;

        if(playerData.level >= MaxLevel)
        {
            playerData.level = MaxLevel;
            playerData.exp = 0;
            return false;
        }

        playerData.exp += getExp;

        bool isLevelUp = false;

        while(playerData.level < MaxLevel)
        {
            int needExp = Managers.ResearchLevel.GetNeedExp(playerData.level);

            if (needExp <= 0)
            {
                playerData.exp = 0;
                return isLevelUp;
            }

            if (playerData.exp < needExp)
                return isLevelUp;

            playerData.exp -= needExp;
            playerData.level++;
            isLevelUp = true;
        }

        playerData.level = MaxLevel;
        playerData.exp = 0;        

        return isLevelUp;
    }

    /// <summary>
    /// 메타 재화를 추가한다.
    /// 0이하의 값은 무시
    /// </summary>
    /// <param name="getCurrency"></param>
    public void AddCurrency(int getCurrency)
    {
        if (getCurrency <= 0)
            return;

        playerData.metaCurrency += getCurrency;
    }

    /// <summary>
    /// 메타 재화를 사용
    /// 보유 재화가 사용할 재화보다 부족하다면 false를 반환
    /// </summary>
    /// <param name="value">사용하는 재화 량</param>
    /// <returns>가지고 있는 재화가 충분하다면 true, 부족하면 false</returns>
    public bool UseCurrency(int value)
    {
        if (value <= 0)
        {
            UnityEngine.Debug.Log("소모 값이 0이하 : " + value);
            return false;
        }
            

        if (value > playerData.metaCurrency)
        {
            TempAddCurrency();
            return false;
        }

        playerData.metaCurrency -= value;

        Managers.Save.MarkPlayerDirty();

        return true;
    }

    public int GetCurrentEXP() => playerData.exp;

    public int GetCurreny() => playerData.metaCurrency;

    /// <summary>
    /// 현재 유저의 연구 레벨 가져오기
    /// </summary>
    /// <returns>현제 유저의 레벨</returns>
    public int GetPlayerLevel() => playerData.level;

    /// <summary>
    /// 저장 데이터를 기반으로 유저 진행 데이터를 초기화
    /// 저장 데이터가 없다면 기본 값으로 새 데이터를 생성
    /// </summary>
    /// <param name="getData"></param>
    public void LoadSaveData(PlayerProgressData getData)
    {
        playerData = getData != null ? getData : new PlayerProgressData() { level = 1, exp = 0, metaCurrency = 0 };

        if (playerData.level <= 0)
            playerData.level = 1;

        if(playerData.level >= MaxLevel)
        {
            playerData.level = MaxLevel;
            playerData.exp = 0;
        }
    }

    /// <summary>
    /// 데이터를 저장하기 위해 SaveDataManager에서 호출 시, 해당 데이터들을 넘겨준다.
    /// </summary>
    /// <returns>현제 유저의 데이터</returns>
    public PlayerProgressData GetSaveData()
    {
        return playerData;
    }
}
