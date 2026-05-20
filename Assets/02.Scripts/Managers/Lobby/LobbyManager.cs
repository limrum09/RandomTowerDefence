using System;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField]
    private LobbyUIController lobbyUICtr;


    public event Action onChangedResearchLevel;
    private void Awake()
    {
        // SetTowerMetaUpgradeSaveManager(null);

        if(lobbyUICtr != null)
        {
            lobbyUICtr.OnSelectStage += OnSelectStageLevel;
            lobbyUICtr.OnMetaUpgrade += OnMetaUpgrade;

            onChangedResearchLevel += lobbyUICtr.OnChangedResearchLevel;
        }        
    }

    private void OnDestroy()
    {
        if(lobbyUICtr != null)
        {
            lobbyUICtr.OnSelectStage -= OnSelectStageLevel;
            lobbyUICtr.OnMetaUpgrade -= OnMetaUpgrade;

            onChangedResearchLevel -= lobbyUICtr.OnChangedResearchLevel;
        }
    }

    public bool OnMetaUpgrade(MetaUpgradeTarget metaType, MetaUpgradeType upgradeType, string uid, int upgradeCost, int upValue)
    {
        bool isSuccess = false;

        if (metaType == MetaUpgradeTarget.Tower)
        {
            TowerData data = Managers.TowerData.GetTowerData(uid);

            if (!Managers.Player.UseCurrency(upgradeCost))
                return false;

            if (upgradeType == MetaUpgradeType.Damage)
                isSuccess = Managers.TowerMetaUpgrade.TowerDamageUpgrade(data.towerType, data.grade, upValue);
            else if (upgradeType == MetaUpgradeType.AttackSpeed)
                isSuccess = Managers.TowerMetaUpgrade.TowerAttackSpeedUpgrade(data.towerType, data.grade, upValue);
        }
        else if(metaType == MetaUpgradeTarget.Public)
        {
            if (!Managers.Player.UseCurrency(upgradeCost))
                return false;

            if (Managers.PublicMetaUpgrade.GetPublicMetaType(uid, out MetaUpgradeType publicType))
                isSuccess = Managers.PublicMetaUpgrade.PublicMetaUpgrade(publicType, upValue);
        }

        if (isSuccess)
        {
            if(Managers.Player.AddExp(upgradeCost / 100))
            {
                onChangedResearchLevel?.Invoke();
            }
        }
            

        return isSuccess;
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
