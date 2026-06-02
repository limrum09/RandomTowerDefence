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

        Show();

        model = getModel;

        Sprite icon = Resources.Load<Sprite>($"Enemy/SpriteLibrary/{model.enemyUID}");
        view.SetIcon(icon);

        string enemyName = Managers.Local.GetString("Sheets", model.stringKey);
        view.SetName(enemyName);

        view.SetLevel($"Lv. {model.level}");
        view.SetHealthText(model.maxHP);
        view.SetSheildText(model.maxShield);
        view.SetSpeedText(model.moveSpeed);
        view.SetSkillName(Managers.Local.GetString("Sheets", model.skillStringKey));

        string des = Managers.Local.GetString("Sheets", model.skillDesStringKey);
        string formatStr = string.Format(des, model.duration, model.cooldown, model.tickInterval, model.range, $"{model.skillValue}(+{model.skillValue - model.basicSkillValue})");
        view.SetSkillDesText(formatStr);
    }

    public void Hide()
    {
        view.Hide();
    }

    public void Show()
    {
        view.Show();
    }
}
