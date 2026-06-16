using TMPro;
using UnityEngine;

public class StoreToolTip : MonoBehaviour
{
    [Header("Component")]
    [SerializeField]
    private CanvasGroup canvas;

    [Header("Rects")]
    [SerializeField]
    private RectTransform tooltipLayer;
    [SerializeField]
    private RectTransform tooltipRect;
    [SerializeField]
    private RectTransform infoTextRect;
    [SerializeField]
    private Vector2 offset = new Vector2(40f, 40f);

    [Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI gradeText;
    [SerializeField]
    private TextMeshProUGUI infoText;

    private void Start()
    {
        Hide();
    }

    private void SetPosition(RectTransform slotRect)
    {
        Vector3[] corner = new Vector3[4];
        slotRect.GetWorldCorners(corner);

        Vector2 screePos = RectTransformUtility.WorldToScreenPoint(null, corner[2]);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipLayer, screePos, null, out Vector2 localPos);

        tooltipRect.anchoredPosition = localPos + offset;
    }

    private void SetTexts(ItemData data)
    {
        nameText.text = Managers.Local.GetString("Item", data.stringKey);
        gradeText.text = Managers.Local.GetString("Item", "ITEM_GRADE_" + data.grade.ToString().ToUpper());
        infoText.text = Managers.Local.GetString("Item", data.itemDesc);

        tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, 140f);
        infoTextRect.sizeDelta = new Vector2(infoTextRect.sizeDelta.x, 85f);
    }

    private void SetTexts(TowerData data)
    {
        nameText.text = Managers.Local.GetString(data.stringKey);

        string gradeStr = string.Format(Managers.Local.GetString("UI","UI_GRADE"), data.grade);
        gradeText.text = gradeStr;

        TowerStatPreview stat = TowerStatCalculator.Calculate(data);

        string info = $"{Managers.Local.GetString("UI", "UI_ATK_DAMAGE")} - {stat.damage}" +
            $"\n{Managers.Local.GetString("UI", "UI_ATK_SPEED")} - {stat.attackSpeed.ToString("N2")}";

        infoText.text = info;

        tooltipRect.sizeDelta = new Vector2(tooltipRect.sizeDelta.x, 100f);
        infoTextRect.sizeDelta = new Vector2(infoTextRect.sizeDelta.x, 45f);
    }

    public void Show(StoreProduct product, RectTransform slotRect)
    {
        canvas.alpha = 1.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        if (product.type == StoreProductType.Tower)
        {
            TowerData data = Managers.TowerData.GetTowerData(product.uid);
            SetTexts(data);
        }
        else if(product.type == StoreProductType.Item)
        {
            ItemData data = Managers.Item.GetItemData(product.uid);
            SetTexts(data);
        }

            SetPosition(slotRect);
    }

    public void Hide()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
}
