using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode
{
    public int x;
    public int y;
    public bool isBlocked;

    public int gCost;
    public int hCost;
    public int FCost => gCost + hCost;

    public GridNode parent;

    public GridNode(int x, int y, bool isBlocked)
    {
        this.x = x;
        this.y = y;
        this.isBlocked = isBlocked;
    }

    public Vector3 WorldPosition(float cellSize)
    {
        return new Vector3(x * cellSize, y * cellSize, 0f);
    }
}
