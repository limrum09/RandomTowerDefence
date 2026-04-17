using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TowerUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private TowerController towerCtr;
    [SerializeField]
    private TowerGradeUpgradeView gradeUpgradeView;
    [SerializeField]
    private TowerStatUpgradeView statUpgradeView;
    [SerializeField]
    private TowerActionMenuView actionMenuView;

    private TowerGradeUpgradePresenter gradePresenter;
    private TowerActionMenuPresenter actionMenuPresenter;
    private TowerStatUpgradePresernter statPresenter;

    private Tower selectedTower;

    private void Awake()
    {
        gradePresenter = new TowerGradeUpgradePresenter(gradeUpgradeView);
        actionMenuPresenter = new TowerActionMenuPresenter(actionMenuView);
        statPresenter = new TowerStatUpgradePresernter(statUpgradeView);

        gradePresenter.onClickNormalUpgrade += OnTowerGradeNormalUpgrade;
        gradePresenter.onClickPremiumUpgrade += OnTowerGradePreminumUpgrade;

        actionMenuPresenter.OnClickMove += OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade += OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade += OnClickStatUpgrade;

        statPresenter.onClickDamageUpgrade += OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade += OnTowerStatAttackSpeedUpgrade;

        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
    }

    private void OnDestroy()
    {
        gradePresenter.onClickNormalUpgrade -= OnTowerGradeNormalUpgrade;
        gradePresenter.onClickPremiumUpgrade -= OnTowerGradePreminumUpgrade;

        actionMenuPresenter.OnClickMove -= OnClickMove;
        actionMenuPresenter.OnClickGradeUpgrade -= OnClickGradeUpgrade;
        actionMenuPresenter.OnClickStatUpgrade -= OnClickStatUpgrade;

        statPresenter.onClickDamageUpgrade -= OnTowerStatDamageUpgrade;
        statPresenter.onClickAttackSpeedUpgrade -= OnTowerStatAttackSpeedUpgrade;
    }

    public void SetSelectedTower(Tower getTower)
    {
        selectedTower = getTower;
        
        actionMenuPresenter.SetModel(selectedTower);
        gradePresenter.HideModel();
        statPresenter.Hide();
    }

    public void ClearSelection()
    {
        selectedTower = null;
        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();
    }

    private void OnClickMove()
    {
        if (selectedTower == null)
            return;

        gradePresenter.HideModel();
        actionMenuPresenter.Hide();
        statPresenter.Hide();

        towerCtr.SetTowerMoveMode();
    }

    public void OnClickGradeUpgrade(Tower tower)
    {
        if(tower == null) 
            return;

        gradePresenter.SetModel(tower);
        statPresenter.Hide();
    }

    public void OnClickStatUpgrade(Tower tower)
    {
        if (tower == null)
            return;

        gradePresenter.HideModel();
        statPresenter.SetModel(tower);
    }

    private void OnTowerGradeNormalUpgrade()
    {
        if (towerCtr == null)
            return;

        towerCtr.TowerGradeNormalUpgrade();
    }

    private void OnTowerGradePreminumUpgrade()
    {
        if (towerCtr == null)
            return;

        towerCtr.TowerGradePreminumUpgrade();
    }

    private void OnTowerStatDamageUpgrade()
    {

    }

    private void OnTowerStatAttackSpeedUpgrade()
    {

    }
}
