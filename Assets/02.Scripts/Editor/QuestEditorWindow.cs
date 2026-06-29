using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuestEditorWindow : EditorWindow
{
    #region Quest Fields
    private List<Quest> quests = new List<Quest>();

    private Quest selectedQuest;
    private Editor questEditor;

    private Vector2 questListScroll;
    private Vector2 questInspectorScroll;
    #endregion

    #region Target Fields
    private List<TaskTarget> targets = new List<TaskTarget>();
    private TaskTarget selectedTarget;
    private Editor targetEditor;
    private Vector2 targetListScroll;
    private Vector2 targetInspectorScroll;
    #endregion

    #region ConditionField
    private List<QuestCondition> conditions = new List<QuestCondition>();
    private QuestCondition selectedCondition;
    private Editor conditionEditor;
    private Vector2 conditionListScroll;
    private Vector2 conditionInspectorScroll;
    #endregion

    private string searchText = "";
    private string createPath = "Assets/09.QuestAndAchievement/";

    private int selectedTab;
    private readonly string[] tabs =
    {
        "Quest",
        "Target",
        "Condition"
    };

    private void OnEnable()
    {
        RefreshQuestList();
        RefreshTargetList();
        RefreshConditionList();
    }

    private void OnDisable()
    {
        DestroyQuestCachedEditor();
        DestroyTargetCachedEditor();
        DestroyConditionCachedEditor();
    }

    private void OnGUI()
    {
        selectedTab = GUILayout.Toolbar(selectedTab, tabs);

        EditorGUILayout.Space();

        switch (selectedTab)
        {
            case 0:
                DrawQuestTab();
                break;
            case 1:
                DrawTargetTab();
                break;
            case 2:
                DrawConditionTab();
                break;
        }
    }

    private void DrawQuestTab()
    {
        DrawQuestToolBar();

        EditorGUILayout.BeginHorizontal();

        DrawQuestList();

        EditorGUILayout.Space(8);

        DrawQuestInspector();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTargetTab()
    {
        DrawTargetToolBar();

        EditorGUILayout.BeginHorizontal();

        DrawTargetList();

        EditorGUILayout.Space(8);

        DrawTargetInspector();

        EditorGUILayout.EndHorizontal();
    }    

    private void DrawConditionTab()
    {
        DrawConditionToolBar();

        EditorGUILayout.BeginHorizontal();

        DrawConditionList();

        EditorGUILayout.Space(8);

        DrawConditionInspector();

        EditorGUILayout.EndHorizontal();
    }

    #region Quest Tabs

    private void DrawQuestToolBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if(GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            RefreshQuestList();
        }

        GUILayout.Space(10);

        searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(250));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Quest", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            CreateQuestAsset();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Achievement", EditorStyles.toolbarButton, GUILayout.Width(150)))
        {
            CreateAchievementAsset();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawQuestList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));

        EditorGUILayout.LabelField("Quest List", EditorStyles.boldLabel);

        questListScroll = EditorGUILayout.BeginScrollView(questListScroll, "box");

        foreach(Quest quest in quests)
        {
            if (quest == null)
                continue;

            if (!IsMatchSearch(quest.name, quest.QuestUID))
                continue;

            GUIStyle style = selectedQuest == quest ? EditorStyles.boldLabel : EditorStyles.label;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(quest.name, style, GUILayout.Height(22)))
                SelectQuest(quest);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Create Path");
        createPath = EditorGUILayout.TextField(createPath);

        EditorGUILayout.EndVertical();
    }

    private void DrawQuestInspector()
    {
        EditorGUILayout.BeginVertical();

        if(selectedQuest == null)
        {
            EditorGUILayout.HelpBox("수정할 Quest를 선택하세요.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(selectedQuest.name, EditorStyles.boldLabel);

        if(GUILayout.Button("Ping", GUILayout.Width(60)))
        {
            EditorGUIUtility.PingObject(selectedQuest);
            Selection.activeObject = selectedQuest;
        }

        if(GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteSelectQuest();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        questInspectorScroll = EditorGUILayout.BeginScrollView(questInspectorScroll, "box");

        if(questEditor == null || questEditor.target != selectedQuest)
        {
            DestroyQuestCachedEditor();
            questEditor = Editor.CreateEditor(selectedQuest);
        }

        EditorGUI.BeginChangeCheck();

        questEditor.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(selectedQuest);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void RefreshQuestList()
    {
        quests.Clear();

        AddAssetsByType<Quest>();
        AddAssetsByType<Achievement>();

        quests.Sort((x, y) => string.Compare(x.name, y.name, System.StringComparison.Ordinal));


        if (quests.Count >= 1)
            SelectQuest(quests[0]);

        Repaint();
    }

    private void AddAssetsByType<T>() where T : Quest
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Quest quest = AssetDatabase.LoadAssetAtPath<Quest>(path);

            if (quest != null && !quests.Contains(quest))
                quests.Add(quest);
        }
    }

    private void CreateQuestAsset()
    {
        QuestCreatePopupWindow.Open("Quest", createPath, inputName =>
        {
            CreateQuestAsset<Quest>("Quest", inputName);
        });
    }

    private void CreateAchievementAsset()
    {
        QuestCreatePopupWindow.Open("Achievement", createPath, inputName =>
        {
            CreateQuestAsset<Achievement>("Achievement", inputName);
        });
    }

    private void CreateQuestAsset<T>(string createType, string inputName) where T : Quest
    {
        string path = createPath + createType;

        if (!AssetDatabase.IsValidFolder(path))
        {
            Debug.LogWarning($"폴더가 없습니다. {path}");
            return;
        }

        string safeName = MakeSafeName(inputName);

        string questUID = $"{createType.ToUpper()}_{safeName.ToUpper()}";
        string taskUID = $"TASK_{safeName.ToUpper()}";
        string assetName = $"{createType}_{safeName}";

        if(IsDuplicateAssetName(path, assetName))
        {
            EditorUtility.DisplayDialog("중복 생성 불가", $"이미 같은 이름의 Asset이 있습니다.\n\n{assetName}.asset","OK");
            return;
        }

        if (IsDuplicateQuestUID(questUID))
        {
            EditorUtility.DisplayDialog("중복 생성 불가", $"이미 같은 Quest UID가 있습니다.\n\n{questUID}", "OK");
            return;
        }

        T quest = CreateInstance<T>();

        quest.EditorSetUID(questUID);
        quest.EditorSetTaskUID(taskUID);

        string achPath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{createType}_{safeName}.asset");

        AssetDatabase.CreateAsset(quest, achPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshQuestList();
        SelectQuest(quest);

        EditorGUIUtility.PingObject(quest);
    }

    private void SelectQuest(Quest quest)
    {
        if (selectedQuest == quest)
            return;

        selectedQuest = quest;

        DestroyQuestCachedEditor();

        Selection.activeObject = selectedQuest;
    }

    private void DeleteSelectQuest()
    {
        if (selectedQuest == null)
            return;

        string path = AssetDatabase.GetAssetPath(selectedQuest);

        bool confirm = EditorUtility.DisplayDialog("Delete Quest", $"정말 삭제할까요?\n\n{selectedQuest.name}", "Delete", "Cancel");

        if (!confirm)
            return;

        DestroyQuestCachedEditor();

        selectedQuest = null;

        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshQuestList();
    }

    private bool IsDuplicateQuestUID(string uid)
    {
        foreach(Quest quest in quests)
        {
            if (quest == null)
                continue;

            if (quest.QuestUID == uid)
                return true;
        }

        return false;
    }

    private bool IsDuplicateAssetName(string folderPath, string assetName)
    {
        string assetPath = $"{folderPath}/{assetName}.asset";
        return AssetDatabase.LoadAssetAtPath<Quest>(assetPath) != null;
    }

    private string MakeSafeName(string name)
    {
        foreach(char c in System.IO.Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return name;
    }

    private void DestroyQuestCachedEditor()
    {
        if(questEditor!= null)
        {
            DestroyImmediate(questEditor);
            questEditor = null;
        }
    }
#endregion

    #region Target Tabs
    private void DrawTargetToolBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            RefreshTargetList();
        }

        GUILayout.Space(10);

        searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(250));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Enemy Target", EditorStyles.toolbarButton, GUILayout.Width(170)))
        {
            CreateEnemyTargetAsset();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create UID Target", EditorStyles.toolbarButton, GUILayout.Width(150)))
        {
            CreateUIDTargetAsset();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawTargetList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(220));

        EditorGUILayout.LabelField("Target List", EditorStyles.boldLabel);

        targetListScroll = EditorGUILayout.BeginScrollView(targetListScroll, "box");

        foreach (TaskTarget target in targets)
        {
            if (target == null)
                continue;

            if (!IsMatchSearch(target.name))
                continue;

            GUIStyle style = selectedTarget == target ? EditorStyles.boldLabel : EditorStyles.label;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(target.name, style, GUILayout.Height(22)))
                SelectTarget(target);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Create Path");
        createPath = EditorGUILayout.TextField(createPath);

        EditorGUILayout.EndVertical();
    }

    private void DrawTargetInspector()
    {
        EditorGUILayout.BeginVertical();

        if (selectedTarget == null)
        {
            EditorGUILayout.HelpBox("수정할 Target를 선택하세요.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(selectedTarget.name, EditorStyles.boldLabel);

        if (GUILayout.Button("Ping", GUILayout.Width(60)))
        {
            EditorGUIUtility.PingObject(selectedTarget);
            Selection.activeObject = selectedTarget;
        }

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteSelectedTarget();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        targetInspectorScroll = EditorGUILayout.BeginScrollView(targetInspectorScroll, "box");

        if (targetEditor == null || targetEditor.target != selectedTarget)
        {
            DestroyTargetCachedEditor();
            targetEditor = Editor.CreateEditor(selectedTarget);
        }

        EditorGUI.BeginChangeCheck();

        targetEditor.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(selectedTarget);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void RefreshTargetList()
    {
        targets.Clear();

        string[] guids = AssetDatabase.FindAssets("t:TaskTarget");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TaskTarget target = AssetDatabase.LoadAssetAtPath<TaskTarget>(path);

            if (target != null && !targets.Contains(target))
                targets.Add(target);
        }

        targets.Sort((x, y) => string.Compare(x.name, y.name, System.StringComparison.Ordinal));

        if (targets.Count >= 1)
            SelectTarget(targets[0]);

        Repaint();
    }

    private void CreateEnemyTargetAsset()
    {
        QuestCreatePopupWindow.Open("Enemy Target", createPath, inputName =>
        {
            CreateTargetAsset<EnemyTarget>("EnemyTarget", createPath + "Target", inputName);
        });
    }

    private void CreateUIDTargetAsset()
    {
        QuestCreatePopupWindow.Open("UID Target", createPath, inputName =>
        {
            CreateTargetAsset<UIDTarget>("UIDTarget", createPath + "Target", inputName);
        });
    }

    private void CreateTargetAsset<T>(string createType, string folderPath, string inputName, Action<T, string> initialize = null) where T : TaskTarget
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning($"폴더가 없습니다. {folderPath}");
            return;
        }

        T asset = CreateInstance<T>();

        initialize?.Invoke(asset, inputName);

        string safeName = MakeSafeName(inputName);

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{createType}_{safeName}.asset");

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshTargetList();
        SelectTarget(asset);

        EditorGUIUtility.PingObject(asset);
    }

    private void DeleteSelectedTarget()
    {
        if (selectedTarget == null)
            return;

        string path = AssetDatabase.GetAssetPath(selectedTarget);

        bool confirm = EditorUtility.DisplayDialog("Delete Target", $"정말 삭제할까요? \n\n{selectedTarget.name}", "Delete", "Cancel");

        if (!confirm)
            return;

        DestroyTargetCachedEditor();

        selectedTarget = null;

        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshTargetList();
    }

    private void SelectTarget(TaskTarget target)
    {
        if (selectedTarget == target)
            return;

        selectedTarget = target;

        DestroyTargetCachedEditor();

        Selection.activeObject = selectedTarget;
    }

    private void DestroyTargetCachedEditor()
    {
        if (targetEditor != null)
        {
            DestroyImmediate(targetEditor);
            targetEditor = null;
        }
    }
    #endregion

    #region Condition Tabs
    private void DrawConditionToolBar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
        {
            RefreshConditionList();
        }

        GUILayout.Space(10);

        searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(250));

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Create Achievement Condition", EditorStyles.toolbarButton, GUILayout.Width(250)))
        {
            CreateAchievementConditionAsset();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Achievement", EditorStyles.toolbarButton, GUILayout.Width(150)))
        {
            // CreateAchievementAsset();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawConditionList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));

        EditorGUILayout.LabelField("Condition List", EditorStyles.boldLabel);

        conditionListScroll = EditorGUILayout.BeginScrollView(conditionListScroll, "box");

        foreach (QuestCondition condition in conditions)
        {
            if (condition == null)
                continue;

            if (!IsMatchSearch(condition.name))
                continue;

            GUIStyle style = selectedCondition == condition ? EditorStyles.boldLabel : EditorStyles.label;

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(condition.name, style, GUILayout.Height(22)))
                SelectCondition(condition);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Create Path");
        createPath = EditorGUILayout.TextField(createPath);

        EditorGUILayout.EndVertical();
    }

    private void DrawConditionInspector()
    {
        EditorGUILayout.BeginVertical();

        if (selectedCondition == null)
        {
            EditorGUILayout.HelpBox("수정할 Condition을 선택하세요.", MessageType.Info);
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(selectedCondition.name, EditorStyles.boldLabel);

        if (GUILayout.Button("Ping", GUILayout.Width(60)))
        {
            EditorGUIUtility.PingObject(selectedCondition);
            Selection.activeObject = selectedCondition;
        }

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteSelectCondition();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        conditionInspectorScroll = EditorGUILayout.BeginScrollView(conditionInspectorScroll, "box");

        if (conditionEditor == null || conditionEditor.target != selectedCondition)
        {
            DestroyConditionCachedEditor();
            conditionEditor = Editor.CreateEditor(selectedCondition);
        }

        EditorGUI.BeginChangeCheck();

        conditionEditor.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(selectedCondition);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void RefreshConditionList()
    {
        conditions.Clear();

        AddConditionAssetsByType<IsAchievementCompleteCondition>();

        conditions.Sort((x, y) => string.Compare(x.name, y.name, System.StringComparison.Ordinal));

        if (conditions.Count >= 1)
            SelectCondition(conditions[0]);

        Repaint();
    }

    private void AddConditionAssetsByType<T>() where T : QuestCondition
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            T condition = AssetDatabase.LoadAssetAtPath<T>(path);

            if (condition != null && !conditions.Contains(condition))
                conditions.Add(condition);
        }
    }

    private void CreateAchievementConditionAsset()
    {
        QuestCreatePopupWindow.Open("Achievement_Condition", createPath, inputName =>
        {
            CreateConditionAsset<IsAchievementCompleteCondition>("Achievement_Condition_", createPath + "Condition", inputName);
        });
    }

    private void CreateConditionAsset<T>(string createType, string folderPath, string inputName, Action<T, string> initialize = null) where T : QuestCondition
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogWarning($"폴더가 없습니다. {folderPath}");
            return;
        }

        T asset = CreateInstance<T>();

        initialize?.Invoke(asset, inputName);

        string safeName = MakeSafeName(inputName);

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{createType}_{safeName}.asset");

        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshConditionList();
        SelectCondition(asset);

        EditorGUIUtility.PingObject(asset);
    }

    private void SelectCondition(QuestCondition condition)
    {
        if (selectedCondition == condition)
            return;

        selectedCondition= condition;

        DestroyConditionCachedEditor();

        Selection.activeObject = selectedCondition;
    }

    private void DeleteSelectCondition()
    {
        if (selectedCondition == null)
            return;

        string path = AssetDatabase.GetAssetPath(selectedCondition);

        bool confirm = EditorUtility.DisplayDialog("Delete Quest", $"정말 삭제할까요?\n\n{selectedCondition.name}", "Delete", "Cancel");

        if (!confirm)
            return;

        DestroyConditionCachedEditor();

        selectedCondition = null;

        AssetDatabase.DeleteAsset(path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshConditionList();
    }

    private void DestroyConditionCachedEditor()
    {
        if (conditionEditor != null)
        {
            DestroyImmediate(conditionEditor);
            conditionEditor = null;
        }
    }
    #endregion

    private bool IsMatchSearch(params string[] vals)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        string lower = searchText.ToLower();

        foreach(string val in vals)
        {
            if (!string.IsNullOrEmpty(val) && val.ToLower().Contains(lower))
                return true;
        }

        return false;
    }

    [MenuItem("Tools/Quest Editor")]
    public static void Open()
    {
        GetWindow<QuestEditorWindow>("Quest Editor");
    }
}
