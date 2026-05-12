using UnityEngine;

public class EnemyInfoPresenter
{
    private EnemyInfoView view;
    private WaveEnemyRosterData model;
    public EnemyInfoPresenter(EnemyInfoView getView)
    {
        view = getView;
    }

    public void GetModel(WaveEnemyRosterData getModel)
    {
        if (getModel == null)
            return;

        Show();

        model = getModel;
        int level = model.enemyLevel;

        EnemyData temp = Managers.EnemyData.GetEnemyData(model.enemyUID);

        Sprite icon = Resources.Load<Sprite>($"Enemy/SpriteLibrary/{temp.enemyUID}");
        view.SetIcon(icon);

        string enemyName = Managers.Local.GetString(temp.stringKey);
        view.SetName(enemyName);

        view.SetLevel($"Lv. {level}");
        view.SetHealthText(temp.basicHp + (temp.increaseHP * level));
        view.SetSheildText(temp.basicShield + (temp.increaseShield * level));
        view.SetSpeedText(temp.moveSpeed);

        EnemySkillData skill = Managers.EnemySkillData.GetEnemySkillData(temp.enemySkillUID);
        view.SetSkillName(Managers.Local.GetString(skill.stringKey));
        view.SetSkillDesText(Managers.Local.GetString(skill.desStringKey));
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
