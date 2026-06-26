using System;
using UnityEngine;

public class RunEffectDataManager
{
    private StageManager stage;
    private RunSessionDataManager session;
    private RunStatUpgradeManager statUpgrade;

    public void Init(StageManager getStage, RunSessionDataManager getSession, RunStatUpgradeManager getStat)
    {
        stage = getStage;
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
            case ItemOptions.AbilityTriggerRequirement:
                ApplyAbilityTriggerRequirement(item);
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
                RemoveItemAtkDamage(item);
                break;
            case ItemOptions.AtkSpeedUp:
                RemoveItemAtkSpeed(item);
                break;
            case ItemOptions.GoldDropIncrease:
                RemoveGoldDrop(item);
                break;
            case ItemOptions.InterestBoost:
                RemoveInterestBoost(item);
                break;
            case ItemOptions.AbilityTriggerRequirement:
                RemoveAbilityTriggerRequirement(item);
                break;
        }
    }

    private void ApplyAtkDamage(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddItemAtkDamage(item.scopeRange, item.value);
    }

    private void RemoveItemAtkDamage(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddItemAtkDamage(item.scopeRange, -item.value);
    }

    private void ApplyAtkSpeed(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddItemAtkSpeed(item.scopeRange, item.value);
    }

    private void RemoveItemAtkSpeed(ItemData item)
    {
        if (item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddItemAtkSpeed(item.scopeRange, -item.value);
    }

    private void ApplyHealLife(ItemData item)
    {
        if (session == null)
            return;

        session.HealLife(item.value);
    }

    private void ApplyRandomDropGold(ItemData item)
    {
        if (session == null)
            return;

        int gold = UnityEngine.Random.Range(1, item.value + 1);
        stage.UsingGold(GoldChangedReason.GAIN, gold);
    }

    private void ApplyAbilityTriggerRequirement(ItemData item)
    {
        if (statUpgrade == null || item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddAbilityTriggerRequirement(item.value);
    }

    private void RemoveAbilityTriggerRequirement(ItemData item)
    {
        if (statUpgrade == null || item.target != ItemTarget.Tower)
            return;

        statUpgrade.AddAbilityTriggerRequirement(-item.value);
    }

    private void ApplyGoldDrop(ItemData item) => statUpgrade.AddGoldDropIncrease(item.value);
    private void RemoveGoldDrop(ItemData item) => statUpgrade.AddGoldDropIncrease(-item.value);
    private void ApplyInterestBoost(ItemData item) => statUpgrade.ChangeMaxInterest(item.value);
    private void RemoveInterestBoost(ItemData item) => statUpgrade.ChangeMaxInterest(-item.value);
}
