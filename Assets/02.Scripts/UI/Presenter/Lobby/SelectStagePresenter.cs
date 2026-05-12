using System;

public class SelectStagePresenter
{
    private SelectStageView view;
    public SelectStagePresenter(SelectStageView getView)
    {
        view = getView;
        view.BindEasyStageButton(OnSelectStage);
        view.BindNormalStageButton(OnSelectStage);
        view.BindHardStageButton(OnSelectStage);
        view.BindHellStageButton(OnSelectStage);
    }

    public Action<string> onSelectStage;

    public void OnSelectStage(string stage)
    {
        onSelectStage?.Invoke(stage);

        Hide();
    }

    public void Show()
    {
        view.Show();
    }

    public void Hide()
    {
        view.Hide();
    }
}
