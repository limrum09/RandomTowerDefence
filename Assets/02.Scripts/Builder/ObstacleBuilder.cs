using UnityEngine;
using UnityEngine.Rendering;
/// <summary>
/// 유저 장애물 설치 및 철거
/// 장애물 설치 / 제거 처리
/// 마우스 위치를 Grid Cell로 변환
/// 장애물 설치 가능 여부 검사
/// 장애무 설치 후에도 Spawn => Goal 경로가 유지되는지 검사
/// 장애물 설치 / 제거 시 재화 및 무료 서리 횟수 처리 
/// </summary>
public class ObstacleBuilder : MonoBehaviour
{
    [SerializeField] 
    private StageManager stage;         // 현재 스테이지 관리자
    [SerializeField] 
    private GridManager gridManager;    // 현재 스테이지의 GridManager
    [SerializeField] 
    private PathFinder pathFinder;      // 경로 차단 여부 확인용 PathFinder
    [SerializeField] 
    private Camera mainCamera;          // 마우스 위치 계산에 사용할 카메라
    [SerializeField] 
    private GameObject obstaclePrefab;  // 생성할 장애물 Prefab
    [SerializeField]
    private GameObject obstacles;       // 생성될 장애물을 관리할 부모 오브젝트
    [SerializeField]
    private int tempCost;               // 장애물 설치 비용

    [Header("Path Rule")]
    [SerializeField] 
    private Vector2Int spawnCell;       // 적 스폰 지점 셀
    [SerializeField] 
    private Vector2Int goalCell;        // 적 목표 지점 셀

    private GameObject[,] obstacleMap;  // Grid 좌표별 장애물을 GameObject에 저장
    private bool isObstacleMode;        // 장애물 설치 모드 여부
    private bool isRemoveObstacle;      // 장애물 제거 모드 여부
    private int obstacleCnt;            // 설치한 장애물 개수

    /// <summary>
    /// TowerController에서 타워 설치를 위해
    /// 해당 셀에 장애물이 있는지 확인
    /// </summary>
    /// <param name="cell">검사할 Grid Cell 좌표</param>
    /// <returns>장애물이 있다면 true반환</returns>
    public bool HasObstacle(Vector2Int cell)
    {
        // 셀이 Grid 범위 밖이면 없다고 처리
        if (!gridManager.IsInBounds(cell))
            return false;

        // 해당 셀의 장애물 정보 확인
        return obstacleMap[cell.x, cell.y] != null;
    }

    /// <summary>
    /// 초기화 
    /// StageManager에서 Grid, Path 정보가 준비된 후 호출
    /// </summary>
    public void Initialized()
    {
        // 카메라 참조가 없으면 Main Camera 사용
        if (mainCamera == null)
            mainCamera = Camera.main;

        // StageManager에서 GridManager와 PathFinder 참조 가져오기
        gridManager = stage.Grid;
        pathFinder = stage.Path;
        // Grid 크기에 맞추어서 장애물 맵 생성
        obstacleMap = new GameObject[gridManager.GridWidth, gridManager.GridHeight];

        GridRefreshHandler();

        gridManager.OnSetSpawnAndGoalPoint += GridRefreshHandler;
    }

    private void Awake()
    {
        // StageManager의 설정 완료 이후 초기화 되도록 이벤트 구독

        // 시작 시 설치 / 제거 모드 비활성화
        stage.OnAfterSettingsInit += Initialized;
        isObstacleMode = false;
        isRemoveObstacle = false;
    }

    private void Update()
    {
        // 장애물 설치 단축키 입력 시 설치 모드 진입
        if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.MakeObstacle)))
        {
            isObstacleMode = true;
            return;
        }

        // 장애물 제거 당축키 입력 시 제거 모드 진입
        if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.RemoveObstacle)))
        {
            isRemoveObstacle = true;
            return;
        }

        // 설치 모드에서 마우스 왼쪽 클릭 시 장애물 설치 시도
        if (isObstacleMode && Input.GetMouseButtonDown(0))
        {
            TryPlaceObstacle();
        }

        // 제거 모드에서 마우스 왼쪽 버튼 클릭 시 장애물 제거 시도
        if(isRemoveObstacle && Input.GetMouseButtonDown(0))
        {
            RemoveObstacle();
        }
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴시, 구독 취소
        stage.OnAfterSettingsInit -= Initialized;
        gridManager.OnSetSpawnAndGoalPoint -= GridRefreshHandler;
    }

    /// <summary>
    /// 마우스로 클릭한 위치의 장애물을 제거
    /// </summary>
    private void RemoveObstacle()
    {
        // 마우스를 월드 좌표로 변환
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        // 월드 좌표를 셀 좌표로 변환
        Vector2Int cell = gridManager.WorldToCell(mouseWorld);

        // Grid 범위 밖이면 제거 모드 종료
        if (!gridManager.IsInBounds(cell))
        {
            isRemoveObstacle = false;
            return;
        }
        // 시작 지점 / 목표 지점은 제거 대상이 아니기에 종료
        if (cell == spawnCell || cell == goalCell)
        {
            isRemoveObstacle = false;
            return;
        }
        // 해당 셀에 장애물이 없으면 종료
        if (obstacleMap[cell.x, cell.y] == null)
        {
            isRemoveObstacle = false;
            return;
        }
            
        // 장애물 오브젝트 제거
        GameObject obstacle = obstacleMap[cell.x, cell.y];
        Destroy(obstacle);
        // 해당 셀의 정보 초기화
        obstacleMap[cell.x, cell.y] = null;
        // Grid에서 이동가능 셀로 전환
        gridManager.SetBlocked(cell.x, cell.y, false);
        // 제거 모드 종료
        isRemoveObstacle = false;

        // 유료로 설치한 장애물이 있으면 골드 환급
        if(obstacleCnt > 0)
        {
            stage.RunSession.ChangeGold(tempCost);
            obstacleCnt--;
            return;
        }

        // 무료 설치 장애물을 제가한 경우 무료 설치 가능 횟수 봔환
        stage.RunSession.GetFreeObstacle(1);
    }

    /// <summary>
    /// 마우스로 클릭한 위치에 장애물 생성
    /// Grid 범위 안, 시작 지점 / 목표 지점이 아니어아 함
    /// 이미 장애물이 없어야 하고, 설치 이후에도 Spawn => Goal 경로가 존재해야함
    /// 무료 설치권 또는 골드가 충분해야 함
    /// </summary>
    private void TryPlaceObstacle()
    {
        // 마우스 위치를 Grid Cell 좌표로 변환
        Vector2Int cell = Managers.InputData.GetMouseCellPosition(mainCamera, gridManager);

        // Grid 범위 박이면 설치 실패
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
            isObstacleMode = false;
            return;
        }

        // 일단 막아보기. 이동 경로가 막히지 않았는지 확인하기 위함
        gridManager.SetBlocked(cell.x, cell.y, true);

        // 설치 이후에도 경로가 있는지 확인
        var testPath = pathFinder.FindPath(spawnCell, goalCell);

        // 경로가 막히면 설치 취소
        if (testPath == null || testPath.Count == 0)
        {
            // 막았던 셀을 이동가능 상태로 전환
            gridManager.SetBlocked(cell.x, cell.y, false);
            // 설치 모드 종료
            isObstacleMode = false;
            return;
        }

        // 무료 장애물 설치권이 있는지 확인
        if (!stage.RunSession.UsingFreeObstable())
        {
            // 무료 장애물 설치권이 없을 시, 골드가 부족하면 설치 실패
            if (!stage.RunSession.ChangeGold(-tempCost))
                return;
            // 유로 장애물 설치 개수 추가
            else
                obstacleCnt++;
        }

        // 실제 장애물 샐성 위치 게산
        Vector3 spawnPos = gridManager.CellToWorldCenter(cell.x, cell.y);
        // 장애물 Prefab 생성
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPos, Quaternion.identity, obstacles.transform);
        // 장애물 맵에 생성된 오브젝트 저장
        obstacleMap[cell.x, cell.y] = obstacle;
        // 설치 모드 종료
        isObstacleMode = false;
    }

    /// <summary>
    /// 현재 설치된 모든 장애물을 제거, obstacleMap과 Grid의 Blocked 상태를 초기화한다.
    /// </summary>
    private void ClearAllObstacles()
    {
        if(obstacleMap == null) 
            return;

        for(int i = 0; i < obstacleMap.GetLength(0); i++)
        {
            for(int j = 0; j < obstacleMap.GetLength(1); j++)
            {
                // 해당 셀에 장애물 오브젝트가 있으면 삭제
                if (obstacleMap[i,j]!= null)
                {
                    Destroy(obstacleMap[i, j]);
                    obstacleMap[i,j] = null;
                }

                // gridManager가 준비되어 있으면 이동 가능 상태로 변경
                if (gridManager != null)
                    gridManager.SetBlocked(i, j, false);
            }
        }
    }

    /// <summary>
    /// GridManager가 초기화 되었을때 액션에서 호출한다
    /// </summary>
    public void GridRefreshHandler()
    {
        ClearAllObstacles();

        // 시작 지점과 목표 지점 저장
        spawnCell = gridManager.SpawnPos;
        goalCell = gridManager.GoalPos;

        // 유로 설치 장애물 개수 초기화
        obstacleCnt = 0;
        isObstacleMode = false;
        isRemoveObstacle = false;
    }
}
