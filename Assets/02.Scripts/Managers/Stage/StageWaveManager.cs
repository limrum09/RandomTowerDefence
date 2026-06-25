using System;
using System.Collections.Generic;
using UnityEngine;

public enum WavePrepareState
{
    Success,
    End,
    Failed
}

public struct WavePrepareResult
{
    public WavePrepareState state;
    public string message;
    public WaveData waveData;
    public List<WaveEnemyRosterData> rosterData;

    public bool IsSuccess => state == WavePrepareState.Success;
    public bool IsEnd => state == WavePrepareState.End;
    public bool IsFailed => state == WavePrepareState.Failed;
}

public class StageWaveManager : MonoBehaviour
{
    private const string END_WAVE = "END";

    private string nextWaveUID;
    private WaveData currentWave;
    private List<WaveEnemyRosterData> currentWaveRosterData;

    public void Init(string startWaveID)
    {
        currentWave = null;
        currentWaveRosterData = null;
        nextWaveUID = startWaveID;
    }

    /// <summary>
    /// 다음 웨이브의 정보를 넘겨줌
    /// 다음 웨이브 데이터를 가져오지 못했다면 Failed을
    /// 다음 웨이브 데이터를 가져왔다면 Success를
    /// 웨이브를 완료했자면 End를 반환한다.
    /// </summary>
    /// <returns></returns>
    public WavePrepareResult TryPrepareNextWave()
    {
        if (string.IsNullOrEmpty(nextWaveUID))
        {
            return new WavePrepareResult
            {
                state = WavePrepareState.Failed,
                message = "Next Wave UID is empty"
            };
        }

        if(nextWaveUID == END_WAVE)
        {
            return new WavePrepareResult
            {
                state = WavePrepareState.End,
                message = "End wave"
            };
        }

        WaveData wave = Managers.Wave.GetWaveData(nextWaveUID);

        if(wave == null)
        {
            return new WavePrepareResult
            {
                state = WavePrepareState.Failed,
                message = $"Wave data missing : {nextWaveUID}"
            };
        }

        List<WaveEnemyRosterData> roster = Managers.WaveRoster.GetWaveRosterData(wave.waveUID);

        if(roster == null || roster.Count <= 0)
        {
            return new WavePrepareResult
            {
                state = WavePrepareState.Failed,
                message = $"Wave roster Data missing : {wave.waveUID}"
            };
        }

        if (string.IsNullOrEmpty(wave.nextWave))
        {
            return new WavePrepareResult
            {
                state = WavePrepareState.Failed,
                message = $"Next wave UID is empty : {wave.waveUID}"
            };
        }

        currentWave = wave;
        currentWaveRosterData = roster;
        nextWaveUID = wave.nextWave;

        return new WavePrepareResult
        {
            state = WavePrepareState.Success,
            message = string.Empty,
            waveData = currentWave,
            rosterData = currentWaveRosterData
        };
    }
}
