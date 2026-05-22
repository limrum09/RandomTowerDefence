using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTarget", menuName = "Quest/Task/EnemyTarget")]
public class EnemyTarget : TaskTarget
{
    [SerializeField]
    private string enemyUID;
    public override object Value => enemyUID;

    public override bool IsEqual(object getTarget)
    {
        if(getTarget == null) 
            return false;

        if(getTarget is Enemy target)
        {
            return string.Equals(enemyUID, target.EnemyUID);
        }

        if(getTarget is string targetUID)
        {
            return string.Equals(enemyUID, targetUID);
        }

        return false;
    }
}
