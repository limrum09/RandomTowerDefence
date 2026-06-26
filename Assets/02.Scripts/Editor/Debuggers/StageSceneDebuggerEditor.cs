#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StageSceneDebugger))]
public class StageSceneDebuggerEditor : Editor
{
    private SerializedProperty stageManager;
    private SerializedProperty difficulty;
    private SerializedProperty waveNumber;
    private SerializedProperty gold;
    private SerializedProperty life;
    private SerializedProperty obstacle;

    private void OnEnable()
    {
        stageManager = serializedObject.FindProperty("stage");
        difficulty = serializedObject.FindProperty("difficulty");
        waveNumber = serializedObject.FindProperty("waveNumber");
        gold = serializedObject.FindProperty("gold");
        life = serializedObject.FindProperty("life");
        obstacle = serializedObject.FindProperty("obstacle");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        StageSceneDebugger debugger = (StageSceneDebugger)target;

        EditorGUILayout.PropertyField(stageManager);
        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(difficulty);
        if(GUILayout.Button("Apply Difficulty"))
        {
            serializedObject.ApplyModifiedProperties();
            debugger.ApplyDifficulty();
        }

        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(waveNumber);
        if(GUILayout.Button("Apply Wave"))
        {
            serializedObject.ApplyModifiedProperties();
            debugger.ApplyWave();
        }

        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("Session Debug", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);

        EditorGUILayout.PropertyField(gold);
        if(GUILayout.Button("Add Gold"))
        {
            serializedObject.ApplyModifiedProperties();
            debugger.AddGold();
        }

        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(life);
        if(GUILayout.Button("Add Life"))
        {
            serializedObject.ApplyModifiedProperties();
            debugger.AddLife();
        }

        EditorGUILayout.Space(8);

        EditorGUILayout.PropertyField(obstacle);
        if(GUILayout.Button("Add Free Obstacle"))
        {
            serializedObject.ApplyModifiedProperties();
            debugger.AddFreeObstacle();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
