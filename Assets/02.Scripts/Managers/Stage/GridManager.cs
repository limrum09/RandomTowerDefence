using UnityEngine;

public class GridManager
{
    private int gridWidth;
    private int gridHeight;
    private float cellSize;
    private Transform mapPlane;
    private GridNode[,] grid;
    private float mapWidth;
    private float mapHeight;
    private Vector3 mapOrigin;
    private Vector2Int spawnPos;
    private Vector2Int goalPos;

    public int GridWidth => gridWidth;
    public int GridHeight => gridHeight;
    public float CellSize => cellSize;
    public Vector3 MapOrigin => mapOrigin;
    public Vector2Int SpawnPos => spawnPos;
    public Vector2Int GoalPos => goalPos;

    public event System.Action OnSetSpawnAndGoalPoint;

    private void SetSpawnPointAndGoalPoint()
    {
        spawnPos = new Vector2Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight));
        goalPos = new Vector2Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight));

        int maxLoop = 0;
        while (maxLoop < 100)
        {
            if (Vector2.Distance(spawnPos, goalPos) >= 2)
                break;

            goalPos = new Vector2Int(Random.Range(0, gridWidth), Random.Range(0, gridHeight));

            maxLoop++;
        }

        Debug.Log($"Grid Manager, Spawn Cell Pos : {spawnPos}, Goal Cell Pos : {goalPos}");
    }

    public void InitializeGrid(int getGridWidth, int getGridHeight, float getCellSize, Transform getMapPlane)
    {
        gridWidth = getGridWidth;
        gridHeight = getGridHeight;
        cellSize = getCellSize;
        mapPlane = getMapPlane;

        mapWidth = gridWidth * cellSize;
        mapHeight = gridHeight * cellSize;

        mapOrigin = new Vector3(mapPlane.position.x - mapWidth * 0.5f, mapPlane.position.y - mapHeight * 0.5f, 0f);        

        CreateGrid();
        SetSpawnPointAndGoalPoint();
    }

    public void CreateGrid()
    {
        grid = new GridNode[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = new GridNode(x, y, false);
            }
        }

        Debug.Log($"2D Grid 생성 완료 : {gridWidth} X {gridHeight}, Origin : {mapOrigin}");
    }

    public GridNode GetNode(int x, int y)
    {
        if (x < 0 || y < 0 || x >= gridWidth || y >= gridHeight)
            return null;

        return grid[x, y];
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt((worldPos.x - mapOrigin.x) / cellSize);
        int y = Mathf.FloorToInt((worldPos.y - mapOrigin.y) / cellSize);

        return new Vector2Int(x, y);

    }
    public Vector3 CellToWorldCenter(int x, int y)
    {
        return new Vector3(mapOrigin.x + x * cellSize + cellSize * 0.5f, mapOrigin.y + y * cellSize + cellSize * 0.5f, 0f);
    }

    public bool IsInBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < gridWidth && cell.y >= 0 && cell.y < gridHeight;
    }

    public bool SetBlocked(int x, int y, bool blocked)
    {
        GridNode node = GetNode(x, y);
        if (node == null)
            return false;

        node.isBlocked = blocked;
        return true;
    }

    public void SetRerollPoints()
    {
        SetSpawnPointAndGoalPoint();
        OnSetSpawnAndGoalPoint?.Invoke();
    }
}
