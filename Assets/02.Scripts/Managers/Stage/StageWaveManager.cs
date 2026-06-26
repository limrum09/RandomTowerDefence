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

        if(!ValidateRosterData(wave, roster, out string errorMessage))
        {
            return new WavePrepareResult
            {
                state = WavePrepareState.Failed,
                message = errorMessage
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

    private bool ValidateRosterData(WaveData wave, List<WaveEnemyRosterData> roster, out string message)
    {
        for(int i = 0; i < roster.Count; i++)
        {
            WaveEnemyRosterData data = roster[i];

            if (data == null)
            {
                message = $"Wave roster row is null : {wave.waveUID},index : {i}";
                return false;
            }

            if(data.waveUID != wave.waveUID)
            {
                message = $"Wave roster UID not equal : wave {wave.waveUID}, roster : {data.waveUID}, index : {i}";
                return false;
            }

            if (string.IsNullOrEmpty(data.enemyUID))
            {
                message = $"Enemy UID is empty : enemy : {data.enemyUID}, wave : {wave.waveUID}, index : {i}";
                return false;
            }

            EnemyData enemyData = Managers.EnemyData.GetEnemyData(data.enemyUID);

            if(enemyData == null)
            {
                message = $"Enemy data Null : {data.enemyUID}, wave {wave.waveUID}, index : {i}";
                return false;
            }

            if(Managers.EnemySkillData.GetEnemySkillData(enemyData.enemySkillUID) == null)
            {
                message = $"Enemy skill data Null : {enemyData.enemySkillUID}, enemy : {data.enemyUID}, wave {wave.waveUID}, index : {i}";
                return false;
            }

            if (data.spawnOrder < 0)
            {
                message = $"Spawn order is Invalid : {wave.waveUID}, enemy : {data.enemyUID}, value : {data.spawnOrder}";
                return false;
            }

            if (data.enemyLevel < 0)
            {
                message = $"Enemy level is Invalid : {wave.waveUID}, enemy : {data.enemyUID}, value : {data.enemyLevel}";
                return false;
            }

            if (data.enemyCount <= 0)
            {
                message = $"Eenmy count is Invalid : {wave.waveUID}, enemy : {data.enemyUID}, value : {data.enemyCount}";
                return false;
            }

            if (data.startTime < 0)
            {
                message = $"Start time is Invalid : {wave.waveUID}, enemy : {data.enemyUID}, value : {data.startTime}";
                return false;
            }

            if (data.spawnInterval < 0)
            {
                message = $"Spawn Interval is Invalid : {wave.waveUID}, enemy : {data.enemyUID}, value : {data.spawnInterval}";
                return false;
            }
        }

        message = string.Empty;

        return true;
    }
}
