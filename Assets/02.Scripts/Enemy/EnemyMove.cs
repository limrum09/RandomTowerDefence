using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private StageManager stage;
    private GridManager gridManager;
    private PathFinder path;
    private float moveSpeed;

    [Header("Path Test")]
    [SerializeField]
    private Vector2Int startCell;
    [SerializeField]
    private Vector2Int endCell;

    private List<GridNode> currentPath;
    private int pathIndex;
    private bool isMove;

    private void Update()
    {
        if (!isMove)
            return;

        MoveAlongPath();
    }

    public void Initialize(StageManager getStage, Vector2Int getStartCell, Vector2Int getEndCell, float getMoveSpeed)
    {
        stage = getStage;
        gridManager = stage.Grid;
        path = stage.Path;

        startCell = getStartCell;
        endCell = getEndCell;
        moveSpeed = getMoveSpeed;
        isMove = true;

        transform.position = gridManager.CellToWorldCenter(startCell.x, startCell.y);
        RecalculatePath();
    }

    public void IsDead()
    {
        isMove = false;
    }

    private void RecalculatePath()
    {
        Vector2Int currentCell = gridManager.WorldToCell(transform.position);
        currentPath = path.FindPath(currentCell, endCell);
        pathIndex = 0;

        if (currentPath == null || currentPath.Count == 0)
        {
            Debug.LogWarning("경로를 찾지 못했습니다.");
            return;
        }

        Debug.Log($"경로 길이 : {currentPath.Count}");
    }

    private void MoveAlongPath()
    {
        if (currentPath == null || currentPath.Count == 0)
            return;

        if (pathIndex >= currentPath.Count)
            return;

        GridNode targetNode = currentPath[pathIndex];
        Vector3 targetPos = gridManager.CellToWorldCenter(targetNode.x, targetNode.y);

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.05f)
        {
            pathIndex++;

            if (pathIndex >= currentPath.Count)
            {
                Destroy(gameObject);
                Debug.Log("적이 목표 지점에 도착했습니다.");
            }
        }
    }
    private void OnDestroy()
    {
        stage.RegisterReachedEnemy();
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
