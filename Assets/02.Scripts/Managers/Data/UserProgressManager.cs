using UnityEngine;

public class UserStageBonusData
{
    public int increaseGold { get; private set; }
    public int freeStoreRollCnt { get; private set; }
    public int freeObstacleCnt { get; private set; }
    public int terrainRollCnt { get; private set; }

    public UserStageBonusData()
    {
        increaseGold = 0;
        freeStoreRollCnt = 0;
        freeObstacleCnt = 0;
        terrainRollCnt = 0;
    }

    public void UpgradeIncreaseGold(int cnt)
    {
        increaseGold += cnt;
    }

    public void UpgradeFreeStoreRollCnt(int cnt)
    {
        freeStoreRollCnt += cnt;
    }

    public void UpgradeFreeObstacleCnt(int cnt)
    {
        freeObstacleCnt += cnt;
    }

    public void UpgradeTerrainRollCnt(int cnt)
    {
        terrainRollCnt += cnt;
    }
}

public class MetaTowerUpgradeData
{

}

/// <summary>
/// 유저가 강화한 메타 데이터를 저장하는 클래스
/// 초기 자금 증가, 기본 목숨 증가, 무료 장애물 개수 증가
/// 상점 무료 리롤 개수 증가, 지형 리롤권 개수 증가
/// 각 타워의 영구 강화 등 메타 재화로 하는 업그레이드를 저장
/// </summary>
public class UserProgressManager
{
    public UserStageBonusData stageBonus { get; private set; }
    public MetaTowerUpgradeData metaTower {  get; private set; }

    public void Init()
    {
        stageBonus = new UserStageBonusData();
        metaTower = new MetaTowerUpgradeData();

        // 나중에 저장된 값을 불러와서 채우기
    }

    public void TempSetDataToStageManager(int gold, int store, int obstacle, int terrain)
    {
        stageBonus.UpgradeIncreaseGold(gold);
        stageBonus.UpgradeFreeStoreRollCnt(store);
        stageBonus.UpgradeFreeObstacleCnt(obstacle);
        stageBonus.UpgradeTerrainRollCnt(terrain);
    }
}
