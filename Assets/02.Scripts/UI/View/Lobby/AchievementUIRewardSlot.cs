using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUIRewardSlot : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI countText;

    public void SetReward(QuestRewardData reward)
    {
        if(reward == null)
        {
            icon.sprite = null;
            icon.gameObject.SetActive(false);
            countText.text = string.Empty;
            return;
        }

        icon.gameObject.SetActive(true);
        icon.sprite = reward.Icon != null ? reward.Icon : null;
        countText.text = reward.RewardCount.ToString();
    }
}
