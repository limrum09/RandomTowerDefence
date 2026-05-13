using System;
using UnityEditor.SceneManagement;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private LobbyUIController lobbyUICtr;

    TowerMetaUpgradeManager towerMetaUpgrade;

    private void Awake()
    {
        // SetTowerMetaUpgradeSaveManager(null);

        if(lobbyUICtr != null)
        {
            lobbyUICtr.OnSelectStage += OnSelectStageLevel;
            lobbyUICtr.OnTowerMetaDamageUpgrade += TowerMetaDamageUpgrade;
            lobbyUICtr.OnTowerMetaAttackSpeedUpgrade += TowerMetaAttackSpeedUpgrade;
        }        
    }

    private void Start()
    {
        towerMetaUpgrade = Managers.TowerMetaUpgrade;
    }

    private void OnDestroy()
    {
        if(lobbyUICtr != null)
        {
            lobbyUICtr.OnSelectStage -= OnSelectStageLevel;
            lobbyUICtr.OnTowerMetaDamageUpgrade -= TowerMetaDamageUpgrade;
            lobbyUICtr.OnTowerMetaAttackSpeedUpgrade -= TowerMetaAttackSpeedUpgrade;
        }
    }

    private void OnSelectStageLevel(string level)
    {
        Managers.Game.SelectStageDifficultyLevel(level);

        LoadSceneManager.Instance.OnLoadStageScene();
    }

    private void TowerMetaDamageUpgrade(TowerType type, int grade, int upValue)
    {
        towerMetaUpgrade.TowerDamageUpgrade(type, grade, upValue);

        int newLevel = towerMetaUpgrade.GetDamageLevel(type, grade);

        lobbyUICtr.OnChangeTowerMetaDamageUpgradeLevel(type, grade, newLevel);
    }

    private void TowerMetaAttackSpeedUpgrade(TowerType type, int grade, int upValue)
    {
        towerMetaUpgrade.TowerAttackSpeedUpgrade(type, grade, upValue);
        
        int newLevel = towerMetaUpgrade.GetAttakSpeedLevel(type, grade);

        lobbyUICtr.OnChangeTowerMetaAttackSpeedUpgradeLevel(type, grade, newLevel);
    }

    public void SetTowerMetaUpgradeSaveManager(TowerMetaUpgradeData data)
    {
        towerMetaUpgrade.Init(data);
    }
}
