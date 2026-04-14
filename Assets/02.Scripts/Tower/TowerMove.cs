using UnityEngine;

public class TowerMove : MonoBehaviour
{
    private GridManager grid;

    public void SetTowerInit(StageManager getStage)
    {
        grid = getStage.Grid;
    }
    public void SetTowerPosition(Vector2Int pos)
    {
        transform.position = grid.CellToWorldCenter(pos.x, pos.y);
    }
}
