using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    [SerializeField]
    private List<StoreSlotUI> slots = new List<StoreSlotUI>();
    [SerializeField]
    private QueueController tempQueue;

    private void Start()
    {
        slots[0].SetStoreSlot("T0011", this);
        slots[1].SetStoreSlot("T0012", this);
        slots[2].SetStoreSlot("T0021", this);
        slots[3].SetStoreSlot("T0031", this);
        slots[4].SetStoreSlot("T0041", this);
        slots[5].SetStoreSlot("T0051", this);
        slots[6].SetStoreSlot("T0061", this);
    }

    public void OnClickSlotUI(string uid)
    {
        tempQueue.AddTower(uid);
    }
}
