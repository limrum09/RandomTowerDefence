using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AchievementDebuggerWindow : EditorWindow
{
    private Vector2 activeScroll;
    private Vector2 completeScroll;
    private string searchText = string.Empty;

    [MenuItem("Tools/Achievement Debugger")]
    private static void Open()
    {
        GetWindow<AchievementDebuggerWindow>("Achievement Debugger");
    }

    private void OnGUI()
    {
        DrawToolbar();

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Achievement Debugger는 Play Mode에서만 사용할 수 있습니다.", MessageType.Info);
            return;
        }

        if (FindObjectOfType<Managers>() == null)
        {
            EditorGUILayout.HelpBox("Managers가 초기화되지 않았습니다.", MessageType.Warning);
            return;
        }

        DrawActiveAchievements();
        EditorGUILayout.Space(8);
        DrawCompleteAchievements();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
            Repaint();

        GUILayout.Space(8);

        searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(260));

        GUILayout.FlexibleSpace();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawActiveAchievements()
    {
        List<Quest> achievements = new List<Quest>(Managers.QuestMgr.ActiveAchievement);

        EditorGUILayout.LabelField($"Active Achievements ({achievements.Count})", EditorStyles.boldLabel);

        activeScroll = EditorGUILayout.BeginScrollView(activeScroll, "box", GUILayout.MinHeight(220));

        foreach (Quest achievement in achievements)
        {
            if (achievement == null)
                continue;

            if (!IsMatchSearch(achievement))
                continue;

            DrawActiveAchievementRow(achievement);
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawActiveAchievementRow(Quest achievement)
    {
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(achievement.QuestUID, EditorStyles.boldLabel);

        if (GUILayout.Button("Complete", GUILayout.Width(90)))
            CompleteAchievement(achievement);

        EditorGUILayout.EndHorizontal();

        DrawTaskInfo(achievement);

        EditorGUILayout.EndVertical();
    }

    private void DrawCompleteAchievements()
    {
        List<Quest> achievements = new List<Quest>(Managers.QuestMgr.CompleteAchievement);

        EditorGUILayout.LabelField($"Complete Achievements ({achievements.Count})", EditorStyles.boldLabel);

        completeScroll = EditorGUILayout.BeginScrollView(completeScroll, "box", GUILayout.MinHeight(160));

        foreach (Quest achievement in achievements)
        {
            if (achievement == null)
                continue;

            if (!IsMatchSearch(achievement))
                continue;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(achievement.QuestUID, EditorStyles.boldLabel);
            DrawTaskInfo(achievement);
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawTaskInfo(Quest achievement)
    {
        QuestTaskData task = achievement.Task;

        if (task == null)
            return;

        EditorGUILayout.LabelField("Task UID", task.TaskUId);
        EditorGUILayout.LabelField("Category", task.TaskCategory.ToString());
        EditorGUILayout.LabelField("Progress", $"{task.CurrentSuccess} / {task.NeedSuccessCount}");
        EditorGUILayout.LabelField("Condition", achievement.IsConditionComplete ? "Pass" : "Blocked");
    }

    private void CompleteAchievement(Quest achievement)
    {
        if (achievement == null)
            return;

        bool confirm = EditorUtility.DisplayDialog(
            "Complete Achievement",
            $"이 업적을 완료 처리할까요?\n\n{achievement.QuestUID}",
            "Complete",
            "Cancel");

        if (!confirm)
            return;

        achievement.QuestComplete();

        Debug.Log($"[AchievementDebugger] Complete Achievement: {achievement.QuestUID}");

        Repaint();
    }

    private bool IsMatchSearch(Quest achievement)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        string keyword = searchText.Trim();

        if (!string.IsNullOrEmpty(achievement.QuestUID) &&
            achievement.QuestUID.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        QuestTaskData task = achievement.Task;

        return task != null &&
               !string.IsNullOrEmpty(task.TaskUId) &&
               task.TaskUId.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
