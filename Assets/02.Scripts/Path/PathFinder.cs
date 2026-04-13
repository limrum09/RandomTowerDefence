using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class PathFinder : MonoBehaviour
{
    [SerializeField]
    private GridManager gridManager;

    private readonly Vector2Int[] directions =
    {
        new Vector2Int(0, 1),
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0)
    };

    private void Start()
    {
        gridManager = Managers.Grid;
    }

    public List<GridNode> FindPath(Vector2Int startCell, Vector2Int goalCell)
    {
        GridNode startNode = gridManager.GetNode(startCell.x, startCell.y);
        GridNode goalNode = gridManager.GetNode(goalCell.x, goalCell.y);

        if (startNode == null || goalNode == null)
            return null;

        if (startNode.isBlocked || goalNode.isBlocked)
            return null;

        ResetNode();

        List<GridNode> openList = new List<GridNode>();
        HashSet<GridNode> closedSet = new HashSet<GridNode>();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, goalNode);
        startNode.parent = null;

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            GridNode currentNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].hCost < currentNode.hCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == goalNode)
            {
                return RetracePath(startNode, goalNode);
            }

            foreach (Vector2Int dir in directions)
            {
                int nextX = currentNode.x + dir.x;
                int nextY = currentNode.y + dir.y;

                GridNode neighbor = gridManager.GetNode(nextX, nextY);
                if (neighbor == null || neighbor.isBlocked || closedSet.Contains(neighbor))
                    continue;

                int newCost = currentNode.gCost + 10;

                if (newCost < neighbor.gCost || !openList.Contains(neighbor))
                {
                    neighbor.gCost = newCost;
                    neighbor.hCost = GetDistance(neighbor, goalNode);
                    neighbor.parent = currentNode;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
        }

        return null;
    }

    private List<GridNode> RetracePath(GridNode startNode, GridNode goalNode)
    {
        List<GridNode> path = new List<GridNode>();
        GridNode currentNode = goalNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    private int GetDistance(GridNode a, GridNode b) => (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y)) * 10;

    private void ResetNode()
    {
        for (int x = 0; x < gridManager.GridWidth; x++)
        {
            for (int y = 0; y < gridManager.GridHeight; y++)
            {
                GridNode node = gridManager.GetNode(x, y);
                node.gCost = int.MaxValue;
                node.hCost = 0;
                node.parent = null;
            }
        }
    }
}
