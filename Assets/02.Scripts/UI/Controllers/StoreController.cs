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
    private QueueController queueSlots;
    [SerializeField]
    private ItemSlotController itemSlots;

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
        int ranGrade = Random.Range(1, 5);
        int ranTower = Random.Range(0, 5);

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
        stage.RunSession.UsingGold(amount);
        SetStoreUI();
    }

    public void OnClickTowerSlotUI(string uid)
    {
        queueSlots.AddTower(uid);
    }

    public void OnClickItemSlotUI(string uid)
    {
        itemSlots.AddItemSlot(uid);
    }

    public void BuyEXP(int amount)
    {
        if(stage.RunSession.UsingGold(amount))
            stage.RunSession.AddExp(2);
    }
}
