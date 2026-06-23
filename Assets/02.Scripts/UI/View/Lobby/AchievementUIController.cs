using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUIController : MonoBehaviour
{
    [Header("Info")]
    [SerializeField]
    private TextMeshProUGUI progressText;
    [SerializeField]
    private TextMeshProUGUI progressPerText;
    [SerializeField]
    private Image progressBar;

    [Header("Interaction")]
    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private Transform content;
    [SerializeField]
    private AchievementUISlot slotPrefab;

    private void Start()
    {
        Hide();
    }

    private void Refresh()
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest achievement in Managers.QuestMgr.ActiveAchievement)
        {
            AchievementUISlot slot = Instantiate(slotPrefab, content);
            slot.SetAchievementUISlot(achievement);
        }

        foreach (Quest achievement in Managers.QuestMgr.CompleteAchievement)
        {
            AchievementUISlot slot = Instantiate(slotPrefab, content);
            slot.SetAchievementUISlot(achievement);
        }

        int completeCnt = Managers.QuestMgr.CompleteAchievement.Count;
        int totalCnt = Managers.QuestMgr.ActiveAchievement.Count + completeCnt;
        int percent = totalCnt <= 0 ? 0 : Mathf.RoundToInt((float)completeCnt / totalCnt * 100);

        progressText.text = $"{completeCnt} / {totalCnt}";
        progressPerText.text = $"{percent}%";
        progressBar.fillAmount = totalCnt <= 0 ? 0f : (float)completeCnt / totalCnt;
    }

    public void Hide()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    public void Show()
    {
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        Refresh();
    }
}
