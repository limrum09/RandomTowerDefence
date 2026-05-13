using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeView : MonoBehaviour
{
    [Header("User Info")]
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private TextMeshProUGUI metaCurrenyText;

    [Header("Option Toggles")]
    [SerializeField]
    private Toggle publicTypeToggle;
    [SerializeField]
    private Toggle towerTypeToggle;

    [Header("Tower Toggles")]
    [SerializeField]
    private Toggle humanToggle;
    [SerializeField]
    private Toggle elfToggle;
    [SerializeField]
    private Toggle orcToggle;
    [SerializeField]
    private Toggle dwarfToggle;
    [SerializeField]
    private Toggle dragonianToggle;
    [SerializeField]
    private Toggle wereBeastToggle;

    [Header("Select Option")]
    [SerializeField]
    private List<MetaUpgradeSelectView> selectViews;

    [Header("Upgrade View")]
    [SerializeField]
    private MetaUpgradeInfoView infoView;

    private void Awake()
    {
        BindTowerToggle(humanToggle, TowerType.Human);
        BindTowerToggle(elfToggle, TowerType.Elf);
        BindTowerToggle(orcToggle, TowerType.Orc);
        BindTowerToggle(dwarfToggle, TowerType.Dwarf);
        BindTowerToggle(dragonianToggle, TowerType.Dragonian);
        BindTowerToggle(wereBeastToggle, TowerType.Werebeast);
    }

    private void BindTowerToggle(Toggle toggle, TowerType type)
    {
        toggle.onValueChanged.AddListener(isOn =>
        {
            if (!isOn)
                return;

            OnClickTowerToggle(type);
        });
    }
    private void OnMetaUpgradeInfoView()
    {

    }
    public void OnClickPublicToggle()
    {

    }

    public void OnClickPublicButton(string type)
    {

    }

    public void OnClickTowerToggle(TowerType type)
    {

    }

    public void OnClickTowerButton(TowerType type, int grade)
    {

    }
}
