using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerActionMenuView : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private Button moveBtn;
    [SerializeField]
    private Button gradeUpgradeBtn;
    [SerializeField]
    private Button statUpgradeBtn;
    [SerializeField]
    private Button toQueueSlotBtn;
    [SerializeField]
    private TextMeshProUGUI towerMoveBtnText;
    [SerializeField]
    private TextMeshProUGUI towerGradeupGradeBtnText;
    [SerializeField]
    private TextMeshProUGUI towerStatUpgradeBtnText;
    [SerializeField]
    private TextMeshProUGUI toQueueSlotBtnText;

    public void Show()
    {
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        towerMoveBtnText.text = $"{Managers.Local.GetString("OptionUI", "INPUT_ACTION_" + InputAction.MoveTower.ToString().ToUpper())}({Managers.InputData.GetKeyCode(InputAction.MoveTower)})";
        towerGradeupGradeBtnText.text = $"{Managers.Local.GetString("OptionUI", "INPUT_ACTION_" + InputAction.ShowGradeUpgradeTowerView.ToString().ToUpper())}({Managers.InputData.GetKeyCode(InputAction.ShowGradeUpgradeTowerView)})";
        towerStatUpgradeBtnText.text = $"{Managers.Local.GetString("OptionUI", "INPUT_ACTION_" + InputAction.ShowStatUpgradeTowerView.ToString().ToUpper())}({Managers.InputData.GetKeyCode(InputAction.ShowStatUpgradeTowerView)})";
        toQueueSlotBtnText.text = $"{Managers.Local.GetString("OptionUI", "INPUT_ACTION_" + InputAction.TowerMoveToQueueSlot.ToString().ToUpper())} ({Managers.InputData.GetKeyCode(InputAction.TowerMoveToQueueSlot)})"; 
    }
    public void Hide()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    public void BindMove(UnityAction action) => moveBtn.onClick.AddListener(action);
    public void BindGradeUpgrade(UnityAction action) => gradeUpgradeBtn.onClick.AddListener(action);
    public void BindStatUpgrade(UnityAction action) => statUpgradeBtn.onClick.AddListener(action);
    public void BindTowerMoveToQueueSlot(UnityAction action) => toQueueSlotBtn.onClick.AddListener(action); 
}
