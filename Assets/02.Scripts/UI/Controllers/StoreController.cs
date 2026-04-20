using System.Collections.Generic;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    [SerializeField]
    private List<StoreSlotUI> slots = new List<StoreSlotUI>();
    [SerializeField]
    private QueueController tempQueue;

    private int len;
    private void Start()
    {
        len = slots.Count;
        for(int i = 0; i <  len; i++)
        {
            slots[i].SetStoreCTR(this);
        }

        RerollStoreUI(0);
    }

    private void SetStoreUI()
    {
        len = slots.Count;

        for (int i = 0; i < len; i++)
        {
            int ranGrade = Random.Range(1, 5);
            int ranTower = Random.Range(0, 5);

            string[] tempTower = Managers.TowerData.GetTowerGradeUID(ranGrade);

            if(tempTower.Length == 6)
            {
                string selectTower = tempTower[ranTower];
                slots[i].SetStoreSlot(selectTower);
            }
            else
            {
                slots[i].SetStoreSlot();
            }
        }
    }

    public void RerollStoreUI(int pay)
    {
        SetStoreUI();
    }

    public void OnClickSlotUI(string uid)
    {
        tempQueue.AddTower(uid);
    }
}
