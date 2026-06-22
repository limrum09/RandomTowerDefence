using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EnemyTableEditorWindow : EditorWindow
{
    private const string EnemyDataPath = "Assets/Resources/Data/EnemyData.json";
    private const string EnemySkillDataPath = "Assets/Resources/Data/EnemySkillData.json";

    private readonly string[] tabs = { "Enemy", "Skill" };
    private readonly string[] enemyTypes = { "Normal", "Fast", "Tank", "Elite", "Boss", "Bonus" };

    private EnemyDataRowList enemyRows = new EnemyDataRowList();
    private EnemySkillRowList skillRows = new EnemySkillRowList();

    private int selectedTab;
    private int selectedEnemyIndex = -1;
    private int selectedSkillIndex = -1;
    private string searchText = string.Empty;
    private bool autoSave = true;
    private bool isDirty;
    private int previewFontSize = 12;

    private Vector2 listScroll;
    private Vector2 inspectorScroll;
    private GUIStyle previewBoxStyle;

    [MenuItem("Tools/Enemy Table Editor")]
    public static void Open()
    {
        GetWindow<EnemyTableEditorWindow>("Enemy Table");
    }

    private void OnEnable()
    {
        LoadAll();
    }

    private void OnGUI()
    {
        DrawToolbar();

        selectedTab = GUILayout.Toolbar(selectedTab, tabs);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        DrawList();
        EditorGUILayout.Space(8);
        DrawInspector();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(70)))
            LoadAll();

        using (new EditorGUI.DisabledScope(!isDirty))
        {
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(70)))
                SaveCurrentTab();
        }

        GUILayout.Space(8);
        autoSave = GUILayout.Toggle(autoSave, "Auto Save", EditorStyles.toolbarButton, GUILayout.Width(90));
        GUILayout.Space(8);
        EditorGUILayout.LabelField("Preview Font", GUILayout.Width(80));
        previewFontSize = EditorGUILayout.IntSlider(previewFontSize, 9, 24, GUILayout.Width(180));
        GUILayout.Space(8);
        searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(240));

        GUILayout.FlexibleSpace();

        if (selectedTab == 0 && GUILayout.Button("Add Enemy", EditorStyles.toolbarButton, GUILayout.Width(90)))
            AddEnemy();

        if (selectedTab == 1 && GUILayout.Button("Add Skill", EditorStyles.toolbarButton, GUILayout.Width(90)))
            AddSkill();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(260));

        EditorGUILayout.LabelField(selectedTab == 0 ? "Enemy List" : "Skill List", EditorStyles.boldLabel);
        listScroll = EditorGUILayout.BeginScrollView(listScroll, "box");

        if (selectedTab == 0)
            DrawEnemyList();
        else
            DrawSkillList();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawEnemyList()
    {
        for (int i = 0; i < enemyRows.datas.Count; i++)
        {
            EnemyDataRow row = enemyRows.datas[i];

            if (!IsMatchSearch(row.Enemy_UID, row.String_Key, row.Type, row.Enemy_Skill_UID))
                continue;

            GUIStyle style = selectedEnemyIndex == i ? EditorStyles.boldLabel : EditorStyles.label;
            string label = $"{row.Enemy_UID} [{row.Type}]";

            if (GUILayout.Button(label, style, GUILayout.Height(22)))
                selectedEnemyIndex = i;
        }
    }

    private void DrawSkillList()
    {
        for (int i = 0; i < skillRows.datas.Count; i++)
        {
            EnemySkillRow row = skillRows.datas[i];

            if (!IsMatchSearch(row.Enemy_Skill_UID, row.Type, row.Target_type, row.String_Key))
                continue;

            GUIStyle style = selectedSkillIndex == i ? EditorStyles.boldLabel : EditorStyles.label;
            string label = $"{row.Enemy_Skill_UID} [{row.Type}/{row.Target_type}]";

            if (GUILayout.Button(label, style, GUILayout.Height(22)))
                selectedSkillIndex = i;
        }
    }

    private void DrawInspector()
    {
        EditorGUILayout.BeginVertical();
        inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll, "box");

        if (selectedTab == 0)
            DrawEnemyInspector();
        else
            DrawSkillInspector();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawEnemyInspector()
    {
        if (selectedEnemyIndex < 0 || selectedEnemyIndex >= enemyRows.datas.Count)
        {
            EditorGUILayout.HelpBox("Select an enemy row.", MessageType.Info);
            return;
        }

        EnemyDataRow row = enemyRows.datas[selectedEnemyIndex];

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(row.Enemy_UID, EditorStyles.boldLabel);

        if (GUILayout.Button("Duplicate", GUILayout.Width(90)))
            DuplicateEnemy(row);

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteEnemy();
            EditorGUILayout.EndHorizontal();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        row.Enemy_UID = EditorGUILayout.TextField("Enemy UID", row.Enemy_UID);
        row.String_Key = EditorGUILayout.TextField("String Key", row.String_Key);
        row.Type = DrawPopupOrText("Type", row.Type, enemyTypes);
        row.Enemy_Skill_UID = DrawSkillUIDField(row.Enemy_Skill_UID);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);

        row.Basic_HP = EditorGUILayout.IntField("Basic HP", row.Basic_HP);
        row.Increase_HP = EditorGUILayout.IntField("Increase HP", row.Increase_HP);
        row.Move_Speed = EditorGUILayout.FloatField("Move Speed", row.Move_Speed);
        row.Basic_Shield = EditorGUILayout.IntField("Basic Shield", row.Basic_Shield);
        row.Increase_Sheild = EditorGUILayout.IntField("Increase Shield", row.Increase_Sheild);
        row.Reward_Gold = EditorGUILayout.FloatField("Reward Gold", row.Reward_Gold);
        row.Icon_UID = EditorGUILayout.TextField("Icon UID", row.Icon_UID);

        if (EditorGUI.EndChangeCheck())
            MarkChanged();

        DrawEnemyFormulaPreview(row);
    }

    private void DrawSkillInspector()
    {
        if (selectedSkillIndex < 0 || selectedSkillIndex >= skillRows.datas.Count)
        {
            EditorGUILayout.HelpBox("Select a skill row.", MessageType.Info);
            return;
        }

        EnemySkillRow row = skillRows.datas[selectedSkillIndex];

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(row.Enemy_Skill_UID, EditorStyles.boldLabel);

        if (GUILayout.Button("Duplicate", GUILayout.Width(90)))
            DuplicateSkill(row);

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteSkill();
            EditorGUILayout.EndHorizontal();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        row.Enemy_Skill_UID = EditorGUILayout.TextField("Skill UID", row.Enemy_Skill_UID);
        row.Type = DrawEnemySkillTypeField("Type", row.Type);
        row.Target_type = DrawEnemySkillTargetField("Target Type", row.Target_type);
        row.Value_type = DrawEnemySkillValueTypeField("Value Type", row.Value_type);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Timing", EditorStyles.boldLabel);

        row.Duration = EditorGUILayout.FloatField("Duration", row.Duration);
        row.CoolDown = EditorGUILayout.FloatField("CoolDown", row.CoolDown);
        row.Tick_Interval = EditorGUILayout.FloatField("Tick Interval", row.Tick_Interval);
        row.Range = EditorGUILayout.FloatField("Range", row.Range);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Value Scaling", EditorStyles.boldLabel);

        row.Basic_Value = EditorGUILayout.FloatField("Basic Value", row.Basic_Value);
        row.Increasee_Value = EditorGUILayout.FloatField("Increase Value", row.Increasee_Value);
        row.Scale_Type = EditorGUILayout.TextField("Scale Type", row.Scale_Type);
        row.Scale_Interval = EditorGUILayout.IntField("Scale Interval", row.Scale_Interval);
        row.Scale_Max = EditorGUILayout.IntField("Scale Max", row.Scale_Max);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Localization / Icon", EditorStyles.boldLabel);

        row.String_Key = EditorGUILayout.TextField("String Key", row.String_Key);
        row.Des_String_Key = EditorGUILayout.TextField("Description Key", row.Des_String_Key);
        row.Icon_UID = EditorGUILayout.TextField("Icon UID", row.Icon_UID);

        if (EditorGUI.EndChangeCheck())
            MarkChanged();

        DrawSkillFormulaPreview(row);
    }

    private string DrawSkillUIDField(string current)
    {
        List<string> uids = new List<string>();

        foreach (EnemySkillRow skill in skillRows.datas)
            uids.Add(skill.Enemy_Skill_UID);

        if (uids.Count == 0)
            return EditorGUILayout.TextField("Enemy Skill UID", current);

        int index = uids.IndexOf(current);

        if (index < 0)
        {
            EditorGUILayout.HelpBox($"Current skill UID does not exist: {current}", MessageType.Warning);
            return EditorGUILayout.TextField("Enemy Skill UID", current);
        }

        int nextIndex = EditorGUILayout.Popup("Enemy Skill UID", index, uids.ToArray());
        return uids[nextIndex];
    }

    private string DrawPopupOrText(string label, string current, string[] values)
    {
        int index = Array.IndexOf(values, current);

        if (index < 0)
            return EditorGUILayout.TextField(label, current);

        int next = EditorGUILayout.Popup(label, index, values);
        return values[next];
    }

    private string DrawEnemySkillTypeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out EnemySkillType value))
            value = EnemySkillType.None;

        value = (EnemySkillType)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawEnemySkillTargetField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out EnemySkillTarget value))
            value = EnemySkillTarget.None;

        value = (EnemySkillTarget)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawEnemySkillValueTypeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out EnemySkillValueType value))
            value = EnemySkillValueType.None;

        value = (EnemySkillValueType)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private void DrawEnemyFormulaPreview(EnemyDataRow row)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Formula Preview", EditorStyles.boldLabel);

        string preview =
            $"MaxHP = {row.Basic_HP} + {row.Increase_HP} * Level\n" +
            $"MaxShield = {row.Basic_Shield} + {row.Increase_Sheild} * Level\n" +
            $"MoveSpeed = {row.Move_Speed}\n" +
            $"RewardGold = {row.Reward_Gold}";

        EditorGUILayout.LabelField(preview, GetPreviewBoxStyle());
    }

    private void DrawSkillFormulaPreview(EnemySkillRow row)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Formula Preview", EditorStyles.boldLabel);

        string formula = row.Scale_Interval > 0
            ? $"{row.Basic_Value} + {row.Increasee_Value} * floor(min(Level, {row.Scale_Max}) / {row.Scale_Interval})"
            : $"{row.Basic_Value}";

        EditorGUILayout.LabelField($"SkillValue = {formula}", GetPreviewBoxStyle());
    }

    private GUIStyle GetPreviewBoxStyle()
    {
        if (previewBoxStyle == null)
        {
            previewBoxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                wordWrap = true,
                richText = false,
                padding = new RectOffset(8, 8, 8, 8)
            };
        }

        previewBoxStyle.fontSize = previewFontSize;
        return previewBoxStyle;
    }

    private void LoadAll()
    {
        enemyRows = LoadJson<EnemyDataRowList>(EnemyDataPath) ?? new EnemyDataRowList();
        skillRows = LoadJson<EnemySkillRowList>(EnemySkillDataPath) ?? new EnemySkillRowList();

        selectedEnemyIndex = enemyRows.datas.Count > 0 ? Mathf.Clamp(selectedEnemyIndex, 0, enemyRows.datas.Count - 1) : -1;
        selectedSkillIndex = skillRows.datas.Count > 0 ? Mathf.Clamp(selectedSkillIndex, 0, skillRows.datas.Count - 1) : -1;

        isDirty = false;
        Repaint();
    }

    private T LoadJson<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            Debug.LogWarning($"JSON file not found: {path}");
            return null;
        }

        return JsonUtility.FromJson<T>(File.ReadAllText(path));
    }

    private void SaveCurrentTab()
    {
        if (selectedTab == 0)
            SaveEnemyData();
        else
            SaveSkillData();

        isDirty = false;
        AssetDatabase.Refresh();
    }

    private void SaveEnemyData()
    {
        SaveJson(EnemyDataPath, enemyRows);
    }

    private void SaveSkillData()
    {
        SaveJson(EnemySkillDataPath, skillRows);
    }

    private void SaveJson<T>(string path, T data)
    {
        File.WriteAllText(path, JsonUtility.ToJson(data, true));
    }

    private void MarkChanged()
    {
        isDirty = true;

        if (autoSave)
            SaveCurrentTab();
    }

    private void AddEnemy()
    {
        string uid = GetNextUID("E", enemyRows.datas.ConvertAll(x => x.Enemy_UID), 3);
        string number = uid.Substring(1);

        enemyRows.datas.Add(new EnemyDataRow
        {
            Enemy_UID = uid,
            String_Key = $"ENEMY_NAME_{number}",
            Type = "Normal",
            Enemy_Skill_UID = "ES0000",
            Basic_HP = 100,
            Increase_HP = 20,
            Move_Speed = 1f,
            Basic_Shield = 0,
            Increase_Sheild = 0,
            Reward_Gold = 1,
            Icon_UID = $"ENEMY_ICON_{number}"
        });

        selectedEnemyIndex = enemyRows.datas.Count - 1;
        MarkChanged();
    }

    private void AddSkill()
    {
        string uid = GetNextUID("ES", skillRows.datas.ConvertAll(x => x.Enemy_Skill_UID), 4);

        skillRows.datas.Add(new EnemySkillRow
        {
            Enemy_Skill_UID = uid,
            Type = EnemySkillType.None.ToString(),
            Target_type = EnemySkillTarget.Self.ToString(),
            Duration = 0,
            CoolDown = 0,
            Tick_Interval = 0,
            Range = 0,
            Value_type = EnemySkillValueType.Flat.ToString(),
            Basic_Value = 0,
            Increasee_Value = 0,
            Scale_Type = "None",
            Scale_Interval = 0,
            Scale_Max = 0,
            String_Key = $"{uid}_NAME",
            Des_String_Key = $"{uid}_DES",
            Icon_UID = "ICON_ENEMY_SKILL_NONE"
        });

        selectedSkillIndex = skillRows.datas.Count - 1;
        MarkChanged();
    }

    private void DuplicateEnemy(EnemyDataRow source)
    {
        string uid = GetNextUID("E", enemyRows.datas.ConvertAll(x => x.Enemy_UID), 3);

        enemyRows.datas.Add(new EnemyDataRow
        {
            Enemy_UID = uid,
            String_Key = source.String_Key,
            Type = source.Type,
            Enemy_Skill_UID = source.Enemy_Skill_UID,
            Basic_HP = source.Basic_HP,
            Increase_HP = source.Increase_HP,
            Move_Speed = source.Move_Speed,
            Basic_Shield = source.Basic_Shield,
            Increase_Sheild = source.Increase_Sheild,
            Reward_Gold = source.Reward_Gold,
            Icon_UID = source.Icon_UID
        });

        selectedEnemyIndex = enemyRows.datas.Count - 1;
        MarkChanged();
    }

    private void DuplicateSkill(EnemySkillRow source)
    {
        string uid = GetNextUID("ES", skillRows.datas.ConvertAll(x => x.Enemy_Skill_UID), 4);

        skillRows.datas.Add(new EnemySkillRow
        {
            Enemy_Skill_UID = uid,
            Type = source.Type,
            Target_type = source.Target_type,
            Duration = source.Duration,
            CoolDown = source.CoolDown,
            Tick_Interval = source.Tick_Interval,
            Range = source.Range,
            Value_type = source.Value_type,
            Basic_Value = source.Basic_Value,
            Increasee_Value = source.Increasee_Value,
            Scale_Type = source.Scale_Type,
            Scale_Interval = source.Scale_Interval,
            Scale_Max = source.Scale_Max,
            String_Key = source.String_Key,
            Des_String_Key = source.Des_String_Key,
            Icon_UID = source.Icon_UID
        });

        selectedSkillIndex = skillRows.datas.Count - 1;
        MarkChanged();
    }

    private void DeleteEnemy()
    {
        if (selectedEnemyIndex < 0 || selectedEnemyIndex >= enemyRows.datas.Count)
            return;

        EnemyDataRow row = enemyRows.datas[selectedEnemyIndex];
        bool confirm = EditorUtility.DisplayDialog("Delete Enemy", $"Delete {row.Enemy_UID}?", "Delete", "Cancel");

        if (!confirm)
            return;

        enemyRows.datas.RemoveAt(selectedEnemyIndex);
        selectedEnemyIndex = enemyRows.datas.Count > 0 ? Mathf.Clamp(selectedEnemyIndex, 0, enemyRows.datas.Count - 1) : -1;
        MarkChanged();
    }

    private void DeleteSkill()
    {
        if (selectedSkillIndex < 0 || selectedSkillIndex >= skillRows.datas.Count)
            return;

        EnemySkillRow row = skillRows.datas[selectedSkillIndex];
        bool confirm = EditorUtility.DisplayDialog("Delete Skill", $"Delete {row.Enemy_Skill_UID}?", "Delete", "Cancel");

        if (!confirm)
            return;

        skillRows.datas.RemoveAt(selectedSkillIndex);
        selectedSkillIndex = skillRows.datas.Count > 0 ? Mathf.Clamp(selectedSkillIndex, 0, skillRows.datas.Count - 1) : -1;
        MarkChanged();
    }

    private string GetNextUID(string prefix, List<string> existingUIDs, int digits)
    {
        int max = 0;

        foreach (string uid in existingUIDs)
        {
            if (string.IsNullOrEmpty(uid) || !uid.StartsWith(prefix))
                continue;

            string numberText = uid.Substring(prefix.Length);

            if (int.TryParse(numberText, out int number))
                max = Mathf.Max(max, number);
        }

        return $"{prefix}{(max + 1).ToString().PadLeft(digits, '0')}";
    }

    private bool IsMatchSearch(params string[] values)
    {
        if (string.IsNullOrWhiteSpace(searchText))
            return true;

        string lower = searchText.ToLowerInvariant();

        foreach (string value in values)
        {
            if (!string.IsNullOrEmpty(value) && value.ToLowerInvariant().Contains(lower))
                return true;
        }

        return false;
    }
}
