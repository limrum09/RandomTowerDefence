using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerGradeUpgradePresenter
{
    private Tower model;
    private TowerGradeUpgradeView view;

    public event Action onClickNormalUpgrade;
    public event Action onClickPremiumUpgrade;
    public event Action onClickTowerSell;
    public TowerGradeUpgradePresenter(TowerGradeUpgradeView getView)
    {
        view = getView;

        view.BindNormalUpgrade(OnClickNormalUpgrade);
        view.BindPreminumUpgrade(OnClickPremiunUpgrade);
        view.BindTowerSell(OnClickTowerSell);
    }

    public void SetModel(Tower getModel)
    {
        this.model = getModel;

        Sprite icon = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{model.IconPath}_{model.Grade}_Idle");

        view.Show();
        view.SetTowerName(model.TowerName());
        view.SetIconImage(icon);
        view.TowerGrade(model.Grade, model.nextGradeUID);
        view.SetSkillName(model.SkillName());
        view.SetDamageCurrentValue(model.CurrentDamage);
        view.SetAttackSpeedCurrentValue(model.CurrentAtkSpeed);
        view.SetRangeCurrentValue(model.AtkRange);
        view.PremiumUpgradePirce(1000);
        view.NormalUpgradePrice(300);
        view.TowerSellPrice(model.SellPrice);


        string skillDesSplit = model.SkillDes();

        List<SkillValueCollect> skill = Managers.TowerSkill.GetTowerCollections(model.Type);

        object[] des = new object[skill.Count * 2];

        for(int i = 0; i < skill.Count; i++)
        {
            des[i] = skill[i].towerCnt;
            des[i + skill.Count] = skill[i].value;
        }

        skillDesSplit = string.Format(skillDesSplit, des);

        view.SetSkillDes(skillDesSplit);
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

    private void OnClickTowerSell()
    {
        if(model == null) 
            return;

        onClickTowerSell?.Invoke();
    }
}
