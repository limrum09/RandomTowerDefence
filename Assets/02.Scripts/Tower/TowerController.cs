using System;
using System.Collections.Generic;
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

    public event Action<Tower> OnTowerSelected;
    public event Action OnTowerSelectCleared;
    public event Action<Tower> OnShowGradeUpgrade;
    public event Action<Tower> OnShowStatUpgrade;
    public event Action<int> OnGoldInterection;

    private readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    private FieldTowerManager fieldTowerManager;
    // StageManager의 그리드 참조
    private GridManager grid;
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

    // 설치 성공 시, 제거할 대기열 정보
    private QueueController tempQueue;
    private int queIndex;

    private void Start()
    {
        fieldTowerManager = stage.FieldTowerManager;
        grid = stage.Grid;
        selectedTowerUID = string.Empty;
        isBuildMode = false;

        isTowerMove = false;
        isGradeUpgradeMode = false;

        needupgradeTowerCnt = 3;
    }

    /// <summary>
    /// 현재 상태에 따라 타워 이동, 타워 설치, 타워 성택 입력을 처리
    /// </summary>
    void Update()
    {
        // 타워 이동 모드일 시
        if (isTowerMove)
        {
            // 마수르 왼쪽 버튼을 클릭하여 타워 이동 시작
            if (Input.GetMouseButtonDown(0))
                TryMoveTower();

            // 다른 모드와 겹치지 않도록 return으로 끝내기
            return;
        }

        // 타워 생성 모드 일 시
        if (isBuildMode)
        {
            // 마우스 왼쪽 버튼을 클릭하여 타워 생성 시작
            if (Input.GetMouseButtonDown(0))
                TryBuildPendingTower();

            // 다른 모드와 겹치지 않도록 return으로 끝내기
            return;
        }

        // 아무런 모드가 아닐 시, 마우스 왼쪽 버튼을 클릭하여 해당 위치에 타워가 있는지 확인
        if (Input.GetMouseButtonDown(0))
        {
            ClickTower();
        }

        // 선택된 타워가 있을 시
        if (selectedTower != null)
        {
            // 타워 이동 단축키 입력
            if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.MoveTower)))
            {
                // 다음 클릭으로 타워 이동 모드 진입
                SetTowerMoveMode();
                // 상세 UI 닫기
                OnTowerSelectCleared?.Invoke();
                return;
            }

            // 타워 등급 업그레이드 뷰 단축키 입력
            if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.ShowGradeUpgradeTowerView)))
            {
                // 등급 업그레이드를 입력 받을 수 있도록 상태 변환
                isGradeUpgradeMode = true;
                // 선택한 타워의 업그레이드 UI 실행
                OnShowGradeUpgrade?.Invoke(selectedTower);
                return;
            }

            // 세션 스탯 업그레이드 단축키 입력
            if (Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.ShowStatUpgradeTowerView)))
            {
                // 현제 선택한 타워의 세션 업그레이드 UI 실행
                OnShowStatUpgrade?.Invoke(selectedTower);
                return;
            }

            // 등급 업그레이드 모드이고, 노말 업그레이드 단축키 실행
            if(isGradeUpgradeMode && Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.TowerGradeNormalUpgrade)))
            {
                // 등급 입력 처리 후, 모드 해체
                isGradeUpgradeMode = false;
                // 등급 업그레이드
                TowerGradeNormalUpgrade();
            }

            // 등급 업그레이드 모드이고, 프리미엄 업그레이드 단축키 실행
            if (isGradeUpgradeMode && Input.GetKeyDown(Managers.InputData.GetKeyCode(InputAction.TowerGradePremiunUpgrade)))
            {
                // 등급 입력 처리 후, 모드 해체
                isGradeUpgradeMode = false;
                // 프리미엄 등급 업그레이드
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
        // 클릭한 마우스의 셀 값 가져오기
        Vector2Int cell = GetMouseCellPosition();
        // 타워를 생성하고 결과를 bool값으로 가져오기
        bool success = BuildTower(selectedTowerUID, cell);

        // 타워 생성 실패
        if (!success)
        {
            // 생성 모드 종료
            EndBuildMode();
            return;
        }

        // 현제 타워의 정보를 관리하는 Queue에서 타워 index정보 삭제
        tempQueue.RemoveTower(queIndex);
        // 생성 모드 종료
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
        // 타워를 cell의 위치에 생성할 수 있는지 확인
        if (!CanPlaceTower(cell))
            return false;

        // 선택 타워가 있다면 타워 선택 초기화
        ClearSelectedTower();

        // 해당 cell의 좌표값을 월드 좌표로 변환
        Vector3 worldPos = grid.CellToWorldCenter(cell.x, cell.y);
        // 타워 Prefab을 생성, towers오브젝트를 부모로 지정해서 Hierachy정리
        GameObject towerObj = Instantiate(towerPre, worldPos, Quaternion.identity, towers.transform);

        Tower tower = towerObj.GetComponent<Tower>();

        // Tower 컴포넌트가 없으면 잘못된 Prefab 이기에 삭제 후, 실패 반환
        if (tower == null)
        {
            Destroy(towerObj);
            return false;
        }

        // 타워 데이터 초기화
        // 타워 데이터, 현제 타워에 지급된 Index값, 세션 스텟 manager 넘겨주기
        tower.Init(towerUID, fieldTowerManager.GetTotalTowerCount(), stage.StatUpgradeManager);

        TowerMove towerMove = towerObj.GetComponent<TowerMove>();
        // TowerMove가 없을 시 컴포넌트 추가
        if (towerMove == null)
            towerMove = towerObj.AddComponent<TowerMove>();

        // 생성되는 Tower에게 현제 사용중인 StageManager넘겨주기
        towerMove.SetTowerInit(stage);
        // cell의 위치로 타워 이동
        towerMove.SetTowerPosition(cell);

        // FieldTower Manager에 타워 등록
        // 셀 위치와 Tower 정보를 매핑
        bool registerd = fieldTowerManager.RegisterTower(tower, cell);

        // 타워 등록 실패시
        if (!registerd)
        {
            // 오브젝트 파괴 이후 실패 리턴
            Destroy(towerObj);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 클릭한 오브젝트가 타워인지 판별하고, 선택된 타워 및 상세 UI를 갱신
    /// </summary>
    private void ClickTower()
    {
        // 클릭한 정보가 TowerUIRaycastTarget을 가지고 있으면 종료
        // 즉, TowerUIRaycastTarget을 가지고 있다면 정보를 안가져옴
        if (Managers.InputData.IsPointerOverUI<TowerUIRaycastTarget>())
            return;

        // 클릭한 위치에 타워가 없다면
        if (!Managers.InputData.TryGetMouseComponent(mainCamera, out Tower tower))
        {
            // 클릭 초기화
            ClearSelectedTower();
            return;
        }

        // 타워가 없다면
        if (tower == null)
        {
            // 클릭 값 초기화
            ClearSelectedTower();
            return;
        }

        // TowerUID를 넘겨서 TowerData가져오기
        TowerData data = Managers.TowerData.GetTowerData(tower.TowerUID);
        // data가 없다는 것은, Json으로 가져오지 못했거나, UID가 않맞거나.
        // 클릭한 곳에 tower가 없더나.
        if (data == null)
        {
            // 상세 UI 닫기
            ClearSelectedTower(false);
            return;
        }

        // TowerMove 컴포넌트 가져오기
        TowerMove move = tower.GetComponent<TowerMove>();
        if (move == null)
        {
            // 컴포넌트가 없다면 상세 UI닫기
            ClearSelectedTower(false);
            return;
        }

        // 타워 상세 UI 닫기
        ClearSelectedTower(false);

        // 새타워 선택
        selectedTower = tower;
        // 공격 범위 보여주기
        selectedTower.ShowAttackRange(true);
        // 현제 Cell값 저장
        selectedTowerCell = move.GetTowerPosition();

        // 현재 선택한 타워 기준으로 상세 UI보여주기
        OnTowerSelected?.Invoke(selectedTower);
    }

    /// <summary>
    /// 선택된 타워를 클릭한 셀로 이동
    /// 대상 셀에 다른 타워가 있으면 서로 위치를 교환
    /// </summary>
    private void TryMoveTower()
    {
        // 선택된 타워가 없다면 종료
        if (selectedTower == null)
        {
            EndMoveMode();
            return;
        }

        // 현재 마우스가 선택한 셀 값 가져오기
        Vector2Int cell = GetMouseCellPosition();

        // 현제 선택한 셀에 타워 설치 불가
        if (!CanUseTowerCell(cell))
        {
            EndMoveMode();
            return;
        }

        // 필드매니저가 가지고 있는 선택한 위치의 타워와 선택한 타워가 같은지 확인
        if (fieldTowerManager.GetTower(selectedTowerCell) != selectedTower)
        {
            // 틀리면 종료
            EndMoveMode();
            return;
        }

        // 타워가 있다면 서로 위치 교환
        if (fieldTowerManager.HasTower(cell))
        {
            // 필드 매니저에서 선택한 셀의 타워 가져오기
            Tower tempTower = fieldTowerManager.GetTower(cell);
            if(tempTower == null)
            {
                EndMoveMode();
                return;
            }

            TowerMove tempMove = tempTower.GetComponent<TowerMove>();
            TowerMove selectedMove = selectedTower.GetComponent<TowerMove>();

            // 컴포넌트가 둘중 하나라도 없다면 종료
            if (tempMove == null || selectedMove == null)
            {
                EndMoveMode();
                return;
            }

            // 위치 이동
            tempMove.SetTowerPosition(selectedTowerCell);
            selectedMove.SetTowerPosition(cell);

            fieldTowerManager.SwapTower(selectedTowerCell, cell);
            EndMoveMode();

            return;
        }

        TowerMove move = selectedTower.gameObject.GetComponent<TowerMove>();
        if (move == null)
        {
            EndMoveMode();
            return;
        }

        // 위치이동
        move.SetTowerPosition(cell);
        // 갱신
        fieldTowerManager.MoveTower(selectedTower, selectedTowerCell, cell);

        EndMoveMode();
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

        // 필요 시 간세 UI도 함께 닫음
        if (hideView)
        {
            OnTowerSelectCleared?.Invoke();
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
        return Managers.InputData.GetMouseCellPosition(mainCamera, grid);
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
        // 그리드 범위 밖으면 사용 불가
        if (!grid.IsInBounds(cell))
            return false;

        // 시작 지점 또는 골인 지점일 시 사용 불가
        if (IsBlockedCell(cell))
            return false;

        // 장애물이 없는 셀에는 타워를 설치/이동 불가능
        if (!obstacle.HasObstacle(cell))
            return false;

        return true;
    }

    /// <summary>
    /// 해당 셀에 타워 설치가 가능한지 확인
    /// 해당 조건을 확인
    /// 맵 범위, 시작/도착 지점이 아니고, 장애물이 설치된 셀, 이미 타워가 없어야 함
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    private bool CanPlaceTower(Vector2Int cell)
    {
        // 기본 설치 가능 조건 ㄱ머사
        if (!CanUseTowerCell(cell))
            return false;

        // 이미 타워가 있다면 설치 불가
        if (HasTower(cell))
            return false;

        return true;
    }

    /// <summary>
    /// 타워 등급 업그레이드 처리
    /// 재료 타워 제거 후, 업그레이드된 타워 생성
    /// </summary>
    /// <param name="buildUID"></param>
    /// <param name="towers"></param>
    private void TowerGradeUpgrade(string buildUID, List<Tower> towers)
    {
        // 첫 번째 재료 타워의 위치를 업그레이드 타워 생성 위치로 사용
        Vector2Int spawnCell = towers[0].GetComponent<TowerMove>().GetTowerPosition();

        // 재료 타워 제거
        fieldTowerManager.RemoveTowers(towers);

        // 업그레이드 타워 생성
        BuildTower(buildUID, spawnCell);

        // 현재 선택 상태 초기화
        ClearSelectedTower();
    }

    /// <summary>
    /// 골드 변경 이벤트 전달
    /// 양수는 골드 획득, 음수는 골드 소비
    /// </summary>
    /// <param name="value"></param>
    private void GoldInterction(int value)
    {
        OnGoldInterection?.Invoke(value);
    }

    /// <summary>
    /// 해당 셀에 타워가 있는지 확인
    /// </summary>
    /// <param name="cell">타워가 있는지 확인이 필요한 셀</param>
    /// <returns>타워 유무</returns>
    public bool HasTower(Vector2Int cell)
    {
        return fieldTowerManager != null && fieldTowerManager.HasTower(cell);
    }

    /// <summary>
    /// 타워 이동 모드 활성화
    /// 타워 클릭 시 타워 이동 또는 위치 교환 수행
    /// </summary>
    public void SetTowerMoveMode()
    {
        isTowerMove = true;
    }

    /// <summary>
    /// 일반 등급 업그레이드
    /// 같은 등급의 타워들을 재료로 사용하여 랜덤한 상위 등급 타워를 생성
    /// </summary>
    public void TowerGradeNormalUpgrade()
    {
        int upgradeCnt = needupgradeTowerCnt;

        // 업그레이드 가능항 재료 타워 탐색
        if (!fieldTowerManager.TryGetGradeUpgradeTower(selectedTower, upgradeCnt, out List<Tower> towers))
            return;

        // 재료타워 개수 부족시 실패
        if (towers.Count != upgradeCnt)
            return;

        // 다음 등급 타워의 UID 가져오기
        string[] nextTowerUIDs = Managers.TowerData.GetTowerGradeUID(towers[0].Grade + 1);
        int cnt = nextTowerUIDs.Length;

        // 랜덤 업그레이드 결과 선택
        int idx = UnityEngine.Random.Range(0, cnt);
        
        string buildTowerUID = nextTowerUIDs[idx];

        // 업그레이드 실행
        TowerGradeUpgrade(buildTowerUID, towers);
    }

    /// <summary>
    /// 프리미엄 등급 업그레이드
    /// </summary>
    public void TowerGradePreminumUpgrade()
    {
        int upgradeCnt = needupgradeTowerCnt;

        // 업그레이드 가능한 재료 타워 탐색
        if (!fieldTowerManager.TryGetGradeUpgradeTower(selectedTower, upgradeCnt, out List<Tower> towers))
            return;

        // 재료 개수가 부족하면 실패
        if (towers.Count != upgradeCnt)
            return;

        // 고정 업그레이드 타워 UID 가져오기
        string buildTowerUID = towers[0].NextGradeUID;

        // 업그레이드 실행
        TowerGradeUpgrade(buildTowerUID, towers);
    }

    /// <summary>
    /// 현재 선택된 타워 판매
    /// 판매 성공 시 골드 지급
    /// 선택 상태 초기화
    /// </summary>
    public void RemoveTower()
    {
        // 판매 가격 가져오기
        int price = selectedTower.SellPrice;
        // 타워 제거 성공 시
        if (fieldTowerManager.RemoveTower(selectedTower))
        {
            // 골드지급
            GoldInterction(price);
            // 선택 초기화
            ClearSelectedTower();
        }
    }
}
