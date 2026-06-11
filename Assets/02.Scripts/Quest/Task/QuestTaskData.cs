using System;
using System.Linq;
using UnityEngine;

public enum TaskActionType
{
    Set,
    Add,
    ContinuePositive
}

public enum TaskStat
{
    Running,
    Complete
}

[Serializable]
public class QuestTaskData
{
    public event Action OnTaskCompleted;
    public event Action<QuestTaskData, TaskStat, TaskStat> OnChangedStat;
    public event Action<QuestTaskData, int, int> OnChangedSuccessCount;

    [SerializeField]
    private QuestCategory category;
    [SerializeField]
    private string taskUID;
    [SerializeField]
    private TaskTarget[] targets;
    [SerializeField]
    private TaskActionType actionType;
    [SerializeField]
    private int needSuccessCount;
    [SerializeField]
    private bool countingAfterComplete;

    private int currentSuccess;
    private TaskStat stat;

    public QuestCategory TaskCategory => category;
    public string TaskUId => taskUID;
    public int NeedSuccessCount => needSuccessCount;
    public bool CountingfterCompleted => countingAfterComplete;
    public bool IsCompleted => stat == TaskStat.Complete;


    public int CurrentSuccess
    {
        get { return currentSuccess; }
        set
        {
            int prevSuccees = currentSuccess;

            currentSuccess = Mathf.Clamp(value, 0, needSuccessCount);

            if (prevSuccees == currentSuccess)
                return;

            stat = currentSuccess == needSuccessCount ? TaskStat.Complete : TaskStat.Running;
            OnChangedSuccessCount?.Invoke(this, currentSuccess, prevSuccees);
        }
    }

    public TaskStat Stat
    {
        get => stat;
        set
        {
            var prevStat = stat;
            stat = value;
            OnChangedStat?.Invoke(this, stat, prevStat);
        }
    }

    public void TaskRecieveReport(QuestCategory category, object target, int successCount)
    {
        if (IsCompleted)
            return;

        if (TaskCategory != category)
            return;

        if (!TaskContainsTarget(target))
            return;

        CurrentSuccess = TaskAction.Run(actionType, currentSuccess, successCount);

        if (IsCompleted)
            OnTaskCompleted?.Invoke();
    }

    public void TaskStart()
    {
        stat = TaskStat.Running;
    }

    public void TaskEnd()
    {
        OnChangedSuccessCount = null;
        OnChangedStat = null;
        OnTaskCompleted = null;
    }

    public void TaskCompleted()
    {
        CurrentSuccess = needSuccessCount;

        stat = TaskStat.Complete;

        TaskEnd();
    }

    public void LoadTaskSuccess(int success)
    {
        currentSuccess = Mathf.Clamp(success, 0, needSuccessCount);
        stat = currentSuccess == needSuccessCount ? TaskStat.Complete : TaskStat.Running;

        if (IsCompleted)
            TaskEnd();
    }

    public QuestTaskData Clone()
    {
        return new QuestTaskData
        {
            category = this.category,
            taskUID = this.taskUID,
            targets = this.targets,
            actionType = this.actionType,
            needSuccessCount = this.needSuccessCount,
            countingAfterComplete = this.countingAfterComplete,
            currentSuccess = 0,
            stat = TaskStat.Running
        };
    }

#if UNITY_EDITOR
    public void EditorSetUID(string uid)
    {
        taskUID = uid;
    }
#endif

    public bool TaskContainsTarget(object target) => targets.Any(x => x.IsEqual(target));
}
