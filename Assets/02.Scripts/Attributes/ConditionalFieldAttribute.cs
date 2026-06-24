using System;
using UnityEngine;

public enum ConditionMode
{
    ShowIf,
    HideIf,
    EnableIf,
    DisableIf,
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class ConditionFieldAtrribute : PropertyAttribute
{
    public string ConditionName { get; }
    public object CompareValue { get; }
    public ConditionMode Mode { get; }

    public ConditionFieldAtrribute(string conditionName, ConditionMode mode)
    {
        ConditionName = conditionName;
        Mode = mode;
    }

    public ConditionFieldAtrribute(string conditionName, ConditionMode mode, object compareValue)
    {
        ConditionName = conditionName;
        CompareValue = compareValue;
        Mode = mode;
    }
}

public class ShowIfAttribute : ConditionFieldAtrribute
{
    public ShowIfAttribute(string conditionName) : base(conditionName, ConditionMode.ShowIf) { }
    public ShowIfAttribute(string conditionName, object compareValue) : base(conditionName, ConditionMode.ShowIf, compareValue) { }
}

public class HideIfAttribute : ConditionFieldAtrribute
{
    public HideIfAttribute(string conditionName) : base(conditionName, ConditionMode.HideIf) { }
    public HideIfAttribute(string conditionName, object compareValue) : base(conditionName, ConditionMode.HideIf, compareValue) { }
}

public class EnablefAttribute : ConditionFieldAtrribute
{
    public EnablefAttribute(string conditionName) : base(conditionName, ConditionMode.EnableIf) { }
    public EnablefAttribute(string conditionName, object compareValue) : base(conditionName, ConditionMode.EnableIf, compareValue) { }
}

public class DisablefAttribute : ConditionFieldAtrribute
{
    public DisablefAttribute(string conditionName) : base(conditionName, ConditionMode.DisableIf) { }
    public DisablefAttribute(string conditionName, object compareValue) : base(conditionName, ConditionMode.DisableIf, compareValue) { }
}
