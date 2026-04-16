public class TowerGradeUpgradePresenter
{
    private Tower model;
    private TowerController towerCtr;
    private TowerGradeUpgradeView gradeUpgradeView;

    public TowerGradeUpgradePresenter(TowerGradeUpgradeView getView, TowerController ctr)
    {
        gradeUpgradeView = getView;
        towerCtr = ctr;
    }

    public void SetTowerController(TowerController controller)
    {
        if (towerCtr != null)
            return;

        towerCtr = controller;
    }

    public void SetModel(Tower getModel)
    {
        this.model = getModel;

        gradeUpgradeView.Show();
        gradeUpgradeView.SetIconImage(model.IconPath);
        gradeUpgradeView.TowerGrade(model.Grade, model.nextGradeUID);
        gradeUpgradeView.SetTowerName(model.IconPath);
        gradeUpgradeView.SetSkillName(model.SkillID);
        gradeUpgradeView.SetSkillDes("타워 스킬 설명");
        gradeUpgradeView.SetDamageCurrentValue(model.BaseAtk);
        gradeUpgradeView.SetAttackSpeedCurrentValue(model.BaseAtkSpeed);
        gradeUpgradeView.SetRangeCurrentValue(model.AtkRange);
        gradeUpgradeView.PremiumUpgradePirce(1000);
        gradeUpgradeView.NormalUpgradePrice(300);
        gradeUpgradeView.TowerSellPrice(model.SellPrice);
    }

    public void HideModel()
    {
        gradeUpgradeView.Hide();
    }
}
