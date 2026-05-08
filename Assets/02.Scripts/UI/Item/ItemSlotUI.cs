using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField]
    private Image itemIcon;
    [SerializeField]
    private TextMeshProUGUI itemNameText;

    private ItemSlotUIController owner;
    private string itemUID;
    private int index;

    public bool IsSlotEmpty => string.IsNullOrEmpty(itemUID);
    public string ItemUID => itemUID;
    private bool RefreshUI()
    {
        if (!IsSlotEmpty)
        {
            ItemData data = Managers.Item.GetItemData(itemUID);

            if(data == null)
                return false;

            Sprite icon = Resources.Load<Sprite>($"Item/Images/{data.iconUID}");

            if (icon != null)
                itemIcon.sprite = icon;

            itemNameText.text = Managers.Local.GetString(data.stringKey);
        }
        else
        {
            itemNameText.text = "";
            itemIcon.sprite = null;
        }

        return true;
    }

    public void Init(ItemSlotUIController ctr, int getIndex)
    {
        itemUID = string.Empty;
        owner = ctr;
        index = getIndex;
        RefreshUI();
    }

    public void SetSlotUI(string uid)
    {
        itemUID = uid;
        RefreshUI();
    }

    public bool RemoveSlotUI()
    {
        itemUID = string.Empty;
        return RefreshUI();
    }

    public void OnClickItem()
    {
        if (owner == null)
            return;

        owner.OnSelectItem(index, itemUID);
    }
}
