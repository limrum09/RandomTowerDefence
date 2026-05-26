using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "_QuestReward", menuName = "Quest/Reward")]
public class QuestReward : ScriptableObject
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private string rewardUID;
    [SerializeField]
    private int cnt;

    public Sprite Icon => icon.sprite;
    public int RewardCount => cnt;
    public void Give()
    {

    }
}
