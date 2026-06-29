using System;
using UnityEngine;

public class EnemyInfoPresenter
{
    private EnemyInfoView view;
    private EnemyResolveInfo model;
    public EnemyInfoPresenter(EnemyInfoView getView)
    {
        view = getView;
    }

    public void GetModel(EnemyResolveInfo getModel)
    {
        if (getModel == null)
            return;

        model = getModel;

        Sprite icon = ResourceCache.Load<Sprite>($"Enemy/Images/{model.enemyUID}");
        view.SetIcon(icon);

        string enemyName = Managers.Local.GetString("Enemy", model.stringKey);
        view.SetName(enemyName);

        view.SetLevel($"Lv. {model.level}");
        view.SetHealthText(model.maxHP);
        view.SetSheildText(model.maxShield);
        view.SetSpeedText(model.moveSpeed);
        view.SetSkillName(Managers.Local.GetString("Enemy", model.skillStringKey));

        string des = Managers.Local.GetString("Enemy", model.skillDesStringKey);

        try
        {
            string formatStr = string.Format(des, model.duration, model.cooldown, model.tickInterval, model.range, $"{model.skillValue}(+{model.skillValue - model.basicSkillValue})");
            view.SetSkillDesText(formatStr);
        }
        catch (FormatException e)
        {
            Debug.LogWarning($"Enemy Skill Des format error : {model.skillDesStringKey}, {e.Message}");
            view.SetSkillDesText(des);
        }
        
    }

    public void Hide()
    {
        view.Hide();
    }
}
