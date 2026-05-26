using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class AchievementContentView : MonoBehaviour
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
    private Image reward1Icon;
    [SerializeField]
    private TextMeshProUGUI reward1Count;
    [SerializeField]
    private Image reward2Icon;
    [SerializeField]
    private TextMeshProUGUI reward2Count;

    public void SetModel(string questUID, string taskUID, int currentProgress, 
        int needProgress, QuestReward reward1 = null, QuestReward reward2 = null)
    {
        title.SetTable(questUID);
        description.SetTable(taskUID);

        float fill = (float)currentProgress / needProgress;
        currentProgressBar.fillAmount = fill;

        currentProgressText.text = $"{currentProgress} / {needProgress}";

        if(reward1 == null)
        {
            reward1Count.text = string.Empty;
            reward1Icon.gameObject.SetActive(false);
            reward2Count.text = string.Empty;
            reward2Icon.gameObject.SetActive(false);
            return;
        }

        reward1Icon.sprite = reward1.Icon;
        reward1Count.text = reward1.RewardCount.ToString();

        if(reward2 == null)
        {
            reward2Count.text = string.Empty;
            reward2Icon.gameObject.SetActive(false);
            return;
        }

        reward1Icon.sprite = reward2.Icon;
        reward1Count.text = reward2.RewardCount.ToString();
    }
}
