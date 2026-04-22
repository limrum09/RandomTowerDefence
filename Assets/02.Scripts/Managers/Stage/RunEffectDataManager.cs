using UnityEngine;

public class RunEffectDataManager
{
    private RunSessionDataManager session;
    private RunStatUpgradeManager statUpgrade;

    public void Init(RunSessionDataManager getSession, RunStatUpgradeManager getStat)
    {
        session = getSession;
        statUpgrade = getStat;
    }

    public void ApplyItemEffect(ItemData item)
    {
        if (item == null)
            return;

        switch (item.itemOption)
        {
            case ItemOptions.AtkDamageUP:
                ApplyAtkDamage(item); 
                break;
            case ItemOptions.AtkSpeedUp:
                ApplyAtkSpeed(item);
                break;
            case ItemOptions.GoldDropIncrease:
                ApplyGoldDrop(item);
                break;
            case ItemOptions.InterestBoost:
                ApplyInterestBoost(item);
                break;
            case ItemOptions.RandomGold:
                ApplyRandomDropGold(item);
                break;
            case ItemOptions.HealLife:
                ApplyHealLife(item);
                break;
        }
    }

    public void RemoveItemEffect(ItemData item)
    {
        if (item == null)
            return;

        switch (item.itemOption)
        {
            case ItemOptions.AtkDamageUP:
                RemoveAtkDamage(item);
                break;
            case ItemOptions.AtkSpeedUp:
                RemoveAtkSpeed(item);
                break;
            case ItemOptions.GoldDropIncrease:
                RemoveGoldDrop(item);
                break;
            case ItemOptions.InterestBoost:
                RemoveInterestBoost(item);
                break;
        }
    }

    private void ApplyAtkDamage(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddAtkDamage(item.scopeRange, item.value);
    }

    private void RemoveAtkDamage(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.RemoveAtkDamageStep(item.scopeRange, item.value);
    }

    private void ApplyAtkSpeed(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddAtkSpeed(item.scopeRange, item.value);
    }

    private void RemoveAtkSpeed(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.RemoveAtkSpeedStep(item.scopeRange, item.value);
    }

    private void ApplyHealLife(ItemData item)
    {
        if (session == null)
            return;

        session.ChangeLife(item.value);
    }

    private void ApplyRandomDropGold(ItemData item)
    {
        if (session == null)
            return;

        int gold = Random.Range(1, item.value + 1);
        session.AddGold(gold);
    }


    private void ApplyGoldDrop(ItemData item) => statUpgrade.AddGoldDropIncrease(item.value);
    private void RemoveGoldDrop(ItemData item) => statUpgrade.RemoveGoldDropIncrease(item.value);
    private void ApplyInterestBoost(ItemData item) => statUpgrade.AddInterestBoost(item.value);
    private void RemoveInterestBoost(ItemData item) => statUpgrade.RemoveInterestBoost(item.value);
}
