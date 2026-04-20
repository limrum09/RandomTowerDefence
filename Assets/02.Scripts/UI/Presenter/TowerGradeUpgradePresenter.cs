using System;
using Unity.VisualScripting;
using UnityEngine;

public class TowerGradeUpgradePresenter
{
    private Tower model;
    private TowerGradeUpgradeView view;

    public event Action onClickNormalUpgrade;
    public event Action onClickPremiumUpgrade;
    public TowerGradeUpgradePresenter(TowerGradeUpgradeView getView)
    {
        view = getView;

        view.BindNormalUpgrade(OnClickNormalUpgrade);
        view.BindPreminumUpgrade(OnClickPremiunUpgrade);
    }

    public void SetModel(Tower getModel)
    {
        this.model = getModel;

        view.Show();
        view.SetTowerName(model.TowerName());
        view.SetIconImage(model.IconPath);
        view.TowerGrade(model.Grade, model.nextGradeUID);
        view.SetSkillName(model.SkillName());
        view.SetSkillDes(model.SkillDes());
        view.SetDamageCurrentValue(model.BaseAtk);
        view.SetAttackSpeedCurrentValue(model.BaseAtkSpeed);
        view.SetRangeCurrentValue(model.AtkRange);
        view.PremiumUpgradePirce(1000);
        view.NormalUpgradePrice(300);
        view.TowerSellPrice(model.SellPrice);
    }

    public void HideModel()
    {
        view.Hide();
    }

    private void OnClickNormalUpgrade()
    {
        if (model == null)
            return;

        onClickNormalUpgrade?.Invoke();
    }

    private void OnClickPremiunUpgrade()
    {
        if (model == null)
            return;

        onClickPremiumUpgrade?.Invoke();
    }
}
