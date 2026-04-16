using UnityEngine;
using UnityEngine.UI;

public class TowerUIController : MonoBehaviour
{
    [Header("Action Menu")]
    [SerializeField]
    private GameObject actionMenuRoot;
    [SerializeField]
    private Button moveBtn;
    [SerializeField]
    private Button gradeUpgradeBtn;

    [Header("References")]
    [SerializeField]
    private TowerController towerCtr;
    [SerializeField]
    private TowerGradeUpgradeView gradeUpgradeView;
    [SerializeField]
    private TowerActionMenuView actionMenuView;


    private TowerGradeUpgradePresenter gradePresenter;
    private TowerActionMenuPresenter actionMenuPresenter;

    private void Awake()
    {
        gradePresenter = new TowerGradeUpgradePresenter(gradeUpgradeView, towerCtr);
        actionMenuPresenter = new TowerActionMenuPresenter(actionMenuView);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
