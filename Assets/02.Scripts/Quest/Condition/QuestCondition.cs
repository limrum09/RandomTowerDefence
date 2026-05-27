using UnityEngine;

public abstract class QuestCondition : ScriptableObject
{
    [SerializeField]
    private string description;

    public abstract bool IsPass();
}
