using UnityEngine;

public class GridSettings : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float cellSize = 1f;

    [Header("Map Settings")]
    [SerializeField]
    private Transform mapPlane;
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private Transform goalPoint;
    
    void Start()
    {
        Managers.Grid.InitializeGrid(gridWidth, gridHeight, cellSize, mapPlane);
        SetSettings();
    }

    public void SetSettings()
    {
        spawnPoint.position = Managers.Grid.CellToWorldCenter(Managers.Grid.SpawnPos.x, Managers.Grid.SpawnPos.y);
        goalPoint.position = Managers.Grid.CellToWorldCenter(Managers.Grid.GoalPos.x, Managers.Grid.GoalPos.y);

        Managers.Game.AfterSettingsInit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
