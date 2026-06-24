using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionFieldAtrribute), true)]
public class ConditionalFieldDrawer : PropertyDrawer
{
    private bool ShouldDraw(SerializedProperty property)
    {
        var attr = (ConditionFieldAtrribute)attribute;

        bool condition = EvaluateCondition(property, attr);

        return attr.Mode switch
        {
            ConditionMode.ShowIf => condition,
            ConditionMode.HideIf => !condition,
            _ => true
        };
    }

    private bool EvaluateCondition(SerializedProperty property, ConditionFieldAtrribute attr)
    {
        SerializedProperty conditionProperty = FindConditionProperty(property, attr.ConditionName);

        if (conditionProperty == null)
            return true;

        bool result = GetconditionResult(conditionProperty, attr.CompareValue);

        return result;
    }

    private SerializedProperty FindConditionProperty(SerializedProperty property, string conditionName)
    {
        string propertyPath = property.propertyPath;
        string conditionPath;

        int lastDotIndex = propertyPath.LastIndexOf('.');

        if(lastDotIndex >= 0)
            conditionPath = propertyPath.Substring(0, lastDotIndex + 1) + conditionName;
        else
            conditionPath = conditionName;

        return property.serializedObject.FindProperty(conditionPath);
    }

    private bool GetconditionResult(SerializedProperty conditionProperty, object compareValue)
    {
        if(compareValue == null)
        {
            return conditionProperty.propertyType switch
            {
                SerializedPropertyType.Boolean => conditionProperty.boolValue,
                SerializedPropertyType.ObjectReference => conditionProperty.objectReferenceValue != null,
                SerializedPropertyType.String => !string.IsNullOrEmpty(conditionProperty.stringValue),
                SerializedPropertyType.Integer => conditionProperty.intValue != 0,
                SerializedPropertyType.Enum => conditionProperty.enumValueIndex != 0,
                _ => true
            };
        }

        return conditionProperty.propertyType switch
        {
            SerializedPropertyType.Boolean => conditionProperty.boolValue.Equals(Convert.ToBoolean(compareValue)),
            SerializedPropertyType.Integer => conditionProperty.intValue.Equals(Convert.ToInt32(compareValue)),
            SerializedPropertyType.Enum => conditionProperty.enumValueIndex.Equals(Convert.ToInt32(compareValue)),
            SerializedPropertyType.String => conditionProperty.stringValue.Equals(compareValue.ToString()),
            _ => true
        };
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldDraw(property))
            return 0f;

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attr = (ConditionFieldAtrribute)attribute;

        bool condition = EvaluateCondition(property, attr);

        bool shouldDraw = attr.Mode switch
        {
            ConditionMode.ShowIf => condition,
            ConditionMode.HideIf => !condition,
            _ => true
        };

        if (!shouldDraw)
            return;

        bool previousEnable = GUI.enabled;

        if (attr.Mode == ConditionMode.EnableIf)
            GUI.enabled = condition;
        else if (attr.Mode == ConditionMode.DisableIf)
            GUI.enabled = !condition;

        EditorGUI.PropertyField(position, property, label, true);

        GUI.enabled = previousEnable;
    }
}
