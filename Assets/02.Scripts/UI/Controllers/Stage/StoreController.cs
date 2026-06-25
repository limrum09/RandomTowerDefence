using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum StoreProductType
{
    None,
    Tower,
    Item
}

public class StoreProduct
{
    public string uid { get; }
    public StoreProductType type { get; }
    public int price { get; }
    public string gradeText { get; }
    public Sprite Icon { get; }

    public StoreProduct(string uid, StoreProductType type, int price, string gradeText, Sprite icon)
    {
        this.uid = uid;
        this.type = type;
        this.price = price;
        this.gradeText = gradeText;
        Icon = icon;
    }

    public static StoreProduct Empty => new StoreProduct(string.Empty, StoreProductType.None, 0, "0", null);
}

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
    [SerializeField]
    private StoreToolTip tooltip;

    private int len;

    private void OnDestroy()
    {
        if (stage == null)
            return;

        if (stage.RunSession != null)
            stage.RunSession.OnGoldAmountChanged -= ChangedGold;

        stage.OnWaveEndRefreshStore -= RefreshStoreUIOnWaveClear;
    }
    private void Start()
    {
        len = slots.Count;
        for(int i = 0; i <  len; i++)
        {
            slots[i].SetStoreCTR(this);
        }

        RefreshStoreUIOnWaveClear();

        stage.RunSession.OnGoldAmountChanged += ChangedGold;
        stage.OnWaveEndRefreshStore += RefreshStoreUIOnWaveClear;
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
            slots[i].SetStoreSlot(CreateTowerProduct(selectTower));
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
            slots[i].SetStoreSlot(CreateItemProduct(getUID));
        }
        else
        {
            slots[i].SetStoreSlot();
        }
    }

    private StoreProduct CreateTowerProduct(string uid)
    {
        TowerData data = Managers.TowerData.GetTowerData(uid);

        if (data == null)
            return StoreProduct.Empty;

        Sprite icon = ResourceCache.Load<Sprite>($"Tower/Images/Icon_Tower_{data.towerType}_{data.grade}_Idle");

        return new StoreProduct(
            uid,
            StoreProductType.Tower,
            data.buyPrice,
            data.grade.ToString(),
            icon
            );
    }

    private StoreProduct CreateItemProduct(string uid)
    {
        ItemData data = Managers.Item.GetItemData(uid);

        if (data == null)
            return StoreProduct.Empty;

        Sprite icon = ResourceCache.Load<Sprite>($"Item/Images/{data.iconUID}");

        return new StoreProduct(
            uid,
            StoreProductType.Item,
            data.buyPrice,
            GetItemGradeText(data.grade),
            icon
            );
    }

    private string GetItemGradeText(ItemGrade grade)
    {
        switch (grade)
        {
            case ItemGrade.Normal:
                return "N";
            case ItemGrade.Rare:
                return "R";
            case ItemGrade.Epic:
                return "E";
            case ItemGrade.Legend:
                return "L";
            case ItemGrade.Mythic:
                return "M";
            default:
                return string.Empty;
        }
    }

    private void RefreshStoreUIOnWaveClear()
    {
        SetStoreUI();
    }

    public void ChangedGold(int value)
    {
        currentGoldText.text = value.ToString();
    }

    public void RerollStoreUI(int amount)
    {
        if (!stage.UsingGold(GoldChangedReason.BUY, -amount))
            return;

        SetStoreUI();
    }

    public void RequestBuy(StoreSlotUI slot)
    {
        StoreProduct product = slot.Product;

        if(product == null || product.type == StoreProductType.None) 
            return;

        if (!UsingGold(GoldChangedReason.BUY, -product.price))
            return;

        bool success = false;

        switch (product.type)
        {
            case StoreProductType.Tower:
                success = queueSlots.AddTower(product.uid);
                break;
            case StoreProductType.Item:
                success = itemSlots.AddItemSlot(product.uid);
                break;
        }

        if (!success)
        {
            stage.UsingGold(GoldChangedReason.GAIN, product.price);
            return;
        }
        
        slot.SetStoreSlot(StoreProduct.Empty);
        tooltip.Hide();
    }

    public bool UsingGold(GoldChangedReason reason, int amount)
    {
        return stage.UsingGold(reason, amount);
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
        if(UsingGold(GoldChangedReason.BUY, -amount))
            stage.RunSession.AddExp(2);
    }

    public void OnPointerEnterSlot(StoreSlotUI slot, RectTransform slotRect)
    {
        if(slot == null) 
            return;

        StoreProduct product = slot.Product;

        if (product == null || product.type == StoreProductType.None)
            return;

        tooltip.Show(product, slotRect);
    }

    public void OnPointerExitSlot(StoreSlotUI slot)
    {
        if (slot == null)
            return;

        tooltip.Hide();
    }
}
