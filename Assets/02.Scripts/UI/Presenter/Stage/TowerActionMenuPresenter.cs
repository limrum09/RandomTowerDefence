using System;

public class TowerActionMenuPresenter
{
    private Tower model;
    private TowerActionMenuView view;

    public event Action OnClickMove;
    public event Action<Tower> OnClickGradeUpgrade;
    public event Action<Tower> OnClickStatUpgrade;
    public event Action OnClickTowerMoveToQueueSlot;

    public TowerActionMenuPresenter(TowerActionMenuView getView)
    {
        view = getView;

        view.BindMove(OnClickedMove);
        view.BindGradeUpgrade(OnClickedGradeUpgrade);
        view.BindStatUpgrade(OnClickedStatUpgrade);
        view.BindTowerMoveToQueueSlot(OnClickedTowerMoveToQueueSlot);
    }

    public void SetModel(Tower getModel)
    {
        model = getModel;
        view.Show();
    }

    public void Hide()
    {
        model = null;
        view.Hide();
    }

    private void OnClickedMove() => OnClickMove?.Invoke();
    private void OnClickedGradeUpgrade() => OnClickGradeUpgrade?.Invoke(model);
    private void OnClickedStatUpgrade() => OnClickStatUpgrade?.Invoke(model);
    private void OnClickedTowerMoveToQueueSlot() => OnClickTowerMoveToQueueSlot?.Invoke();
}
