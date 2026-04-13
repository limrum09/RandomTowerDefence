using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    private GridManager gridManager;
    [SerializeField]
    private PathFinder path;
    [SerializeField]
    private float moveSpeed = 3f;

    [Header("Path Test")]
    [SerializeField]
    private Vector2Int startCell;
    [SerializeField]
    private Vector2Int endCell;

    private List<GridNode> currentPath;
    private int pathIndex;

    private void Update()
    {
        MoveAlongPath();
    }

    private void Start()
    {
        gridManager = Managers.Grid;
        transform.position = gridManager.CellToWorldCenter(startCell.x, startCell.y);
        RecalculatePath();
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
