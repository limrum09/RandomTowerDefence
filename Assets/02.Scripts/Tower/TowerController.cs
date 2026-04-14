using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    [SerializeField]
    private StageManager stage;
    [SerializeField]
    private ObstacleBuilder obstacle;
    [SerializeField] 
    private Camera mainCamera;
    [SerializeField]
    private GameObject towerPre;

    private List<Tower> fieldTowers = new List<Tower>();
    private GridManager grid;
    private bool isTowerMove;
    private Tower[,] towerMap;
    private Tower selectedTower;
    private bool isBuildMode;
    private string pendingTowerUID;
    private QueueController tempQueue;
    private int queIndex;
    private Vector2Int selectTowerCell;
    private void Start()
    {
        pendingTowerUID = string.Empty;
        isBuildMode = false;
        grid = stage.Grid;
        towerMap = new Tower[grid.GridWidth, grid.GridHeight];
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTowerMove && Input.GetMouseButtonDown(0))
        {
            ClickTower();
        }

        if(isTowerMove && Input.GetMouseButtonDown (0))
        {
            TryMoveTower();
        }

        if (isBuildMode && Input.GetMouseButtonDown(0))
        {
            TryBuildPendingTower();
        }
    }
    public void BeginBuildTower(string towerUID, int index, QueueController tQueue)
    {
        pendingTowerUID = towerUID;
        isBuildMode = true;
        isTowerMove = false;
        selectedTower = null;

        tempQueue = tQueue;
        queIndex = index;
    }

    private void TryBuildPendingTower()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2Int cell = grid.WorldToCell(mouseWorld);

        bool success = BuildTower(pendingTowerUID, cell);

        isBuildMode = false;
        pendingTowerUID = string.Empty;

        if (!success)
        {
            Debug.Log("타워 설치 실패");
        }

        tempQueue.RemoveTower(queIndex);
        queIndex = 0;
        tempQueue = null;
    }


    private bool BuildTower(string towerUID, Vector2Int cell)
    {
        if (HasTower(cell))
        {
            Debug.Log("타워가 존재");
            return false;
        }

        if (!obstacle.HasObstacle(cell))
        {
            Debug.Log("장애물이 없음");
            return false;
        }

        if (!grid.IsInBounds(cell))
        {
            isTowerMove = false;
            Debug.Log("바깥");
            return false;
        }

        if (cell == stage.SpawnPos || cell == stage.GoalPos)
        {
            Debug.Log("스폰 지역, 골 지역 선택 불가");
            isTowerMove = false;
            return false;
        }

        Vector3 worldPos = grid.CellToWorldCenter(cell.x, cell.y);
        GameObject towerObj = Instantiate(towerPre, worldPos, Quaternion.identity);

        Tower tower = towerObj.GetComponent<Tower>();
        if (tower == null)
        {
            Debug.Log("타워가 없음");
            Destroy(towerObj);
            return false;
        }

        tower.Init(towerUID, fieldTowers.Count);

        TowerMove towerMove = towerObj.GetComponent<TowerMove>();
        if (towerMove != null)
        {
            towerMove.SetTowerInit(stage);
            towerMove.SetTowerPosition(cell);
        }

        towerMap[cell.x, cell.y] = tower;
        fieldTowers.Add(tower);

        return true;
    }

    private void ClickTower()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(mousePosition.x, mousePosition.y);

        Collider2D hit = Physics2D.OverlapPoint(point);
        if (hit == null)
            return;

        Tower tower = hit.GetComponent<Tower>();
        if(tower == null) 
            return;

        TowerData data = Managers.TowerData.GetTowerData(tower.TowerUID);
        if (data == null)
            return;

        if (Input.GetKey(Managers.InputKey.GetKeyCode(InputAction.MoveTower)))
        {
            selectedTower = tower;
            isTowerMove = true;
            selectTowerCell = grid.WorldToCell(mousePosition);
        }
    }

    private void TryMoveTower()
    {
        if(selectedTower == null)
        {
            Debug.Log("선택된 타워 없음");
            isTowerMove = false;
            return;
        }

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2Int cell = grid.WorldToCell(mouseWorld);

        if (!grid.IsInBounds(cell))
        {
            Debug.Log("그리드 넘어감");
            isTowerMove = false;
            return;
        }

        if(cell == stage.SpawnPos || cell == stage.GoalPos)
        {
            Debug.Log("시작, 끝 위치 않됨");
            isTowerMove = false;
            return;
        }

        if (!obstacle.HasObstacle(cell))
        {
            Debug.Log("장애물 위가 아님");
            isTowerMove = false;
            return;
        }

        if (HasTower(cell))
        {
            Debug.Log("타워가 있음");
            isTowerMove = false;
            return;
        }

        
        TowerMove move = selectedTower.gameObject.GetComponent<TowerMove>();
        move.SetTowerPosition(cell);
        towerMap[cell.x, cell.y] = selectedTower;
        towerMap[selectTowerCell.x, selectTowerCell.y] = null;
        Debug.Log("이동 완료");
    }

    public bool HasTower(Vector2Int cell)
    {
        if (!grid.IsInBounds(cell))
            return false;

        return towerMap[cell.x, cell.y] != null;
    }
}
