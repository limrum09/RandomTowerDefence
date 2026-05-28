using UnityEngine;

public class TowerBuildPreview : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer previewRenderer;
    [SerializeField]
    private SpriteRenderer cellHighlight;

    [SerializeField]
    private Color canPlaceColor = new Color(0f, 1f, 0f, 0.4f);
    [SerializeField]
    private Color cannotPlaceColor = new Color(1f, 0f, 0f, 0.4f);
    
    public void Show(Sprite sprite)
    {
        previewRenderer.sprite = sprite;
        previewRenderer.gameObject.SetActive(true);
        cellHighlight.gameObject.SetActive(true);
    }

    public void Hide()
    {
        previewRenderer.sprite = null;
        previewRenderer.gameObject.SetActive(false);
        cellHighlight.gameObject.SetActive(false);
    }

    public void UpdatePreview(Vector3 worldPos, Vector3 cellCenter, bool canPlace)
    {
        previewRenderer.transform.position = worldPos;
        cellHighlight.transform.position = cellCenter;

        Color color = canPlace ? canPlaceColor : cannotPlaceColor;

        previewRenderer.color = new Color(color.r, color.g, color.b, 0.6f);
        cellHighlight.color = color;
    }
}
