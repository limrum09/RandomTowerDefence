using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField]
    private List<ItemSlotUI> itemSlots = new List<ItemSlotUI>();

    private ItemData[] itemUId;
    private int len;

    public event Action<ItemData, int> OnClickItem;
    public event Action<ItemData> OnItemAdd;
    public event Action<ItemData, int> OnItemSell;

    private void Awake()
    {
        len = itemSlots.Count;
        itemUId = new ItemData[len];
        for(int i = 0; i < len; i++)
        {
            itemSlots[i].Init(this, i);
            itemUId[i] = null;
            RemoveItem(i);
        }
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
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].SetSlotUI(uid);
                itemUId[i] = item;

                OnItemAdd?.Invoke(item);
                return true;
            }
        }

        return false;
    }

    public bool RemoveItem(int index)
    {
        if(index < 0 || index >= len) 
            return false;

        if (itemSlots[index].RemoveSlotUI())
        {
            itemSlots[index].gameObject.SetActive(false);
            itemUId[index] = null;

            return true;
        }

        return false;
    }

    public void OnSelectItem(int index, string uid)
    {
        ItemData item = Managers.Item.GetItemData(uid);

        OnClickItem?.Invoke(item, index);
    }

    public void SellItem(int index)
    {
        string sellUID = itemSlots[index].ItemUID;

        ItemData item = Managers.Item.GetItemData(sellUID);

        if (RemoveItem(index))
        {
            OnItemSell?.Invoke(item, index);
        }
    }
}
