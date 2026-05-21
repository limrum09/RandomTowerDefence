using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemInfoView : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI itemNameText;
    [SerializeField]
    private TextMeshProUGUI itemGradeText;
    [SerializeField]
    private TextMeshProUGUI itemTargetText;
    [SerializeField]
    private TextMeshProUGUI itemScopeText;
    [SerializeField]
    private TextMeshProUGUI itemDescriptionText;
    [SerializeField]
    private TextMeshProUGUI itemPriceText;
    [SerializeField]
    private Button itemSellBtn;

    private void Clear()
    {
        itemNameText.text = string.Empty;
        itemGradeText.text = string.Empty;
        itemTargetText.text = string.Empty; 
        itemScopeText.text = string.Empty; 
        itemDescriptionText.text = string.Empty;
        itemPriceText.text = string.Empty;
    }

    public void Hide()
    {
        Clear();
        this.gameObject.SetActive(false);
    }

    public void Show()
    {
        this.gameObject.SetActive(true);
    }

    public void SetIcon(Sprite getIcon) => icon.sprite = getIcon;
    public void SetItemName(string name) => itemNameText.text = name;
    public void SetItemGrade(ItemGrade grade) => itemGradeText.text = grade.ToString();
    public void SetItemTarget(ItemTarget target) => itemTargetText.text = target.ToString();
    public void SetItemScope(ScopeRange scopRange) => itemScopeText.text = scopRange.ToString();
    public void SetItemDes(string des) => itemDescriptionText.text = des;
    public void SetItemPrice(int price) => itemPriceText.text = price.ToString();
    public void BindItemSell(UnityAction action) => itemSellBtn.onClick.AddListener(action);
}
