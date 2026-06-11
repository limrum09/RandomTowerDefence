using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStat
{
    Running,
    Comoplete
}

public enum QuestCategory
{
    KillEnemy,
    ClearStage,
    CollectItem,
    UpgradeTower,
    BuildTower,
    Achievement
}

[CreateAssetMenu(fileName = "Quest_", menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public event Action<Quest> OnQuestComplete;

    [Header("Quest Info")]
    [SerializeField]
    private QuestCategory questCategory;
    [SerializeField]
    private string questUID;
    [SerializeField]
    private QuestTaskData task;
    [SerializeField]
    private QuestCondition condition;

    [Header("Reward")]
    [SerializeField]
    private QuestRewardData[] rewards;

    [Header("Option")]
    [SerializeField]
    private bool isSaveable;    // 업적에서만 사용

    public QuestCategory Category => questCategory;
    public string QuestUID => questUID;
    public QuestTaskData Task => task;
    public QuestStat Stat { get; set; }
    public IReadOnlyList<QuestRewardData> QuestRewards => rewards;
    public bool IsQuestComplete => Stat == QuestStat.Comoplete;
    public bool IsConditionComplete => condition != null ? condition.IsPass() : true;
    public virtual bool IsSaveable => isSaveable;

#if UNITY_EDITOR
    public void EditorSetUID(string uid)
    {
        questUID = uid;
    }

    public void EditorSetTaskUID(string uid)
    {
        if (task == null)
            task = new QuestTaskData();

        task.EditorSetUID(uid);
    }
#endif

    public void QuestOnRegister()
    {
        if (Stat == QuestStat.Comoplete)
            return;

        task.OnTaskCompleted += QuestComplete;        

        Stat = QuestStat.Running;
    }

    public void QuestRecieveReport(QuestCategory category, object target, int getSucessCount)
    {
        if (IsQuestComplete)
            return;

        if (!IsConditionComplete)
            return;

        if (task.IsCompleted)
        {
            Stat = QuestStat.Comoplete;
            return;
        }

        task.TaskRecieveReport(category, target, getSucessCount);
        Managers.Save.MarkQuestDirty();
    }

    public void QuestComplete()
    {
        if (Stat == QuestStat.Comoplete)
            return;

        Stat = QuestStat.Comoplete;

        foreach(var reward in rewards)
        {
            reward.Give();
        }

        task.TaskCompleted();

        OnQuestComplete?.Invoke(this);
        OnQuestComplete = null;
    }

    public void QuestContains(object target) => task.TaskContainsTarget(target);
    public void QuestContains(TaskTarget target) => QuestContains(target.Value);
    
    public Quest Clone()
    {
        Quest clone = Instantiate(this);
        clone.task = task.Clone();
        return clone;
    }

    public QuestSaveData GetSaveData()
    {
        return new QuestSaveData
        {
            UID = questUID,
            stat = (int)Stat,
            currentSuccess = task.CurrentSuccess
        };
    }

    public void LoadQuestSaveData(QuestSaveData saveData)
    {
        Stat = (QuestStat)saveData.stat;

        task.LoadTaskSuccess(saveData.currentSuccess);
    }
}
