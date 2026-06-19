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

        view.Init();
        view.BindDamageUpgrade(OnClickDamageStatUpgrade);
        view.BindAttakSpeedUpgrade(OnClickAttakSpeedUpgrade);
    }

    public void SetModel(Tower getModel)
    {
        model = getModel;
        string modelUid = model.TowerUID;

        Sprite icon = ResourceCache.Load<Sprite>($"Tower/Images/Icon_Tower_{model.IconPath}_{model.Grade}_Idle");

        view.SetIconImage(icon);
        view.SetTowerName(Managers.Local.GetString("Tower", model.StringKey));
        view.TowerGrade(model.Grade, model.nextGradeUID);
        view.SetSkillName(model.SkillName());

        TowerData data = Managers.TowerData.GetTowerData(modelUid);
        TowerStatPreview currentStat = TowerStatCalculator.Calculate(data);

        TowerSessionUpgradeData tempSessionDamageData =
            Managers.SessionTowerUpgrade.GetUpgradeStepData(model.TowerUID, UpgradeType.Damge);

        int currentDamageStep = TowerStatCalculator.GetAttackStep(model.Type);
        string nextDamageText = $"{(model.CurrentDamage + tempSessionDamageData.increaseValue)} (+{tempSessionDamageData.increaseValue})";

        view.SetCurrentDamageStepText(currentDamageStep);
        view.SetCurrentDamageText(model.CurrentDamage);
        view.SetNextDamageStepText(currentDamageStep + 1);
        view.SetNextDamageText(nextDamageText);
        view.SetDamaePriceText(tempSessionDamageData.baseCost + (tempSessionDamageData.increaseCost * currentDamageStep));

        TowerSessionUpgradeData tempSessionSpeedData = 
            Managers.SessionTowerUpgrade.GetUpgradeStepData(model.TowerUID, UpgradeType.Speed);

        int currentSpeedStep = TowerStatCalculator.GetSpeedStep(model.Type);
        string nextSpeedText = $"{(model.CurrentAtkSpeed + tempSessionSpeedData.increaseValue).ToString("N2")} (+{tempSessionSpeedData.increaseValue.ToString("N2")})";

        view.SetCurrentAttakSpeedStepText(currentSpeedStep);
        view.SetCurrentAttakSpeedText(model.CurrentAtkSpeed);
        view.SetNextAttakSpeedStepText(currentSpeedStep + 1);
        view.SetNextAttakSpeedText(nextSpeedText);
        view.SetAttakSpeedPriceText(tempSessionSpeedData.baseCost + (tempSessionSpeedData.increaseCost * currentSpeedStep));
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
