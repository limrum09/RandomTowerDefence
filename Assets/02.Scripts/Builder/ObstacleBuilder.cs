using UnityEngine;
/// <summary>
/// 유저 장애물 설치 및 철거
/// </summary>
public class ObstacleBuilder : MonoBehaviour
{
    [SerializeField] 
    private StageManager stage;
    [SerializeField] 
    private GridManager gridManager;
    [SerializeField] 
    private PathFinder pathFinder;
    [SerializeField] 
    private Camera mainCamera;
    [SerializeField] 
    private GameObject obstaclePrefab;
    [SerializeField]
    private GameObject obstacles;
    [SerializeField]
    private int tempCost;

    [Header("Path Rule")]
    [SerializeField] 
    private Vector2Int spawnCell;
    [SerializeField] 
    private Vector2Int goalCell;

    private GameObject[,] obstacleMap;
    private bool isObstacleMode;
    private bool isRemoveObstacle;
    private int obstacleCnt;
    public bool HasObstacle(Vector2Int cell)
    {
        if (!gridManager.IsInBounds(cell))
            return false;

        return obstacleMap[cell.x, cell.y] != null;
    }

    public void Initialized()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        gridManager = stage.Grid;
        pathFinder = stage.Path;
        obstacleMap = new GameObject[gridManager.GridWidth, gridManager.GridHeight];

        spawnCell = new Vector2Int(stage.SpawnPos.x, stage.SpawnPos.y);
        goalCell = new Vector2Int(stage.GoalPos.x, stage.GoalPos.y);

        obstacleCnt = 0;
    }

    private void Awake()
    {
        stage.OnAfterSettingsInit += Initialized;
        isObstacleMode = false;
        isRemoveObstacle = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.MakeObstacle)))
        {
            isObstacleMode = true;
        }

        if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.RemoveObstacle)))
        {
            isRemoveObstacle = true;
        }

        if (isObstacleMode && Input.GetMouseButtonDown(0))
        {
            TryPlaceObstacle();
        }

        if(isRemoveObstacle && Input.GetMouseButtonDown(0))
        {
            RemoveObstacle();
        }
    }

    private void OnDestroy()
    {
        stage.OnAfterSettingsInit -= Initialized;
    }

    private void RemoveObstacle()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2Int cell = gridManager.WorldToCell(mouseWorld);

        if (!gridManager.IsInBounds(cell))
        {
            isRemoveObstacle = false;
            return;
        }
            

        if (cell == spawnCell || cell == goalCell)
        {
            isRemoveObstacle = false;
            return;
        }

        if (obstacleMap[cell.x, cell.y] == null)
        {
            isRemoveObstacle = false;
            return;
        }
            

        GameObject obstacle = obstacleMap[cell.x, cell.y];
        Destroy(obstacle);
        obstacleMap[cell.x, cell.y] = null;
        gridManager.SetBlocked(cell.x, cell.y, false);

        isRemoveObstacle = false;

        if(obstacleCnt > 0)
        {
            stage.RunSession.ChangeGold(tempCost);
            obstacleCnt--;
            return;
        }

        stage.RunSession.GetFreeObstacle(1);
    }

    /// <summary>
    /// 장애물 생성
    /// </summary>
    private void TryPlaceObstacle()
    {
        Vector2Int cell = Managers.InputData.GetMouseCellPosition(mainCamera, gridManager);
        Debug.Log("셀 위치 : " + cell);

        if (!gridManager.IsInBounds(cell))
            return;

        // 시작칸 / 목표칸에는 설치 금지
        if (cell == spawnCell || cell == goalCell)
        {
            isObstacleMode = false;
            return;
        }

        // 이미 장애물이 있으면 설치 금지
        if (obstacleMap[cell.x, cell.y] != null)
        {
            Debug.Log("설치 실패");
            isObstacleMode = false;
            return;
        }

        // 일단 막아보고 경로 존재 확인
        gridManager.SetBlocked(cell.x, cell.y, true);

        var testPath = pathFinder.FindPath(spawnCell, goalCell);

        if (testPath == null || testPath.Count == 0)
        {
            gridManager.SetBlocked(cell.x, cell.y, false);
            isObstacleMode = false;
            return;
        }

        if (!stage.RunSession.UsingFreeObstable())
        {
            if (!stage.RunSession.ChangeGold(tempCost))
                return;
            else
                obstacleCnt++;
        }

        // 실제 장애물 생성
        Vector3 spawnPos = gridManager.CellToWorldCenter(cell.x, cell.y);
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, obstacles.transform);
        obstacleMap[cell.x, cell.y] = obstacle;
        isObstacleMode = false;
    }
}
