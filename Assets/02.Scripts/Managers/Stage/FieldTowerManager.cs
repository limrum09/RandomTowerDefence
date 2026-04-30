using System;
using System.Collections.Generic;
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

    public bool HasTower(Vector2Int cell)
    {
        if (!IsValidCell(cell))
            return false;

        return towerMap[cell.x, cell.y] != null;
    }

    public Tower GetTower(Vector2Int cell)
    {
        if (!IsValidCell(cell))
            return null;
        
        return towerMap[cell.x, cell.y];
    }

    public int GetTowerCount(Tower tower) => GetTowerCount(tower.Type);
    public int GetTowerCount(TowerType type)
    {
        if (towerTypeCnt.TryGetValue(type, out var count))
                return count;

        return 0;
    }

    public int GetTotalTowerCount() => fieldTowers.Count;

    public List<Tower> GetAllTowers()
    {
        return new List<Tower>(fieldTowers);
    }

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

    public bool RemoveTowers(List<Tower> removeTowers)
    {
        if (removeTowers == null || removeTowers.Count == 0)
            return false;

        int cnt = removeTowers.Count;
        TowerType type = removeTowers[0].Type;

        for (int i = 0; i < cnt; i++)
        {
            RemoveTower(removeTowers[i], false);
        }

        int tCnt = GetTowerCount(type);
        OnFieldTowerChanged?.Invoke(type, tCnt);
        return true;
    }
    
    public bool RemoveTower(Tower tower, bool notify = true)
    {
        if(tower == null) 
            return false;

        TowerMove move = tower.GetComponent<TowerMove>();
        if (move == null)
            return false;

        Vector2Int cell = move.GetTowerPosition();

        if (!UnRegisterTower(tower, cell, notify))
            return false;

        UnityEngine.Object.Destroy(tower.gameObject);

        return true;
    }
}
