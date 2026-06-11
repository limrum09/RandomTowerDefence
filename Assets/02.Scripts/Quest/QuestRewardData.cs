using UnityEngine;

public enum QuestRewardType
{
    Gold,
    Item,
    Exp
}

[System.Serializable]
public class QuestRewardData
{
    [SerializeField]
    private QuestRewardType rewardType;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    [HideIf(nameof(rewardType), (int)QuestRewardType.Gold)]
    [HideIf(nameof(rewardType), (int)QuestRewardType.Exp)]
    private string rewardUID;
    
    [SerializeField]
    private int cnt;

    public Sprite Icon => icon;
    public int RewardCount => cnt;
    public void Give()
    {

    }
}
