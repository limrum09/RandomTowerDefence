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

[CreateAssetMenu(fileName = "Task_", menuName = "Quest/Task/Task")]
public class QuestTask : ScriptableObject
{
    public event Action OnTaskCompleted;
    public event Action<QuestTask, TaskStat, TaskStat> OnChangedStat;
    public event Action<QuestTask, int, int> OnChangedSuccessCount;

    [SerializeField]
    private Category category;
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

    private TaskAction taskAction = new TaskAction();
    private int currentSuccess;
    private TaskStat stat;

    public Category TaskCategory => category;
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

    public void TaskRecieveReport(string category, object target, int successCount)
    {
        if (IsCompleted)
            return;

        if (TaskCategory.CategoryUID != category)
            return;

        if (!TaskContainsTarget(target))
            return;

        CurrentSuccess = taskAction.Run(actionType, currentSuccess, successCount);

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

    public bool TaskContainsTarget(object target) => targets.Any(x => x.IsEqual(target));
}
