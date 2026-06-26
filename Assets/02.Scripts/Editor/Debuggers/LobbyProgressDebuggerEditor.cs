#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LobbyProgressDebugger))]
public class LobbyProgressDebuggerEditor : Editor
{
    private SerializedProperty lobbyUICtr;
    private SerializedProperty metaCurrenyAmount;
    private SerializedProperty expAmount;

    private void OnEnable()
    {
        lobbyUICtr = serializedObject.FindProperty("lobbyUI");
        metaCurrenyAmount = serializedObject.FindProperty("metaCurrenyAmount");
        expAmount = serializedObject.FindProperty("expAmount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        LobbyProgressDebugger debugger = (LobbyProgressDebugger)target;

        EditorGUILayout.PropertyField(lobbyUICtr);
        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(metaCurrenyAmount);
        EditorGUILayout.Space(2);

        if (GUILayout.Button("Add Meta Currency"))
        {
            serializedObject.ApplyModifiedProperties();
            debugger.AddMetaCurrency();
        }

        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(expAmount);
        EditorGUILayout.Space(2);

        if (GUILayout.Button("Add Meta Exp"))
        {
            serializedObject.ApplyModifiedProperties();
            debugger.AddExp();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif