using UnityEngine;

[CreateAssetMenu(fileName = "UIDTarget", menuName = "Quest/Task/Target_")]
public class UIDTarget : TaskTarget
{
    [SerializeField]
    private string uid;

    public override object Value => uid;

    public override bool IsEqual(object target)
    {
        if (target == null)
            return false;

        if (target is string targetUID)
            return string.Equals(uid, targetUID);

        if (target is TaskTarget taskTarget)
            return string.Equals(uid, taskTarget.Value as string);

        return false;
    }
}
