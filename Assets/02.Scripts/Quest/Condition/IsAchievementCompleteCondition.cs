using UnityEngine;

[CreateAssetMenu(fileName = "AchievementComplete_Condition_", menuName = "Quest/Condition/AchievementComplete")]
public class IsAchievementCompleteCondition : QuestCondition
{
    [SerializeField]
    private Achievement achievement;

    public string QuestUID => achievement.QuestUID;
    public override bool IsPass()
    {
        return Managers.QuestMgr.ContainsCompletedAchievement(achievement);
    }
}
