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

    private void SetLocalString(LocalizeStringEvent localEvent, string key)
    {
        localEvent.StringReference.TableReference = "Quest";
        localEvent.StringReference.TableEntryReference = key;
        localEvent.RefreshString();
    }
    public void SetAchievementUISlot(Quest achievement)
    {
        SetLocalString(title, achievement.QuestUID);
        SetLocalString(description, achievement.Task.TaskUId);

        int current = achievement.Task.CurrentSuccess;
        int need = achievement.Task.NeedSuccessCount;

        currentProgressBar.fillAmount = need <= 0 ? 0f : (float)current / need;
        currentProgressText.text = $"{current} / {need}";

        int rewardCount = achievement.QuestRewards.Count;

        for(int i = 0; i < rewardSlots.Length; i++)
        {
            if (rewardCount > i)
            {
                rewardSlots[i].SetReward(achievement.QuestRewards[i]);
            }
            else
                rewardSlots[i].SetReward(null);
        }

        this.gameObject.SetActive(achievement.IsConditionComplete);
    }
}
