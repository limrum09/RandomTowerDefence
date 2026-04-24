using System;
using System.Collections.Generic;
using UnityEngine;

public class StageWaveManager : MonoBehaviour
{
    private string nextWaveUID;
    private WaveData currentWave;
    private List<WaveEnemyRosterData> currentWaveRosterData;

    public event Action<List<WaveEnemyRosterData>> onWaveRosterData;

    private void Awake()
    {
        currentWave = null;
        nextWaveUID = "W001";
    }
    void Start()
    {
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
        if (SetCurrentWaveData())
        {
            onWaveRosterData?.Invoke(currentWaveRosterData);
        }

        if(nextWaveUID == "END")
        {
            // 웨이브 모두 클리어, 스테이지 종료
        }
    }
}
