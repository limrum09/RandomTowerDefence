using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StoreController : MonoBehaviour
{
    [SerializeField]
    private StageManager stage;
    [SerializeField]
    private List<StoreSlotUI> slots = new List<StoreSlotUI>();
    [SerializeField]
    private TextMeshProUGUI currentGoldText;
    [SerializeField]
    private QueueUIController queueSlots;
    [SerializeField]
    private ItemSlotUIController itemSlots;

    private int len;

    private void OnDestroy()
    {
        stage.RunSession.OnGoldAmountChanged -= ChangedGold;
    }
    private void Start()
    {
        len = slots.Count;
        for(int i = 0; i <  len; i++)
        {
            slots[i].SetStoreCTR(this);
        }

        RerollStoreUI(0);

        stage.RunSession.OnGoldAmountChanged += ChangedGold;
        ChangedGold(stage.RunSession.SessionState.Gold);
    }

    private void SetStoreUI()
    {
        len = slots.Count;

        for (int i = 0; i < len; i++)
        {
            int ran = Random.Range(0, 5);

            if (ran <= 3)
                SlotGetTowerUID(i);
            else
                SlotGetItemUID(i);
        }
    }

    private void SlotGetTowerUID(int i)
    {
        int ranGrade = Random.Range(1, 6);
        int ranTower = Random.Range(0, 6);

        string[] tempTower = Managers.TowerData.GetTowerGradeUID(ranGrade);

        if (tempTower.Length == 6)
        {
            string selectTower = tempTower[ranTower];
            slots[i].SetStoreSlot(selectTower);
        }
        else
        {
            slots[i].SetStoreSlot();
        }
    }

    private void SlotGetItemUID(int i)
    {
        int uidIndex = Random.Range(0, 18);
        string getUID = Managers.Item.GetItemUID(uidIndex);

        if (!string.IsNullOrEmpty(getUID))
        {
            slots[i].SetStoreSlot(getUID);
        }
        else
        {
            slots[i].SetStoreSlot();
        }
    }

    public void ChangedGold(int value)
    {
        currentGoldText.text = value.ToString();
    }

    public void RerollStoreUI(int amount)
    {
        UsingGold(-amount);
        SetStoreUI();
    }

    public bool UsingGold(int amount)
    {
        return stage.RunSession.ChangeGold(amount);
    }

    public bool OnClickTowerSlotUI(string uid)
    {
        return queueSlots.AddTower(uid);
    }

    public bool OnClickItemSlotUI(string uid)
    {
        return itemSlots.AddItemSlot(uid);
    }

    public void BuyEXP(int amount)
    {
        if(UsingGold(-amount))
            stage.RunSession.AddExp(2);
    }
}
