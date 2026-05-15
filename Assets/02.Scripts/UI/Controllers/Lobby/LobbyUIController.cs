using System;
using System.Net.NetworkInformation;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField]
    private SelectStageView selectView;
    [SerializeField]
    private MetaUpgradeView metaView;

    public SelectStagePresenter selectPresenter;

    public event Func<MetaUpgradeTarget, MetaUpgradeType, string, int, bool> OnMetaUpgrade;
    public event Action<string> OnSelectStage;

    private void Awake()
    { 
        selectPresenter = new SelectStagePresenter(selectView);

        selectPresenter.onSelectStage += OnSelectStageLevel;

        metaView.OnMetaUpgrade += OnClickMetaUpgrade;

        HideAllUI();
    }

    private void OnDestroy()
    {
        selectPresenter.onSelectStage -= OnSelectStageLevel;
        metaView.OnMetaUpgrade -= OnClickMetaUpgrade;
    }

    private void ShowAllUI()
    {
        ShowSelectStageLevelView();
        ShowMetaUpgradeView();
    }

    private void HideAllUI()
    {
        HideSelectStageLevelView();
        HideMetaUpgradeView();
    }

    private bool OnClickMetaUpgrade(MetaUpgradeTarget metaType, MetaUpgradeType upgradeType, string uid, int upValue)
    {
        return OnMetaUpgrade?.Invoke(metaType, upgradeType, uid, upValue) ?? false;
    }

    public void ShowMetaUpgradeView()
    {
        metaView?.Show();
    }

    public void HideMetaUpgradeView()
    {
        metaView?.HIde();
    }

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
