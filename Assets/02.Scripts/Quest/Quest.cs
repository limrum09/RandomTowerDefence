using System;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStat
{
    Running,
    Comoplete
}

[CreateAssetMenu(fileName = "Quest_", menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public event Action<Quest> OnQuestComplete;

    [Header("Quest Info")]
    [SerializeField]
    private Category questCategory;
    [SerializeField]
    private string questUID;
    [SerializeField]
    private Task task;

    [Header("Reward")]
    [SerializeField]
    private QuestReward[] rewards;

    [Header("Option")]
    [SerializeField]
    private bool isSaveable;    // 업적에서만 사용

    public Category QuestCategory => questCategory;
    public string QuestUID => questUID;
    public Task Task => task;
    public QuestReward[] Rewards => rewards;
    public QuestStat Stat { get; set; }
    public IReadOnlyList<QuestReward> QuestRewards => rewards;
    public bool IsQuestComplete => Stat == QuestStat.Comoplete;
    public virtual bool IsSaveable => isSaveable;
    
    public void QuestOnRegister()
    {
        if (Stat == QuestStat.Comoplete)
            return;

        task.OnTaskCompleted += QuestComplete;        

        Stat = QuestStat.Running;
    }

    public void QuestRecieveReport(string category, object target, int getSucessCount)
    {
        if (IsQuestComplete)
            return;

        if (task.IsCompleted)
        {
            Stat = QuestStat.Comoplete;
            return;
        }
            
        task.TaskRecieveReport(category, target, getSucessCount);
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
        clone.task = Instantiate(task);
        return clone;
    }

    public QuestSaveData GetSaveData()
    {
        return new QuestSaveData
        {
            UID = questUID,
            stat = Stat,
            currentSuccess = task.CurrentSuccess
        };
    }

    public void LoadQuestSaveData(QuestSaveData saveData)
    {
        Stat = saveData.stat;

        task.LoadTaskSuccess(saveData.currentSuccess);
    }
}
