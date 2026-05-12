using DG.Tweening.Core.Easing;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemSlotUIController : MonoBehaviour
{
    [SerializeField]
    private List<ItemSlotUI> itemSlots = new List<ItemSlotUI>();

    private ItemData[] itemUId;
    private int len;

    public event Action<ItemData, int> OnClickItem;
    public event Action<ItemData> OnItemAdd;
    public event Action<int> OnRequestSellItem;

    private void Awake()
    {
        len = itemSlots.Count;
        itemUId = new ItemData[len];
        for(int i = 0; i < len; i++)
        {
            itemSlots[i].Init(this, i);
            itemUId[i] = null;
            ClearSlot(i);
        }
    }

    private void ClearSlot(int index)
    {
        itemUId[index] = null;
        itemSlots[index].RemoveSlotUI();
        itemSlots[index].gameObject.SetActive(false);
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < len;
    }

    public bool AddItemSlot(string uid)
    {
        ItemData item = Managers.Item.GetItemData(uid);

        if (item == null)
            return false;

        if (itemUId.Contains(item))
            return false;

        for(int i = 0; i < len; i++)
        {
            if (itemSlots[i].IsSlotEmpty)
            {
                itemUId[i] = item;

                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].SetSlotUI(uid);

                OnItemAdd?.Invoke(item);
                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(int index)
    {
        if(!IsValidIndex(index)) 
            return false;

        if (itemUId[index] == null)
            return false;

        ClearSlot(index);
        return true;
    }

    public ItemData GetItem(int index)
    {
        if (!IsValidIndex(index))
            return null;

        return itemUId[index];
    }

    public void OnSelectItem(int index, string uid)
    {
        if (!IsValidIndex(index))
            return;

        if (string.IsNullOrEmpty(uid))
            return;

        ItemData item = Managers.Item.GetItemData(uid);

        if(item == null) 
            return;

        if (item.itemUID != uid)
            return;

        OnClickItem?.Invoke(item, index);
    }

    public void RequestSellItem(int index)
    {
        if (!IsValidIndex(index))
            return;

        if(itemUId[index] == null) 
            return;

        OnRequestSellItem?.Invoke(index);
    }
}
