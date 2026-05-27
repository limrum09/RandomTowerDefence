using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "_QuestReward", menuName = "Quest/Reward")]
public class QuestReward : ScriptableObject
{
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private string rewardUID;
    [SerializeField]
    private int cnt;

    public Sprite Icon => icon;
    public int RewardCount => cnt;
    public void Give()
    {

    }
}
