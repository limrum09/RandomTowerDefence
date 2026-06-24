using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class AchievementUISlot : MonoBehaviour
{
    [Header("Info")]
    [SerializeField]
    private LocalizeStringEvent title;
    [SerializeField]
    private LocalizeStringEvent description;
    [SerializeField]
    private Image currentProgressBar;
    [SerializeField]
    private TextMeshProUGUI currentProgressText;

    [Header("Rewards")]
    [SerializeField]
    private AchievementUIRewardSlot[] rewardSlots;

    private Quest currentAchievement;

    private void Awake()
    {
        title.OnUpdateString.AddListener(SetTitleAddDifficulty);
    }

    private void OnDestroy()
    {
        title.OnUpdateString.RemoveListener(SetTitleAddDifficulty);
    }

    private void SetTitleAddDifficulty(string localtitle)
    {
        if (currentAchievement == null)
            return;

        TextMeshProUGUI titleText = title.gameObject.GetComponent<TextMeshProUGUI>();

        if (titleText == null)
            return;

        titleText.text = $"[{currentAchievement.Difficulty}] {localtitle}";
    }

    private void SetLocalString(LocalizeStringEvent localEvent, string key)
    {
        localEvent.StringReference.TableReference = "Quest";
        localEvent.StringReference.TableEntryReference = key;
        localEvent.RefreshString();
    }

    public void SetAchievementUISlot(Quest achievement)
    {
        currentAchievement = achievement;

        SetLocalString(title, currentAchievement.QuestUID);
        SetLocalString(description, currentAchievement.Task.TaskUId);

        int current = currentAchievement.Task.CurrentSuccess;
        int need = currentAchievement.Task.NeedSuccessCount;

        currentProgressBar.fillAmount = need <= 0 ? 0f : (float)current / need;
        currentProgressText.text = $"{current} / {need}";

        int rewardCount = currentAchievement.QuestRewards.Count;

        for(int i = 0; i < rewardSlots.Length; i++)
        {
            if (rewardCount > i)
            {
                rewardSlots[i].SetReward(currentAchievement.QuestRewards[i]);
            }
            else
                rewardSlots[i].SetReward(null);
        }

        this.gameObject.SetActive(currentAchievement.IsConditionComplete);
    }
}
