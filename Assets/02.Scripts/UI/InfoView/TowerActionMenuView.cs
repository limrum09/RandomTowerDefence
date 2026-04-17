using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerActionMenuView : MonoBehaviour
{
    [SerializeField]
    private GameObject root;
    [SerializeField]
    private Button moveBtn;
    [SerializeField]
    private Button gradeUpgradeBtn;
    [SerializeField]
    private Button statUpgradeBtn;
    [SerializeField]
    private TextMeshProUGUI towerMoveBtnText;
    [SerializeField]
    private TextMeshProUGUI towerGradeupGradeBtnText;
    [SerializeField]
    private TextMeshProUGUI towerStatUpgradeBtnText;

    public void Show()
    {
        towerMoveBtnText.text = $"타워이동({Managers.InputKey.GetKeyCode(InputAction.MoveTower)})";
        towerGradeupGradeBtnText.text = $"타워 정보 보기({Managers.InputKey.GetKeyCode(InputAction.ShowGradeUpgradeTowerView)})";
        towerStatUpgradeBtnText.text = $"타워 스탯 강화({Managers.InputKey.GetKeyCode(InputAction.ShowStatUpgradeTowerView)})";

        root.SetActive(true);
    }
    public void Hide() => root.SetActive(false);

    public void BindMove(UnityAction action) => moveBtn.onClick.AddListener(action);
    public void BindGradeUpgrade(UnityAction action) => gradeUpgradeBtn.onClick.AddListener(action);
    public void BindStatUpgrade(UnityAction action) => statUpgradeBtn.onClick.AddListener(action);
}
