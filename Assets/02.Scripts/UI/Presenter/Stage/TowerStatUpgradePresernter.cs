using System;
using UnityEngine;

public class TowerStatUpgradePresernter
{
    private Tower model;
    private TowerStatUpgradeView view;

    public event Action<Tower> onClickDamageUpgrade;
    public event Action<Tower> onClickAttackSpeedUpgrade;
    public TowerStatUpgradePresernter(TowerStatUpgradeView getView)
    {
        view = getView;

        view.BindDamageUpgrade(OnClickDamageStatUpgrade);
        view.BindAttakSpeedUpgrade(OnClickAttakSpeedUpgrade);
    }

    public void SetModel(Tower getModel)
    {
        model = getModel;
        string modelUid = model.TowerUID;

        Sprite icon = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{model.IconPath}_{model.Grade}_Idle");

        view.SetIconImage(icon);
        view.SetTowerName(model.TowerName());
        view.TowerGrade(model.Grade, model.nextGradeUID);
        view.SetSkillName(model.SkillName());

        RunStatUpgradeManager tempRunUpgradeManager = model.StatUpgrade;
        TowerSessionUpgradeData tempSessionDamageData = 
            Managers.SessionTowerUpgrade.GetUpgradeStepData(model.TowerUID, UpgradeType.Damge);

        int currentStatDamageStep = tempRunUpgradeManager.GetAtkDamageStep(model.Type);
        int currentItemDamageStep = tempRunUpgradeManager.GetItemAtkDamageStep(model.Type);
        int currentSkillDamageStep = tempRunUpgradeManager.GetSkillAtkDamageStep(model.Type);
        string nextDamageText = $"{model.CurrentDamage + tempSessionDamageData.increaseValue} " +
            $"+ ({tempSessionDamageData.increaseValue})";

        view.SetCurrentDamageStepText(currentStatDamageStep + currentItemDamageStep + currentSkillDamageStep);
        view.SetCurrentDamageText(model.CurrentDamage);
        view.SetNextDamageStepText(currentStatDamageStep + currentItemDamageStep + currentSkillDamageStep + 1);
        view.SetNextDamageText(nextDamageText);
        view.SetDamaePriceText(tempSessionDamageData.baseCost + (tempSessionDamageData.increaseCost * currentStatDamageStep));

        TowerSessionUpgradeData tempSessionSpeedData = 
            Managers.SessionTowerUpgrade.GetUpgradeStepData(model.TowerUID, UpgradeType.Speed);

        int currentStatSpeedStep = tempRunUpgradeManager.GetAtkSpeedStep(model.Type);
        int currentItemSpeedStep = tempRunUpgradeManager.GetItemAtkSpeedStep(model.Type);
        int currentSkillSpeedStep = tempRunUpgradeManager.GetSkillAtkSpeedStep(model.Type);
        string nextSpeedText = $"{model.CurrentAtkSpeed + tempSessionSpeedData.increaseValue} " +
            $"+ ({tempSessionSpeedData.increaseValue})";

        view.SetCurrentAttakSpeedStepText(currentStatSpeedStep + currentItemSpeedStep + currentSkillSpeedStep);
        view.SetCurrentAttakSpeedText(model.CurrentAtkSpeed);
        view.SetNextAttakSpeedStepText(currentStatSpeedStep + currentItemSpeedStep + currentSkillSpeedStep + 1);
        view.SetNextAttakSpeedText(nextSpeedText);
        view.SetAttakSpeedPriceText(tempSessionSpeedData.baseCost + (tempSessionSpeedData.increaseCost * currentStatSpeedStep));

        view.Show();
    }

    public void Clear()
    {
        view.Clear();
    }

    public void Hide()
    {
        view.Hide();
    }

    public void OnClickDamageStatUpgrade()
    {
        if (model == null)
            return;

        onClickDamageUpgrade?.Invoke(model);
    }

    public void OnClickAttakSpeedUpgrade()
    {
        if (model == null) 
            return;

        onClickAttackSpeedUpgrade?.Invoke(model);
    }
}
