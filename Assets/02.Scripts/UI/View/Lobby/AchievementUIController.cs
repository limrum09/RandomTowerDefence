using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUIController : MonoBehaviour
{
    public enum AchievementFilter
    {
        All,
        Active,
        Complete
    }

    [Header("Info")]
    [SerializeField]
    private TextMeshProUGUI progressText;
    [SerializeField]
    private TextMeshProUGUI progressPerText;
    [SerializeField]
    private Image progressBar;

    [Header("Toggles")]
    [SerializeField]
    private Toggle allToggle;
    [SerializeField]
    private Toggle activeToggle;
    [SerializeField]
    private Toggle completedToggle;

    [Header("Interaction")]
    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private Transform content;
    [SerializeField]
    private AchievementUISlot slotPrefab;

    private readonly List<AchievementUISlot> slotPool = new List<AchievementUISlot>();
    private AchievementFilter currentFilter = AchievementFilter.All;

    private void Awake()
    {
        allToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                SetFilter(AchievementFilter.All);
        });

        activeToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                SetFilter(AchievementFilter.Active);
        });

        completedToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
                SetFilter(AchievementFilter.Complete);
        });
    }

    private void Start()
    {
        Hide();
    }

    private void SetFilter(AchievementFilter filter)
    {
        currentFilter = filter;
        Refresh();
    }

    private List<Quest> GetSortActiveAchievements()
    {
        return Managers.QuestMgr.ActiveAchievement.Where(achievement => achievement.IsConditionComplete)
            .OrderBy(achievement => achievement.Difficulty).ThenBy(achievement => achievement.QuestUID).ToList();
    }

    private List<Quest> GetSortCompletedAchievements()
    {
        return Managers.QuestMgr.CompleteAchievement.Where(achievement => achievement.IsConditionComplete)
            .OrderBy(achievement => achievement.Difficulty).ThenBy(achievement => achievement.QuestUID).ToList();
    }

    private List<Quest> GetDisplayAchievements()
    {
        List<Quest> result = new List<Quest>();

        if(currentFilter == AchievementFilter.All || currentFilter == AchievementFilter.Active)
        {
            result.AddRange(GetSortActiveAchievements());
        }

        if(currentFilter == AchievementFilter.All || currentFilter == AchievementFilter.Complete)
        {
            result.AddRange(GetSortCompletedAchievements());
        }

        return result;
    }

    private void Refresh()
    {
        HideAllSlots();

        List<Quest> displayAchievement = GetDisplayAchievements();

        for(int i = 0; i < displayAchievement.Count; i++)
        {
            AchievementUISlot slot = GetSlot(i);
            slot.SetAchievementUISlot(displayAchievement[i]);
            slot.gameObject.SetActive(true);
        }

        int completeCnt = Managers.QuestMgr.CompleteAchievement.Count;
        int totalCnt = Managers.QuestMgr.ActiveAchievement.Count + completeCnt;
        int percent = totalCnt <= 0 ? 0 : Mathf.RoundToInt((float)completeCnt / totalCnt * 100);

        progressText.text = $"{completeCnt} / {totalCnt}";
        progressPerText.text = $"{percent}%";
        progressBar.fillAmount = totalCnt <= 0 ? 0f : (float)completeCnt / totalCnt;
    }

    private AchievementUISlot GetSlot(int index)
    {
        if(slotPool.Count <= index)
        {
            AchievementUISlot slot = Instantiate(slotPrefab, content);
            slotPool.Add(slot);
        }

        return slotPool[index];
    }

    private void HideAllSlots()
    {
        foreach(AchievementUISlot slot in slotPool)
        {
            slot.gameObject.SetActive(false);
        }
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
