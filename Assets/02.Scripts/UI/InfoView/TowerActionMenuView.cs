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

    public void Show() => root.SetActive(true);
    public void Hide() => root.SetActive(false);

    public void BindMove(UnityAction action) => moveBtn.onClick.AddListener(action);
    public void BindGradeUpgrade(UnityAction action) => gradeUpgradeBtn.onClick.AddListener(action);
}
