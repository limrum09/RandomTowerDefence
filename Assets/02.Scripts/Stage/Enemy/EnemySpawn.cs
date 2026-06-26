using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 한 종류의 적 스폰 정보를 담는 데이터 클래스
/// WaveEnemyrosterData에서 필요한 값만 받아 EnemySpawn에서 사용하기 쉽게 변환
/// </summary>
[Serializable]
public class EnemySpawnInfo
{
    public string enemyUID;     // 생성할 적 UID
    public int spawnOrder;      // 웨이브 내 스폰 순서
    public int level;           // 생성할 적 레벨
    public int spawnCnt;        // 생성할 적 개수
    public float startTime;     // 웨이브 시작 후 몇 초 뒤에 스폰을 시작할지 지정하는 값
    public float spawnInterval; // 1마리당 생성 간격
    public EnemySpawnInfo(string enemyUID, int spawnOrder, int level, int spawnCnt, float startTime, float spawnInterval)
    {
        this.enemyUID = enemyUID;
        this.spawnOrder = spawnOrder;
        this.level = level;
        this.spawnCnt = spawnCnt;
        this.startTime = startTime;
        this.spawnInterval = spawnInterval;
    }
}
/// <summary>
/// 적 스폰 관리
/// 웨이브 적 목록 저장, 스폰 위치 설정, 웨이브 시작 시 적을 순서대로 생성
/// 생성된 적의 사망/도착 이벤트를 StageManager에 전달
/// </summary>
public class EnemySpawn : MonoBehaviour
{
    [SerializeField]
    private Enemy baseEnemy;            // 생성할 기본 Enemy Prefab
    [SerializeField]
    private Transform spawnEnemysParent;// 생성할 적의 Hierachy 위치
    // 현재 웨이브에서 스폰할 적 정보
    [SerializeField]
    private List<EnemySpawnInfo> waveEnemySpawnsInfo = new List<EnemySpawnInfo>();

    private EnemyFactory enemyFactory;
    private GridManager grid;           // 적 이동 시에 사용할 GridManager
    private Vector3 spawnPoint;         // 적이 생성될 월드 좌표
    private bool isSpawnActive;
    private Coroutine waveCoroutine;
    private Coroutine enemySpawnCoroutine;

    public EnemyFactory EnemyFactory => enemyFactory;
    public event Action OnSpawnEnd;     // 스폰이 종료되면 호출
    

    private void Start()
    {
        waveEnemySpawnsInfo.Clear();
    }

    /// <summary>
    /// WaveEnemyRosterData 모록을 EnemySpawnInfo 목록으로 변환하여 저장
    /// StageWaveaManager에서 웨이브 정보가 결정된 후 호출
    /// </summary>
    /// <param name="enemyRoster">현재 웨이브에 등록할 적 목록</param>
    public void SetSpawnEnemyInfo(List<WaveEnemyRosterData> enemyRoster)
    {
        // 이전 정보 제거
        waveEnemySpawnsInfo.Clear();

        if(enemyRoster == null || enemyRoster.Count <= 0)
        {
            Debug.LogError("Enemy roster is Empty");
            return;
        }
    
        for (int i = 0; i < enemyRoster.Count; i++)
        {
            // 데이터 테이블의 WaveEnmeyRosterData를 EnemySpaen에서 사용할 EnemySpawnInfo로 변환
            EnemySpawnInfo newEnemy = new EnemySpawnInfo(enemyRoster[i].enemyUID,enemyRoster[i].spawnOrder, enemyRoster[i].enemyLevel,
                enemyRoster[i].enemyCount, enemyRoster[i].startTime, enemyRoster[i].spawnInterval);

            // 현재 웨이브 스폰 목록에 추가
            waveEnemySpawnsInfo.Add(newEnemy);
        }
    }

    /// <summary>
    /// 초기화
    /// GridManageㄱ과 PathFinder를 박아서 적 생성 위치와 이동 경로 게산에 사용
    /// </summary>
    /// <param name="getGrid"></param>
    /// <param name="getPath"></param>
    public void SetInitialized(GridManager getGrid, PathFinder getPath)
    {
        grid = getGrid;

        enemyFactory = new EnemyFactory(baseEnemy, spawnEnemysParent, getGrid, getPath);
    }

    /// <summary>
    /// 적 스폰 시작
    /// </summary>
    public void EnemySpawnStart()
    {
        enemyFactory.SetCanSpawn(true);

        isSpawnActive = true;
        spawnPoint = grid.CellToWorldCenter(grid.SpawnPos.x, grid.SpawnPos.y);

        if (waveCoroutine != null)
            StopCoroutine(waveCoroutine);


        waveCoroutine = StartCoroutine(StartWave());
    }


    /// <summary>
    /// 오로지 적 1마리를 생성하고 입력 경로 초기화
    /// </summary>
    /// <param name="spawnInfo">생성할 적 정보</param>
    private void SpawnOneEnemy(EnemySpawnInfo spawnInfo)
    {
        if (!isSpawnActive)
            return;

        bool success = enemyFactory.SpawnEnemy(spawnInfo.enemyUID, spawnInfo.level, spawnPoint);

        if (!success)
        {
            Debug.LogError($"Enemy spawn failed : {spawnInfo.enemyUID}, level : {spawnInfo.level}");
        }
    }

    /// <summary>
    /// 현재 웨이브의 적을 순서오 시간에 맞춰서 생성
    /// </summary>
    /// <returns></returns>
    IEnumerator StartWave()
    {
        // spawnOrder기준으로 정렬
        waveEnemySpawnsInfo.Sort((x,y) => x.spawnOrder.CompareTo(y.spawnOrder));
        // 웨이브 시작 시간 저장
        float waveStartTime = Time.time;

        foreach(EnemySpawnInfo info in waveEnemySpawnsInfo)
        {
            // 이 적 그룹이 스폰을 시작해야하는 절대 시간
            float targetStartTime = waveStartTime + info.startTime;
            // 현재 시간 기준으로 기다려야하는 시간 게산
            float waitTime = targetStartTime - Time.time;

            // 시간이 안되면 대시
            if(waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            // 해당 저거 그릅 스폰 시작
            enemySpawnCoroutine = StartCoroutine(StartEnemySpawn(info));
            yield return enemySpawnCoroutine;
            enemySpawnCoroutine = null;
        }

        waveCoroutine = null;
        // 스폰 종료 이벤트 호출
        OnSpawnEnd?.Invoke();
        yield return null;
    }

    /// <summary>
    /// 하나의 EnemySpawnInfo에 해당하는 적을 생성
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    private IEnumerator StartEnemySpawn(EnemySpawnInfo info)
    {
        for(int i = 0; i < info.spawnCnt; i++)
        {
            if (!isSpawnActive)
                yield break;

            // 적 1마리 생성
            SpawnOneEnemy(info);

            if(!isSpawnActive)
                yield break;

            // 스폰 시간동안 대기
            yield return new WaitForSeconds(info.spawnInterval);
        }
    }

    private void RemoveAllEnemies()
    {
        for(int i = spawnEnemysParent.childCount - 1; i >= 0 ; i--)
        {
            Destroy(spawnEnemysParent.GetChild(i).gameObject);
        }
    }

    private void StopSpawnCoroutines()
    {
        if(enemySpawnCoroutine != null)
        {
            StopCoroutine(enemySpawnCoroutine);
            enemySpawnCoroutine = null;
        }

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }
    }

    public void GameOver()
    {
        isSpawnActive = false;
        enemyFactory.SetCanSpawn(false);

        StopSpawnCoroutines();
        RemoveAllEnemies();
    }
}
