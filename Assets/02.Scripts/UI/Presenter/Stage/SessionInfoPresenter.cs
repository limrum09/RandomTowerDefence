using Unity.VisualScripting;
using UnityEngine;

public class SessionInfoPresenter
{
    private RunSessionDataManager model;
    private SessionInfoView view;

    public SessionInfoPresenter(SessionInfoView getView)
    {
        this.view = getView;
    }

    public void GetRunSessionDatamanager(RunSessionDataManager getModel)
    {
        model = getModel;

        view.SetCurrentLevel(model.SessionState.Level);
        view.SetCurrentExpBar(model.SessionState.CurrentExp, model.GetNeedExp());
        view.SetCurrentLife(model.SessionState.CurrentLife);
        view.SetCurrentWave(model.SessionState.CurrentWave);

        model.OnLevelChanged += view.SetCurrentLevel;
        model.OnExpChanged += view.SetCurrentExpBar;
        model.OnLifeChanged += view.SetCurrentLife;
        model.OnWaveChanged += view.SetCurrentWave;
    }
    public void UnBindAction()
    {
        model.OnLevelChanged -= view.SetCurrentLevel;
        model.OnExpChanged -= view.SetCurrentExpBar;
        model.OnLifeChanged -= view.SetCurrentLife;
        model.OnWaveChanged -= view.SetCurrentWave;
    }
}
