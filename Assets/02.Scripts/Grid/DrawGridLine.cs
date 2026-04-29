using System.Net.NetworkInformation;
using UnityEngine;

public class DrawGridLine : MonoBehaviour
{
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Material lineMaterial;

    private float lineWidth = 0.15f;
    private Color lineColor = Color.black;

    private int width;
    private int height;
    private float cellSize;
    private Vector3 map;
    public void Init(int getWidth, int getHeight, float getCellSize, Vector3 mapVector)
    {
        ClearLine();

        width = getWidth;
        height = getHeight;
        cellSize = getCellSize;
        map = mapVector;

        for(int i = 0; i <= height; i++)
        {
            Vector3 from = map + new Vector3(width, (i * cellSize) - height, 0f);
            Vector3 to = map + new Vector3(-width, (i * cellSize) - height, 0f);
            CreateLine(from, to, $"Vertical_{i}");
        }

        for(int i = 0; i <= width; i++)
        {
            Vector3 from = new Vector3((i * cellSize) - width, height, i);
            Vector3 to = new Vector3((i * cellSize) - width, -height, i);
            CreateLine(from, to, $"Horizontal_{i}");
        }
    }

    private void CreateLine(Vector3 from, Vector3 to, string lineName)
    {
        GameObject lineObj = new GameObject(lineName);
        lineObj.transform.SetParent(parent);

        LineRenderer line = lineObj.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.useWorldSpace = true;

        line.SetPosition(0, from);
        line.SetPosition(1, to);

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.material = lineMaterial;
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.sortingOrder = 6;
    }

    private void ClearLine()
    {
        for(int i = parent.childCount - 1; i >= 0; i--)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
