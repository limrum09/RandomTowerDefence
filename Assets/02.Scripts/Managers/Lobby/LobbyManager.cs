using System;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private LobbyUIController lobbyUICtr;

    

    private void Awake()
    {
        // SetTowerMetaUpgradeSaveManager(null);

        if(lobbyUICtr != null)
        {
            lobbyUICtr.OnSelectStage += OnSelectStageLevel;
            lobbyUICtr.OnMetaUpgrade += OnMetaUpgrade;
        }        
    }

    private void OnDestroy()
    {
        if(lobbyUICtr != null)
        {
            lobbyUICtr.OnSelectStage -= OnSelectStageLevel;
            lobbyUICtr.OnMetaUpgrade -= OnMetaUpgrade;
        }
    }

    public bool OnMetaUpgrade(MetaUpgradeTarget metaType, MetaUpgradeType upgradeType, string uid, int upValue)
    {
        if(metaType == MetaUpgradeTarget.Tower)
        {
            TowerData data = Managers.TowerData.GetTowerData(uid);

            if (upgradeType == MetaUpgradeType.Damage)
                return Managers.TowerMetaUpgrade.TowerDamageUpgrade(data.towerType, data.grade, upValue);
            else if (upgradeType == MetaUpgradeType.AttackSpeed)
                return Managers.TowerMetaUpgrade.TowerAttackSpeedUpgrade(data.towerType, data.grade, upValue);
        }
        else if(metaType == MetaUpgradeTarget.Public)
        {
            if(Managers.PublicMetaUpgrade.GetPublicMetaType(uid, out MetaUpgradeType publicType))
                return Managers.PublicMetaUpgrade.PublicMetaUpgrade(publicType, upValue);
        }

        return false;
    }

    private void OnSelectStageLevel(string level)
    {
        Managers.Game.SelectStageDifficultyLevel(level);

        LoadSceneManager.Instance.OnLoadStageScene();
    }

    public void SetTowerMetaUpgradeSaveManager(TowerMetaUpgradeData data)
    {
        Managers.TowerMetaUpgrade.Init(data);
    }
}
