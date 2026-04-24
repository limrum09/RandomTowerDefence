using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WaveEnemyController : MonoBehaviour
{
    [SerializeField]
    private List<WaveEnemyInfo> infos = new List<WaveEnemyInfo>();
    private List<WaveEnemyRosterData> waveRoster;

    public event Action<WaveEnemyRosterData> onClickEnemyInfo;

    private int len;
    void Start()
    {
        len = infos.Count;

        for(int i = 0; i < len; i++)
        {
            infos[i].Init(this, i);
        }
    }

    public void GetWaveInfo(List<WaveEnemyRosterData> getWaveRoster)
    {
        if (getWaveRoster == null)
        {
            return;
        }
            

        waveRoster = getWaveRoster;

        int cnt = Mathf.Min(waveRoster.Count, len);

        Debug.Log($"Cnt : {cnt}, Waveroster.Count : {waveRoster.Count}, Len : {len}");

        for(int i = 0; i < cnt; i++)
        {
            infos[i].gameObject.SetActive(true);
            infos[i].SetWaveEnemyInfo(waveRoster[i].enemyUID, waveRoster[i].enemyLevel, waveRoster[i].enemyCount);
        }

        for(int i = cnt; i < infos.Count; i++)
        {
            infos[i].Clear();
            infos[i].gameObject.SetActive(false);
        }

    }

    public void OnClickEnemyInfo(int infoIndex)
    {
        WaveEnemyRosterData newData = waveRoster[infoIndex];
        Debug.Log("적 정보 보기");
        onClickEnemyInfo?.Invoke(newData);
    }
}
