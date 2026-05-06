using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 적의 이동을 담당하는 클래스
/// PathFinder를 이용한 경로 이동
/// 목표 지점 도착 처리
/// 사망 처리
/// 이동 방향에 따른 Sprite 좌우 반전
/// </summary>
public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    private Enemy enemy;                    // 이동 속도 등의 데이터를 가진 Enemy 참조
    [SerializeField]
    private SpriteRenderer spriteRenderer;  // 좌, 우 방향 표시용 

    private GridManager gridManager;        // Grid 정보 참조
    private PathFinder path;                // 경로 탐색기

    [Header("Path Test")]
    [SerializeField]
    private Vector2Int startCell;           // 시작 셀 위치
    [SerializeField]
    private Vector2Int endCell;             // 목표 셀 위치

    private List<GridNode> currentPath;     // 현제 이동 중인 경로
    private int pathIndex;                  // 현제 이동 중인 경로 인덱스
    private bool isMove;                    // 이동 가능 판단

    // 적 사망 이벤트
    // 사망시 골드 지급 등
    public event Action<int> onDead;
    // 목표지점 도착 이벤트
    // 도착시 삭제되고, 플레이어 목숨 감소 등
    public event Action onReachGoal;

    private void Update()
    {
        // 이동 상태가 아니면 종료
        if (!isMove)
            return;

        // 실시간 이동 계산
        MoveAlongPath();
    }

    /// <summary>
    /// 초기화
    /// </summary>
    /// <param name="getGrid"></param>
    /// <param name="getPath"></param>
    /// <param name="getEnemy"></param>
    /// <param name="getStartCell"></param>
    /// <param name="getEndCell"></param>
    public void Initialize(GridManager getGrid, PathFinder getPath, Enemy getEnemy, Vector2Int getStartCell, Vector2Int getEndCell)
    {
        gridManager = getGrid;
        path = getPath;
        enemy = getEnemy;

        startCell = getStartCell;
        endCell = getEndCell;
        isMove = true;

        // 시작 위치로 이동
        transform.position = gridManager.CellToWorldCenter(startCell.x, startCell.y);
        // 이동 경로 계산
        RecalculatePath();
    }

    /// <summary>
    /// 적 사망
    /// </summary>
    /// <param name="rewardGold"></param>
    public void IsDead(int rewardGold)
    {
        if (!isMove)
            return;

        isMove = false;
        // 사망 이벤트 호출
        onDead?.Invoke(rewardGold);
    }

    /// <summary>
    /// 현제 위치 기준으로 경로 계산
    /// </summary>
    private void RecalculatePath()
    {
        // 현재 위치를 Grid Cell로 변환
        Vector2Int currentCell = gridManager.WorldToCell(transform.position);
        // 현제 위치 부터 목표 지점까지 경로 탐색
        currentPath = path.FindPath(currentCell, endCell);
        // 경로 시작 인덱스 초기화
        pathIndex = 0;

        // 경로 탐색 실패
        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning("경로를 찾지 못했습니다.");
            return;
        }

        Debug.Log($"경로 길이 : {currentPath.Count}");
    }

    /// <summary>
    /// 경로를 따라 이동
    /// </summary>
    private void MoveAlongPath()
    {
        // 경로가 없으면 종료
        if (currentPath == null || currentPath.Count == 0)
            return;

        // 경로 끝까지 이동하면 종료
        if (pathIndex >= currentPath.Count)
            return;

        // 현제 목표 노드
        GridNode targetNode = currentPath[pathIndex];
        // 목표 노드 월드 위치
        Vector3 targetPos = gridManager.CellToWorldCenter(targetNode.x, targetNode.y);
        // 현제 이동 경로 방향 게산
        Vector3 dir = targetPos - transform.position;

        // SpriteRenderer가 있는지 확인
        if(spriteRenderer == null)
        {
            // 없으면 자식에게 가져오기
            spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

            if (spriteRenderer == null)
                Debug.Log("Sprite Renderer를 찾을 수 없음.");
        }

        // 방향에 따라 바라보는 위치 지정
        if (dir.x > 0.01f)
        {
            // 오른쪽
            spriteRenderer.flipX = false;
        }
        else if (dir.x < -0.01f)
        {
            // 왼쪽
            spriteRenderer.flipX = true;
        }

        // 목표 위치로 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPos, enemy.MoveSpeed * Time.deltaTime * 3.0f);

        // 목표 위치에 거의 도착했는지 확인
        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            // 거의 도착했다면 다음 노드로 이동
            pathIndex++;

            // 경로 끝까지 도착했으면 목표 지점 도달 처리
            if (pathIndex >= currentPath.Count)
            {
                onReachGoal?.Invoke();
                Destroy(gameObject);
                Debug.Log("적이 목표 지점에 도착했습니다.");
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (currentPath == null || gridManager == null)
            return;

        Gizmos.color = Color.red;

        for (int i = 0; i < currentPath.Count; i++)
        {
            Vector3 pos = gridManager.CellToWorldCenter(currentPath[i].x, currentPath[i].y);
            Gizmos.DrawSphere(pos, 0.15f);

            if (i < currentPath.Count - 1)
            {
                Vector3 nextPos = gridManager.CellToWorldCenter(currentPath[i + 1].x, currentPath[i + 1].y);
                Gizmos.DrawLine(pos, nextPos);
            }
        }
    }
}
