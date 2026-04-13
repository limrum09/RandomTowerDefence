using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBuilder : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private PathFinder pathFinder;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject obstaclePrefab;

    [Header("Path Rule")]
    [SerializeField] private Vector2Int spawnCell;
    [SerializeField] private Vector2Int goalCell;

    private GameObject[,] obstacleMap;

    public void Initialized()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        gridManager = Managers.Grid;
        obstacleMap = new GameObject[gridManager.GridWidth, gridManager.GridHeight];

        spawnCell = new Vector2Int(Managers.Grid.SpawnPos.x, Managers.Grid.SpawnPos.y);
        goalCell = new Vector2Int(Managers.Grid.GoalPos.x, Managers.Grid.GoalPos.y);
    }

    private void Awake()
    {
        Managers.Game.OnAfterSettingsInit += Initialized;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceObstacle();
        }
    }

    private void OnDestroy()
    {
        Managers.Game.OnAfterSettingsInit -= Initialized;
    }

    private void TryPlaceObstacle()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2Int cell = gridManager.WorldToCell(mouseWorld);

        if (!gridManager.IsInBounds(cell))
            return;

        // 시작칸 / 목표칸에는 설치 금지
        if (cell == spawnCell || cell == goalCell)
        {
            Debug.Log("시작점/도착점에는 설치할 수 없습니다.");
            return;
        }

        // 이미 장애물이 있으면 설치 금지
        if (obstacleMap[cell.x, cell.y] != null)
        {
            Debug.Log("이미 장애물이 있는 칸입니다.");
            return;
        }

        // 일단 막아보고 경로 존재 확인
        gridManager.SetBlocked(cell.x, cell.y, true);

        var testPath = pathFinder.FindPath(spawnCell, goalCell);

        if (testPath == null || testPath.Count == 0)
        {
            gridManager.SetBlocked(cell.x, cell.y, false);
            Debug.Log("길이 막혀서 설치할 수 없습니다.");
            return;
        }

        // 실제 장애물 생성
        Vector3 spawnPos = gridManager.CellToWorldCenter(cell.x, cell.y);
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity);
        obstacleMap[cell.x, cell.y] = obstacle;

        Debug.Log($"장애물 설치 완료: ({cell.x}, {cell.y})");
    }
}
