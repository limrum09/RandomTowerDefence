using System.Collections.Generic;
using UnityEditor.Build.Pipeline.Injector;
using UnityEngine;



public class StageResultData
{
    public string stageLevel;
    public int clearWave;
    public int currentLife;

    public int stageLevelBonus;
    public int lifeBonus;
    public int waveScore;
    public int finalScore;

    public int towerSellGold;
    public int itemSellGold;
    public int remainGold;

    public List<TowerResultData> towers = new List<TowerResultData>();
    public List<ItemResultData> items = new List<ItemResultData>();
}

public class TowerResultData
{
    public Sprite icon;
    public TowerType type;
    public int count;
    public int sellValueTotal;
}

public class ItemResultData
{
    public Sprite icon;
    public int sellValue;
}

/// <summary>
/// 스테이지 결과 화면에서 최종 점수를 계산
/// </summary>
public class StageScoreCalculator
{
    /// <summary>
    /// 최종 점수 게산
    /// ((난이도 보너스 * 생명력 보너스) * 웨이브 점수) + 남은 재화 가치
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public StageResultData ScoreCalculator(StageResultData getData)
    {
        StageResultData data = getData;

        int totalGold = data.remainGold;
        int towerGold = 0;
        int itemGold = 0;

        // 아이템 가치 합산
        foreach(var item in data.items)
        {
            if (item == null)
                continue;

            itemGold += item.sellValue;
        }

        // 타워 가치 합산
        foreach(var tower in data.towers)
        {
            towerGold += tower.sellValueTotal;
        }

        data.towerSellGold = towerGold;
        data.itemSellGold = itemGold;
        totalGold = totalGold + towerGold + itemGold;
        data.stageLevelBonus = GetStageLevelBonus(data.stageLevel);
        data.lifeBonus = GetLifeBonus(data.currentLife, data.stageLevelBonus);
        data.waveScore = GetWaveScore(data.clearWave);

        data.finalScore = (Mathf.Max(1, (data.stageLevelBonus * data.lifeBonus)) * data.waveScore) + totalGold;

        return data;
    }

    /// <summary>
    /// 난이도별 배율
    /// Easy : 1, Normal : 2, Hard : 3, Hell : 4
    /// </summary>
    /// <param name="getStageLevel"></param>
    /// <returns></returns>
    private int GetStageLevelBonus(string getStageLevel)
    {
        switch (getStageLevel)
        {
            case "EASY":
                return 1;
            case "NORMAL":
                return 2;
            case "HARD":
                return 3;
            case "HELL":
                return 4;
            default:
                return 0;
        }
    }

    /// <summary>
    /// 남은 생명력에 따른 보너스 계산
    /// 생명력을 보유할 수 있는 상한선은 없지만, 보너스를 받을 수 있는 상한선이 있음
    /// 난이도마다 생명력을 4구역으로 나누어 1 - 5점의 보너스를 부여
    /// 단, 스테이지 클리어시에만 획득 가능하다
    /// </summary>
    /// <param name="currentLife"></param>
    /// <param name="stageBonus"></param>
    /// <returns></returns>
    private int GetLifeBonus(int currentLife, int stageBonus)
    {
        if (currentLife <= 0)
            return 0;

        if (stageBonus <= 0)
            return 0;

        int step = stageBonus - 1;

        int maxLifeCnt = 20 - (step * 4);
        int lifeStep = maxLifeCnt / 4;

        int lifeCnt = Mathf.Min(maxLifeCnt, currentLife);
        int minus = maxLifeCnt - lifeCnt;

        if(minus == 0)
            return 5;

        int value = minus / lifeStep;
        
        return Mathf.Max(1, 4 - value);
    }

    /// <summary>
    /// 웨이브 점수를 계산
    /// 각 웨이브마다 (웨이브 * 10)점을 누적
    /// </summary>
    /// <param name="currentWave"></param>
    /// <returns></returns>
    private int GetWaveScore(int currentWave)
    {
        int totalScore = 0;

        for(int i = 1; i <= currentWave; i++)
        {
            totalScore += (i * 10);
        }

        return totalScore;
    }
}
