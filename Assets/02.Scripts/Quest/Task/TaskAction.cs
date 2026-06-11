public static class TaskAction
{
    public static int Run(TaskActionType actionType, int current, int prev)
    {
        switch (actionType)
        {
            case TaskActionType.Set:
                return current;
            case TaskActionType.Add:
                return prev + current;
            case TaskActionType.ContinuePositive:
                return current > 0 ? prev + current : 0;
            default:
                return prev;
        }
    }
}
