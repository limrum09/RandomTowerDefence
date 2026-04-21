using System;
using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RunSessionState
{
    private int stageID;

    private int currentWave;
    private int currentLife;
    private int gold;
    private int level;
    private int currentExp;
    private int totalKillCount;
    private int waveKillCount;
    private int freeRollCount;
    private int freeObstacleCount;

    private bool isWaveRunning;
    private bool isStageEnded;

    public int StageID => stageID;
    public int CurrentWave => currentWave;
    public int CurrentLife => currentLife;
    public int Gold => gold;
    public int Level => level;
    public int CurrentExp => currentExp;
    public int TotalKillCnt => totalKillCount;
    public int WaveKillCnt => waveKillCount;
    public int FreeRollCnt => freeRollCount;
    public int FreeObstacleCnt => freeObstacleCount;
    public bool IsWaveRunning => isWaveRunning;
    public bool IsStageEnded => isStageEnded;

    public RunSessionState(int id, int life, int startingGold, int rollCnt, int obstacleCnt)
    {
        stageID = id;
        currentLife = life;
        gold = startingGold;
        freeRollCount = rollCnt;
        freeObstacleCount = obstacleCnt;

        level = 1;
        currentWave = 1;
        currentExp = 0;
        totalKillCount = 0;
        waveKillCount = 0;
        isWaveRunning = false;
        isStageEnded = false;
    }

    public void SetGold(int value) => gold = Math.Max(0, value);
    public void SetExp(int value) => currentExp = Math.Max(0, value);
    public void SetLevel(int value) => level = Math.Max(1, value);
    public void SetLife(int value) => currentLife = Math.Max(0, value);
    public void SetWave(int value)=> currentWave = Math.Max(1, value);
    public void SetTotalSkillCount(int value) => totalKillCount = Math.Max(0, value);
    public void SetWaveSkillCount(int value) => waveKillCount = Math.Max(0, value);
    public void SetFreeRollCount(int value) => freeRollCount = Math.Max(0, value);
    public void SetFreeObstacleCount(int value) => freeObstacleCount = Math.Max(0, value);
    public void SetWaveRunning(bool value) => isWaveRunning = value;
    public void SetStageEnded(bool value) => isStageEnded = value;
}

public class RunSessionDataManager
{
    // 퀘스트브릿지 역할
    public event Action<int> OnGoldAmountChanged;
    public event Action<int, int> OnExpChanged;
    public event Action<int> OnLevelChanged;
    public event Action<int> OnLifeChanged;
    public event Action<int> OnWaveChanged;
    public event Action<int> OnObstacleCntChanged;
    public event Action<int> OnRollCntChanged;
    public event Action<int, int> OnKillCntChanged;

    private RunSessionState state;
    private SessionRule rule = new SessionRule();

    public RunSessionState SessionState => state;

    public void Init(int stageID, int life, int increaseGold = 0, int freeRoll = 0, int freeObstacle = 0)
    {
        state = new RunSessionState(stageID, life, 300 + increaseGold, freeRoll, 10 + freeObstacle);
    }

    public int GetNeedExp()
    {
        return rule.GetNeedEXP(state.Level);
    }

    public void AddExp(int amount)
    {
        if (state == null || amount <= 0)
            return;

        state.SetExp(state.CurrentExp + amount);

        while(state.CurrentExp >= rule.GetNeedEXP(state.Level))
        {
            int needExp = rule.GetNeedEXP(state.Level);
            
            state.SetExp(state.CurrentExp - needExp);
            state.SetLevel(state.Level + 1);

            OnLevelChanged?.Invoke(state.Level);
        }

        OnExpChanged?.Invoke(state.CurrentExp, rule.GetNeedEXP(state.Level));
    }

    public void AddGold(int amount)
    {
        if (state == null || amount <= 0)
            return;

        state.SetGold(state.Gold + amount);
        OnGoldAmountChanged?.Invoke(state.Gold);
    }

    public bool UsingGold(int amount)
    {
        if(state == null || amount <= 0)
            return false;

        if (state.Gold < amount)
            return false;

        state.SetGold(state.Gold - amount);
        OnGoldAmountChanged?.Invoke(state.Gold);

        return true;
    }

    public void ChangeLife(int value)
    {
        if (state == null || value == 0)
            return;

        state.SetLife(state.CurrentLife + value);

        OnLifeChanged?.Invoke(state.CurrentLife);
    }

    public void AddkillCount(int amount = 1)
    {
        if (state == null || amount <= 0)
            return;

        state.SetTotalSkillCount(state.TotalKillCnt + amount);
        state.SetWaveSkillCount(state.WaveKillCnt + amount);

        OnKillCntChanged?.Invoke(state.TotalKillCnt, state.WaveKillCnt);
    }

    public void SetWave(int wave)
    {
        if (state == null)
            return;

        state.SetWave(wave);
        state.SetWaveSkillCount(0);

        OnWaveChanged?.Invoke(wave);
    }

    public void SetWaveRunning(bool isRun)
    {
        if (state == null)
            return;

        state.SetWaveRunning(isRun);
    }

    public void EndStage()
    {
        if(state == null)
            return;

        state.SetStageEnded(true);
        state.SetWaveRunning(false);
    }

    public bool UsingFreeRoll()
    {
        if(state == null || state.FreeRollCnt <= 0)
            return false;

        state.SetFreeRollCount(state.FreeRollCnt - 1);
        OnRollCntChanged?.Invoke(state.FreeRollCnt);

        return true;
    }

    public bool UsingFreeObstable()
    {
        if (state.FreeObstacleCnt <= 0 || state == null)
            return false;

        state.SetFreeObstacleCount(state.FreeObstacleCnt - 1);
        OnObstacleCntChanged?.Invoke(state.FreeObstacleCnt);

        return true;
    }

    public int GetFreeObstacle(int amount)
    {
        if(state == null || amount <= 0)
            return -1;

        if (amount > 0)
        {
            state.SetFreeObstacleCount(state.FreeObstacleCnt + amount);
            OnObstacleCntChanged?.Invoke(state.FreeObstacleCnt);
        }

        return state.FreeObstacleCnt;
    }
}
