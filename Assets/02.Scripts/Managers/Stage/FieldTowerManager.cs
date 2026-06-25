using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FieldTowerManager
{
    private GridManager grid;
    private Tower[,] towerMap;
    private readonly List<Tower> fieldTowers = new List<Tower>();
    private readonly Dictionary<TowerType, int> towerTypeCnt = new Dictionary<TowerType, int>();

    public event Action<TowerType, int> OnFieldTowerChanged;

    private bool IsValidCell(Vector2Int cell)
    {
        return grid != null && grid.IsInBounds(cell);
    }

    /// <summary>
    /// 타워가 towerMap 어디 있는지 반환
    /// </summary>
    /// <param name="tower"></param>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool TryGetRegisteredTowerCell(Tower tower, out Vector2Int cell)
    {
        cell = Vector2Int.zero;

        if (tower == null || towerMap == null || grid == null)
            return false;

        if (!fieldTowers.Contains(tower))
            return false;

        for(int i = 0; i < grid.GridWidth; i++)
        {
            for(int j = 0; j < grid.GridHeight; j++)
            {
                if(towerMap[i, j] == tower)
                {
                    cell = new Vector2Int(i, j);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 받은 타워들이 제거해도 되는지 확인
    /// </summary>
    /// <param name="removeTowers"></param>
    /// <returns></returns>
    private bool CanRemoveTowers(List<Tower> removeTowers)
    {
        if (removeTowers == null || removeTowers.Count <= 0)
            return false;

        HashSet<Tower> checkDuplicate = new HashSet<Tower>();

        foreach(Tower tower in removeTowers)
        {
            if (tower == null)
                return false;

            if (!checkDuplicate.Add(tower))
                return false;

            if(!TryGetRegisteredTowerCell(tower, out Vector2Int cell))
                return false;
        }

        return true;
    }

    public void Init(GridManager getGrid)
    {
        grid = getGrid;

        if (grid == null)
            return;

        towerMap = new Tower[grid.GridWidth, grid.GridHeight];

        fieldTowers.Clear();
        towerTypeCnt.Clear();

        foreach (TowerType t in Enum.GetValues(typeof(TowerType)))
            towerTypeCnt[t] = 0;
    }

    public bool RegisterTower(Tower tower, Vector2Int cell)
    {
        if (tower == null)
            return false;

        if (!IsValidCell(cell))
            return false;

        // 지금은 생성 불가능, 나중에 대기열로 돌아가는 로직 추가 필요
        if (towerMap[cell.x, cell.y] != null)
            return false;

        if (fieldTowers.Contains(tower))
            return false;

        towerMap[cell.x, cell.y] = tower;
        fieldTowers.Add(tower);

        if (!towerTypeCnt.ContainsKey(tower.Type))
            towerTypeCnt[tower.Type] = 0;

        towerTypeCnt[tower.Type]++;

        int cnt = GetTowerCount(tower.Type);
        OnFieldTowerChanged?.Invoke(tower.Type, cnt);

        return true;
    }

    public bool UnRegisterTower(Tower tower, Vector2Int cell, bool notify = true)
    {
        if(tower == null)
            return false;

        if (!IsValidCell(cell))
            return false;

        if (towerMap[cell.x, cell.y] != tower)
            return false;

        towerMap[cell.x, cell.y] = null;
        fieldTowers.Remove(tower);

        if (towerTypeCnt.ContainsKey(tower.Type))
            towerTypeCnt[tower.Type] = Mathf.Max(0, towerTypeCnt[tower.Type] - 1);

        if (notify)
        {
            int cnt = GetTowerCount(tower.Type);
            OnFieldTowerChanged?.Invoke(tower.Type, cnt);
        }

        return true;
    }

    /// <summary>
    /// 받은 타워의 fromCell에서 toCell로 위치를 이동
    /// </summary>
    /// <param name="tower"></param>
    /// <param name="fromCell"></param>
    /// <param name="toCell"></param>
    /// <returns></returns>
    public bool MoveTower(Tower tower, Vector2Int fromCell, Vector2Int toCell)
    {
        if (tower == null)
            return false;

        if (!IsValidCell(fromCell) || !IsValidCell(toCell))
            return false;

        if (towerMap[fromCell.x, fromCell.y] != tower)
            return false;

        if (towerMap[toCell.x, toCell.y] != null)
            return false;

        towerMap[fromCell.x, fromCell.y] = null;
        towerMap[toCell.x, toCell.y] = tower;

        int cnt = GetTowerCount(tower.Type);
        OnFieldTowerChanged?.Invoke(tower.Type, cnt);
        return true;
    }

    /// <summary>
    /// tower1Cell과 tower2Cell의 좌표에 있는 두개의 타워의 위치를 변환
    /// </summary>
    /// <param name="tower1Cell"></param>
    /// <param name="tower2Cell"></param>
    /// <returns></returns>
    public bool SwapTower(Vector2Int tower1Cell, Vector2Int tower2Cell)
    {
        if(!IsValidCell(tower1Cell)  || !IsValidCell(tower2Cell)) 
            return false;

        Tower tower1 = towerMap[tower1Cell.x, tower1Cell.y];
        Tower tower2 = towerMap[tower2Cell.x, tower2Cell.y];

        if (tower1 == null || tower2 == null)
            return false;

        towerMap[tower1Cell.x, tower1Cell.y] = tower2;
        towerMap[tower2Cell.x, tower2Cell.y] = tower1;

        return true;
    }

    /// <summary>
    /// 타워 등급 업그레이드
    /// </summary>
    /// <param name="selectTower"></param>
    /// <param name="needCount"></param>
    /// <param name="towers"></param>
    /// <returns></returns>
    public bool TryGetGradeUpgradeTower(Tower selectTower, int needCount, out List<Tower> towers)
    {
        towers = new List<Tower>();

        if (selectTower == null)
            return false;

        if (selectTower.Grade == 6)
            return false;

        if (string.Equals(selectTower.nextGradeUID, "MASTER") || string.Equals(selectTower.nextGradeUID, "Master"))
            return false;

        foreach(Tower tower in fieldTowers)
        {
            if (tower == null)
                continue;

            if (tower.Grade != selectTower.Grade)
                continue;

            if (tower.Type != selectTower.Type)
                continue;

            TowerMove move = tower.GetComponent<TowerMove>();
            if (move == null)
                continue;

            towers.Add(tower);

            if (towers.Count >= needCount)
                break;
        }

        return towers.Count == needCount;
    }

    /// <summary>
    /// 넘겨 받은 모든 타워들 제거
    /// </summary>
    /// <param name="removeTowers"></param>
    /// <returns></returns>
    public bool RemoveTowers(List<Tower> removeTowers, out List<int> removedIndex)
    {
        removedIndex = new List<int>();

        if (!CanRemoveTowers(removeTowers))
            return false;

        TowerType type = removeTowers[0].Type;

        for(int i = 0; i < removeTowers.Count; i++)
        {
            if (RemoveTower(removeTowers[i], false))
            {
                removedIndex.Add(i);
                continue;
            }

            int failCnt = GetTowerCount(type);
            OnFieldTowerChanged?.Invoke(type, failCnt);
            return false;
        }

        int tCnt = GetTowerCount(type);
        OnFieldTowerChanged?.Invoke(type, tCnt);
        return true;
    }
    
    /// <summary>
    /// 넘겨 받은 타워 하나 제거
    /// </summary>
    /// <param name="tower"></param>
    /// <param name="notify"></param>
    /// <returns></returns>
    public bool RemoveTower(Tower tower, bool notify = true)
    {
        if(tower == null) 
            return false;

        if (!TryGetRegisteredTowerCell(tower, out Vector2Int cell))
            return false;

        if (!UnRegisterTower(tower, cell, notify))
            return false;

        UnityEngine.Object.Destroy(tower.gameObject);

        return true;
    }

    /// <summary>
    /// 모든 타워의 스텟을 재확인
    /// </summary>
    public void RefreshAllTowerDamageStats()
    {
        foreach(Tower tower in fieldTowers)
        {
            if (tower == null)
                continue;

            tower.RefreshStats();
        }
    }

    /// <summary>
    /// 해당 좌표에 타워가 있는지 반환
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public bool HasTower(Vector2Int cell)
    {
        if (!IsValidCell(cell))
            return false;

        return towerMap[cell.x, cell.y] != null;
    }

    /// <summary>
    /// 해당 좌표에 있는 타워를 반환
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public Tower GetTower(Vector2Int cell)
    {
        if (!IsValidCell(cell))
            return null;

        return towerMap[cell.x, cell.y];
    }

    /// <summary>
    /// 받은 타워의 같은 타입의 타워의 개수를 반환
    /// </summary>
    /// <param name="tower"></param>
    /// <returns></returns>
    public int GetTowerCount(Tower tower) => GetTowerCount(tower.Type);

    /// <summary>
    /// 받은 타워의 타입을 가지고 있는 타워들의 개수를 반환
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public int GetTowerCount(TowerType type)
    {
        if (towerTypeCnt.TryGetValue(type, out var count))
            return count;

        return 0;
    }

    /// <summary>
    /// 모든 타워의 개수를 반환
    /// </summary>
    /// <returns></returns>
    public int GetTotalTowerCount() => fieldTowers.Count;

    /// <summary>
    /// 모든 타워의 정보를 반환
    /// </summary>
    /// <returns></returns>
    public List<Tower> GetAllTowers()
    {
        return new List<Tower>(fieldTowers);
    }

    /// <summary>
    /// 필드에 있는 모든 타워의 데이터를 TowerResultData로 가공하여 반환
    /// </summary>
    /// <returns></returns>
    public List<TowerResultData> GetTowerResultData()
    {
        Dictionary<TowerType, TowerResultData> result = new Dictionary<TowerType, TowerResultData>();

        foreach(Tower tower in fieldTowers)
        {
            if (tower == null)
                continue;

            TowerType towerType = tower.Type;

            if(!result.TryGetValue(towerType, out TowerResultData data))
            {
                data = new TowerResultData
                {
                    icon = ResourceCache.Load<Sprite>($"Tower/Images/Icon_Tower_{towerType}_{1}_Idle"),
                    type = towerType,
                    count = 0,
                    sellValueTotal = 0
                };

                result.Add(towerType, data);
            }

            data.count++;
            data.sellValueTotal += tower.SellPrice;
        }

        return result.Values.ToList();
    }
}
