using System;
using Unity.VisualScripting;

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
        view.SetIconImage(model.IconPath);
        view.TowerGrade(model.Grade, model.nextGradeUID);
        view.SetTowerName(model.IconPath);
        view.SetSkillName(model.SkillID);
        view.SetSkillDes("타워 스킬 설명");
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
