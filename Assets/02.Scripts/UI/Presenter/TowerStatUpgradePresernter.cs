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

        view.SetIconImage(model.IconPath);
        view.SetTowerName(model.IconPath);
        view.TowerGrade(model.Grade, model.nextGradeUID);
        view.SetSkillName(model.SkillID);
        view.SetCurrentDamageStepText(1);
        view.SetCurrentDamageText(model.CurrentDamage);
        view.SetNextDamageStepText(2);

        int temp1 = (model.CurrentDamage * 12) / 10;
        string nextDamage = $"{temp1} ( + {temp1 - model.CurrentDamage})";
        view.SetNextDamageText(nextDamage);
        view.SetDamaePriceText(400);

        view.SetCurrentAttakSpeedStepText(1);
        view.SetCurrentAttakSpeedText(model.CurrentAtkSpeed);
        view.SetNextAttakSpeedStepText(2);

        float temp2 = model.CurrentAtkSpeed * 1.2f;
        string nextAttackSpeed = $"{temp2} ( +{Mathf.Floor((temp2 - model.CurrentAtkSpeed) * 10f) / 10f})";
        view.SetNextAttakSpeedText(nextAttackSpeed);
        view.SetAttakSpeedPriceText(500);

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
