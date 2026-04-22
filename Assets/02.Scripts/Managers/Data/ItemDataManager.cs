using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ItemGrade
{
    Normal,
    Rare,
    Epic,
    Legend,
    Mythic
}

public enum ItemOptions
{
    AtkDamageUP,
    AtkSpeedUp,
    AbilityTriggerRequirement,
    GoldDropIncrease,
    HealLife,
    RandomGold,
    InterestBoost,
    QueueSlot,
    MarketSlot
}

public enum ItemTarget
{
    Tower,
    System
}

public enum ScopeRange
{
    AllTower,
    HumanTower,
    ElfTower,
    OrcTower,
    BeastTower,
    DragonTower,
    DwarfTower,
    Global
}

[Serializable]
public class ItemDataRow
{
    public string Item_UID;
    public string Item_Name;
    public string Grade;
    public string Item_Option;
    public string Target;
    public string Scope_Range;
    public int Value;
    public int Buy_Price;
    public int Sale_Price;
    public string String_Key;
    public string Item_Desc;
    public string Icon_UID;
}

[Serializable]
public class ItemDataRowList
{
    public List<ItemDataRow> datas = new List<ItemDataRow>();
}

public class ItemData
{
    public string itemUID;
    public string itemName;
    public ItemGrade grade;
    public ItemOptions itemOption;
    public ItemTarget target;
    public ScopeRange scopeRange;
    public int value;
    public int buyPrice;
    public int salePrice;
    public string stringKey;
    public string itemDesc;
    public string iconUID;

    public ItemData(string itemUID, string itemName, ItemGrade grade, ItemOptions itemOption, ItemTarget target, ScopeRange scopeRange, int value, int buyPrice, int salePrice, string stringKey, string itemDesc, string iconUID)
    {
        this.itemUID = itemUID;
        this.itemName = itemName;
        this.grade = grade;
        this.itemOption = itemOption;
        this.target = target;
        this.scopeRange = scopeRange;
        this.value = value;
        this.buyPrice = buyPrice;
        this.salePrice = salePrice;
        this.stringKey = stringKey;
        this.itemDesc = itemDesc;
        this.iconUID = iconUID;
    }
}

public class ItemDataManager
{
    Dictionary<string, ItemData> itemDatas = new Dictionary<string, ItemData>();
    Dictionary<int, string> itemUIDs = new Dictionary<int, string>();

    private void GetItemDatasToJson()
    {
        ItemDataRowList rowList = JsonLoader.LoadFromResources<ItemDataRowList>("Data/ItemData");

        if (rowList == null || rowList.datas == null)
            return;

        int itemIndex = 0;
        foreach(ItemDataRow row in rowList.datas)
        {
            if(!Enum.TryParse(row.Grade, true, out ItemGrade itemGrade))
                continue;

            if (!Enum.TryParse(row.Item_Option, true, out ItemOptions itemOption))
                continue;

            if (!Enum.TryParse(row.Target, true, out ItemTarget itemTarget))
                continue;


            if (!Enum.TryParse(row.Scope_Range, true, out ScopeRange itemScopeRange))
                continue;

            ItemData data = new ItemData(row.Item_UID, row.Item_Name, itemGrade, itemOption, itemTarget
                , itemScopeRange, row.Value, row.Buy_Price, row.Sale_Price, row.String_Key, row.Item_Desc, row.Icon_UID);

            itemDatas[data.itemUID] = data;
            itemUIDs[itemIndex] = data.itemUID;
            itemIndex++;
        }
    }

    public void Init()
    {
        itemDatas.Clear();
        GetItemDatasToJson();
    }

    public ItemData GetItemData(string uid)
    {
        if (itemDatas.TryGetValue(uid, out ItemData data))
            return data;

        return null;
    }

    public string GetItemUID(int index)
    {
        if (itemUIDs.TryGetValue(index, out string itemUID))
            return itemUID;

        return string.Empty;
    }
}
