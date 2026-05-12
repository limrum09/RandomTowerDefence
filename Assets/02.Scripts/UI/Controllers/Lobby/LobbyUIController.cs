using System;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField]
    private SelectStageView selectView;

    public SelectStagePresenter selectPresenter;

    private void Start()
    {
        selectPresenter = new SelectStagePresenter(selectView);

        selectPresenter.onSelectStage += OnSelectStageLevel;


        HideAllUI();
    }

    private void OnDestroy()
    {
        selectPresenter.onSelectStage -= OnSelectStageLevel;
    }

    private void HideAllUI()
    {
        HideSelectStageLevelView();
    }

    public event Action<string> OnSelectStage;


    public void ShowSelectStageLevelView()
    {
        selectPresenter?.Show();
    }

    public void HideSelectStageLevelView()
    {
        selectPresenter?.Hide();
    }

    public void OnSelectStageLevel(string level)
    {
        HideAllUI();
        OnSelectStage?.Invoke(level);
    }
}
