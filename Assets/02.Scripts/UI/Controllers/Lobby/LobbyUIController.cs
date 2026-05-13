using System;
using UnityEngine;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField]
    private SelectStageView selectView;

    public SelectStagePresenter selectPresenter;

    public event Action<TowerType, int, int> OnTowerMetaAttackSpeedUpgrade;
    public event Action<TowerType, int, int> OnTowerMetaDamageUpgrade;
    public event Action<string> OnSelectStage;

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

    

    public void TowerMetaDamageUpgrade(TowerType type, int grade, int upvalue)
    {
        OnTowerMetaDamageUpgrade?.Invoke(type, grade, upvalue);
    }

    public void TowerMeraAttackSpeedUpgrade(TowerType type, int grade, int upValue)
    {
        OnTowerMetaAttackSpeedUpgrade?.Invoke(type, grade, upValue);
    }

    public void OnChangeTowerMetaDamageUpgradeLevel(TowerType type, int grade, int level)
    {

    }

    public void OnChangeTowerMetaAttackSpeedUpgradeLevel(TowerType type, int grade, int level)
    {

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
