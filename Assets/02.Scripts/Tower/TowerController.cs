using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField]
    private StageManager stage;
    [SerializeField]
    private ObstacleBuilder obstacle;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject towerPre;
    [SerializeField]
    private GameObject towers;

    [Header("Viewer")]
    [SerializeField]
    private TowerGradeUpgradeView towerGradeUpgradeView;
    [SerializeField]
    private TowerActionMenuView towerActionMenuView;

    [SerializeField]
    private TowerUIController towerUICtr;

    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();
    /*// 타워 상세 UI 제어
    private TowerGradeUpgradePresenter towerGradeUpgradePreseter;
    // 타워 클릭시 보이는 버튼 제어
    private TowerActionMenuPresenter towerActionMenuPresenter;*/
    // StageManager의 그리드 참조
    private GridManager grid;

    // 필드에 배치된 타워 목록
    private List<Tower> fieldTowers = new List<Tower>();
    // 그리드 기준 타워 배치 상태 저장
    private Tower[,] towerMap;
    // 현재 선택된 타워
    private Tower selectedTower;
    // 선택된 타워의 현제 셀 위치
    private Vector2Int selectedTowerCell;
    // 설치 대기 중인 타워 UID
    private string selectedTowerUID;

    // 등급 업그레이드 시 필요한 타워 개수
    private int needupgradeTowerCnt;

    // 타워이동 모드 여부
    private bool isTowerMove;
    // 타워 설치 모드 여부
    private bool isBuildMode;
    private bool isGradeUpgradeMode;
    private bool isStatUpgradeMode;

    // 설치 성공 시, 제거할 대기열 정보
    private QueueController tempQueue;
    private int queIndex;

    private void Start()
    {
        selectedTowerUID = string.Empty;
        isBuildMode = false;
        grid = stage.Grid;
        isTowerMove = false;
        isGradeUpgradeMode = false;
        isStatUpgradeMode = false;
        towerMap = new Tower[grid.GridWidth, grid.GridHeight];
        needupgradeTowerCnt = 3;
    }


    /// <summary>
    /// 현재 상태에 따라 타워 이동, 타워 설치, 타워 성택 입력을 처리
    /// </summary>
    void Update()
    {
        if (isTowerMove)
        {
            if (Input.GetMouseButtonDown(0))
                TryMoveTower();

            return;
        }

        if (isBuildMode)
        {
            if (Input.GetMouseButtonDown(0))
                TryBuildPendingTower();

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            ClickTower();
        }

        if (selectedTower != null)
        {
            if (Input.GetKeyDown(Managers.InputKey.GetKeyCode(InputAction.MoveTower)))
            {
                SetTowerMoveMode();
                towerUICtr.ClearSelection();
                return;
            }

            if (Input.GetKeyDown(Managers.InputKey.GetKeyCode(InputAction.ShowGradeUpgradeTowerView)))
            {
                isGradeUpgradeMode = true;
                towerUICtr.OnClickGradeUpgrade(selectedTower);
                return;
            }

            if (Input.GetKeyDown(Managers.InputKey.GetKeyCode(InputAction.ShowStatUpgradeTowerView)))
            {
                isStatUpgradeMode = true;
                towerUICtr.OnClickStatUpgrade(selectedTower);
                return;
            }

            if(isGradeUpgradeMode && Input.GetKeyDown(Managers.InputKey.GetKeyCode(InputAction.TowerGradeNormalUpgrade)))
            {
                isGradeUpgradeMode = false;
                TowerGradeNormalUpgrade();
            }

            if (isGradeUpgradeMode && Input.GetKeyDown(Managers.InputKey.GetKeyCode(InputAction.TowerGradePremiunUpgrade)))
            {
                isGradeUpgradeMode = false;
                TowerGradePreminumUpgrade();
            }
        }
    }

    /// <summary>
    /// 대기열의 타워를 설치할 수 있도록 설치 모드를 시작
    /// </summary>
    /// <param name="towerUID">대기열에서 선택된 타워의 UID</param>
    /// <param name="index">대기열에서 타워 위치</param>
    /// <param name="tQueue">대기열 정보</param>
    public void BeginBuildTower(string towerUID, int index, QueueController tQueue)
    {
        selectedTowerUID = towerUID;
        isBuildMode = true;
        selectedTower = null;

        tempQueue = tQueue;
        queIndex = index;
    }

    /// <summary>
    /// 클릭한 셀에 설치 대기 중인 타워를 배치 시도
    /// 설치 성공 시 대기열에서 해당 타워를 제거
    /// </summary>
    private void TryBuildPendingTower()
    {
        Vector2Int cell = GetMouseCellPosition();
        bool success = BuildTower(selectedTowerUID, cell);

        if (!success)
        {
            Debug.Log("타워 설치 실패");
            EndBuildMode();
            return;
        }

        tempQueue.RemoveTower(queIndex);
        EndBuildMode();
    }

    /// <summary>
    /// 지정한 셀에 타워를 생성하고 맵 정보에 등록
    /// </summary>
    /// <param name="towerUID">설치할 타워의 UID</param>
    /// <param name="cell">타워 설치를 위한 셀</param>
    /// <returns></returns>
    private bool BuildTower(string towerUID, Vector2Int cell)
    {
        if (!CanPlaceTower(cell))
            return false;
        ClearSelectedTower();

        Vector3 worldPos = grid.CellToWorldCenter(cell.x, cell.y);
        GameObject towerObj = Instantiate(towerPre, worldPos, Quaternion.identity, towers.transform);

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

    /// <summary>
    /// 클릭한 오브젝트가 타워인지 판별하고, 선택된 타워 및 상세 UI를 갱신
    /// </summary>
    private void ClickTower()
    {
        Debug.Log("타워 클릭");

        if (IsPointerOverUI())
            return;

        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = new Vector2(mousePosition.x, mousePosition.y);

        Collider2D hit = Physics2D.OverlapPoint(point);
        if (hit == null)
        {
            ClearSelectedTower();
            Debug.Log("아무것도 안부딪힘");
            return;
        }

        Tower tower = hit.GetComponent<Tower>();
        if (tower == null)
        {
            ClearSelectedTower();
            Debug.Log("타워 없음");
            return;
        }

        TowerData data = Managers.TowerData.GetTowerData(tower.TowerUID);
        if (data == null)
        {
            ClearSelectedTower(false);
            return;
        }

        TowerMove move = tower.GetComponent<TowerMove>();
        if (move == null)
        {
            ClearSelectedTower(false);
            return;
        }

        Debug.Log("타워 있음");

        towerUICtr.ClearSelection();
        isGradeUpgradeMode = false;
        isStatUpgradeMode = false;

        if (selectedTower != null)
            selectedTower.ShowAttackRange(false);

        selectedTower = tower;
        selectedTower.ShowAttackRange(true);
        selectedTowerCell = move.GetTowerPosition();

        towerUICtr.SetSelectedTower(selectedTower);

        Debug.Log("타워 이동 입력 : ");
    }

    /// <summary>
    /// 선택된 타워를 클릭한 셀로 이동
    /// 대상 셀에 다른 타워가 있으면 서로 위치를 교환
    /// </summary>
    private void TryMoveTower()
    {
        if (selectedTower == null)
        {
            Debug.Log("선택된 타워 없음");
            EndMoveMode();
            return;
        }

        Vector2Int cell = GetMouseCellPosition();

        if (!CanUseTowerCell(cell))
        {
            EndMoveMode();
            return;
        }

        if (towerMap[selectedTowerCell.x, selectedTowerCell.y] != selectedTower)
        {
            EndMoveMode();
            return;
        }

        // 타워가 있다면 서로 위치 교환
        if (HasTower(cell))
        {
            Debug.Log("타워가 있음");

            Tower tempTower = towerMap[cell.x, cell.y];
            tempTower.GetComponent<TowerMove>().SetTowerPosition(selectedTowerCell);
            towerMap[cell.x, cell.y] = selectedTower;

            selectedTower.GetComponent<TowerMove>().SetTowerPosition(cell);
            towerMap[selectedTowerCell.x, selectedTowerCell.y] = tempTower;

            EndMoveMode();
            return;
        }

        TowerMove move = selectedTower.gameObject.GetComponent<TowerMove>();
        if (move == null)
        {
            EndMoveMode();
            return;
        }

        move.SetTowerPosition(cell);

        towerMap[cell.x, cell.y] = selectedTower;
        towerMap[selectedTowerCell.x, selectedTowerCell.y] = null;

        EndMoveMode();

        Debug.Log("이동 완료");
    }

    /// <summary>
    /// 선택된 타워 정보 초기화
    /// </summary>
    /// <param name="hideView"></param>
    private void ClearSelectedTower(bool hideView = true)
    {
        if (selectedTower != null)
            selectedTower.ShowAttackRange(false);

        selectedTower = null;
        selectedTowerCell = Vector2Int.zero;
        isGradeUpgradeMode = false;
        isStatUpgradeMode = false;

        // 필요 시 간세 UI도 함께 닫음
        if (hideView)
        {
            towerUICtr.ClearSelection();
        }            
    }

    /// <summary>
    /// 타워 이동 모드 종료로 인한 초기화
    /// </summary>
    /// <param name="clearSelection"></param>
    private void EndMoveMode(bool clearSelection = true)
    {
        isTowerMove = false;
        isGradeUpgradeMode = false;
        isStatUpgradeMode = false;

        if (clearSelection)
            ClearSelectedTower(false);
    }

    /// <summary>
    /// 타우 설치 모드 종료로 인한 초기화
    /// </summary>
    private void EndBuildMode()
    {
        isBuildMode = false;
        selectedTowerUID = string.Empty;
        queIndex = 0;
        tempQueue = null;
    }

    /// <summary>
    /// 마우스 클릭 위치 리턴
    /// </summary>
    /// <returns>마우스 클릭 위치의 그리드 좌표</returns>
    private Vector2Int GetMouseCellPosition()
    {
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        return grid.WorldToCell(mouseWorld);
    }

    /// <summary>
    /// 해당 셀이 시작지점이나 도착지점인지 확인
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool IsBlockedCell(Vector2Int cell)
    {
        return cell == stage.SpawnPos || cell == stage.GoalPos;
    }

    /// <summary>
    /// 타워를 만들기 적확한지 확인
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool CanUseTowerCell(Vector2Int cell)
    {
        if (!grid.IsInBounds(cell))
            return false;

        if (IsBlockedCell(cell))
            return false;

        if (!obstacle.HasObstacle(cell))
            return false;

        return true;
    }

    private bool CanPlaceTower(Vector2Int cell)
    {
        if (!CanUseTowerCell(cell))
            return false;

        if (HasTower(cell))
            return false;

        return true;
    }

    /// <summary>
    /// 마우스가 UI를 클릭하는지 확인
    /// TowerUIRaycastTarget.cs를 소유하고 있는 UI를 클릭시 true 반환
    /// </summary>
    /// <returns></returns>
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        raycastResults.Clear();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach(RaycastResult result in raycastResults)
        {
            if (result.gameObject == null)
                continue;

            if (result.gameObject.GetComponentInParent<TowerUIRaycastTarget>() != null)
                return true;
        }

        return false;
    }

    private bool TryGetUpgradeTowers(out List<Tower> towers, out List<TowerMove> moves)
    {
        int upgradeCnt = needupgradeTowerCnt;
        towers = new List<Tower>();
        moves = new List<TowerMove>();

        if (selectedTower.Grade == 6 || selectedTower.nextGradeUID == "MASTER" || selectedTower.nextGradeUID == "Master")
            return false;

        foreach (var t in fieldTowers)
        {
            if (t.Grade != selectedTower.Grade || t.Type != selectedTower.Type)
                continue;

            TowerMove move = t.GetComponent<TowerMove>();

            if (move == null)
                continue;

            towers.Add(t);
            moves.Add(move);

            if (towers.Count == upgradeCnt)
                break;
        }

        if (towers.Count == upgradeCnt && moves.Count == upgradeCnt)
            return true;

        return false;
    }

    private void TowerGradeUpgrade(string buildUID, List<Tower> towers, List<TowerMove> moves)
    {
        Vector2Int spawnCell = moves[0].GetTowerPosition();

        RemoveTower(towers, moves);
        BuildTower(buildUID, spawnCell);
        ClearSelectedTower();
    }

    private void RemoveTower(List<Tower> rTowers, List<TowerMove> rMoves)
    {
        int cnt = rTowers.Count;
        for (int i = 0; i < cnt; i++)
        {
            fieldTowers.Remove(rTowers[i]);
            Vector2Int cell = rMoves[i].GetTowerPosition();
            towerMap[cell.x, cell.y] = null;
            Destroy(rTowers[i].gameObject);
        }
    }

    /// <summary>
    /// 해당 셀에 타워가 있는지 확인
    /// </summary>
    /// <param name="cell">타워가 있는지 확인이 필요한 셀</param>
    /// <returns>타워 유무</returns>
    public bool HasTower(Vector2Int cell)
    {
        if (!grid.IsInBounds(cell))
            return false;

        return towerMap[cell.x, cell.y] != null;
    }

    public void SetTowerMoveMode()
    {
        isTowerMove = true;
    }

    public void TowerGradeNormalUpgrade()
    {
        int upgradeCnt = needupgradeTowerCnt;

        if (!TryGetUpgradeTowers(out List<Tower> towers, out List<TowerMove> moves))
            return;

        if (towers.Count != upgradeCnt)
            return;

        string[] nextTowerUIDs = Managers.TowerData.GetTowerNextGradeUID(towers[0].Grade);
        int cnt = nextTowerUIDs.Length;
        int idx = Random.Range(0, cnt - 1);

        string buildTowerUID = nextTowerUIDs[idx];

        TowerGradeUpgrade(buildTowerUID, towers, moves);
    }

    public void TowerGradePreminumUpgrade()
    {
        int upgradeCnt = needupgradeTowerCnt;

        if (!TryGetUpgradeTowers(out List<Tower> towers, out List<TowerMove> moves))
            return;

        if (towers.Count != upgradeCnt)
            return;

        string buildTowerUID = towers[0].NextGradeUID;
        TowerGradeUpgrade(buildTowerUID, towers, moves);
    }
}
