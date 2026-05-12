using System;
using System.Collections.Generic;
using UnityEngine;

public class StageWaveManager : MonoBehaviour
{
    private const string END_WAVE = "END";

    private string nextWaveUID;
    private WaveData currentWave;
    private List<WaveEnemyRosterData> currentWaveRosterData;

    public event Action<List<WaveEnemyRosterData>> onWaveRosterData;

    public void Init(string startWaveID)
    {
        currentWave = null;
        currentWaveRosterData = null;
        nextWaveUID = startWaveID;

        SetCurrentWaveData();
    }

    private bool SetCurrentWaveData()
    {
        if (nextWaveUID == "END")
            return false;

        currentWave = Managers.Wave.GetWaveData(nextWaveUID);

        if (currentWave == null)
            return false;

        currentWaveRosterData = Managers.WaveRoster.GetWaveRosterData(currentWave.waveUID);

        if (currentWave == null || currentWaveRosterData == null)
            return false;

        nextWaveUID = currentWave.nextWave;
        return true;
    }

    public void WaveStart()
    {

    }

    public void WaveEnd()
    {
        if(nextWaveUID == END_WAVE)
        {
            // 웨이브 모두 클리어, 스테이지 종료
        }

        if (SetCurrentWaveData())
        {
            onWaveRosterData?.Invoke(currentWaveRosterData);
        }
    }
}
