using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class QueueController : MonoBehaviour
{
    [SerializeField]
    private TowerController towerController;
    [SerializeField]
    private List<QueueSlotUI> slots = new List<QueueSlotUI>();
    private string[] towerUID;
    int length;

    private void Awake()
    {
        length = slots.Count;

        towerUID = new string[length];

        for(int i = 0; i < length; i++)
        {
            towerUID[i] = string.Empty;
            slots[i].Init(this, i);
            slots[i].RemoveSlotUI();
        }
    }

    public void BindTowerController(TowerController getTowerController)
    {
        towerController = getTowerController;
    }

    public bool AddTower(string uid)
    {
        for(int i = 0; i < length; i++)
        {
            if (slots[i].IsQueueEmpty)
            {
                towerUID[i] = uid;
                slots[i].SetSlotUI(uid);
                return true;
            }
        }

        return false;
    }

    public void RemoveTower(int index)
    {
        if (index < 0 || index >= length)
            return;

        towerUID[index] = string.Empty;
        slots[index].RemoveSlotUI();
    }

    public string GeTowerUID(int index)
    {
        if (index < 0 || index >= length)
            return string.Empty;

        return towerUID[index];
    }

    public void OnClickQueueSlot(int index, string uid)
    {
        if (index < 0 || index >= length)
            return;

        if (uid == string.Empty)
            return;

        if (!towerUID.Contains(uid))
            return;

        towerController.BeginBuildTower(uid, index, this);
    }
}
