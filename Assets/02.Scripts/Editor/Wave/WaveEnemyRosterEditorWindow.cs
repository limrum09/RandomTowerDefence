using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class WaveEnemyRosterEditorWindow : EditorWindow
{
    private const string WaveDataPath = "Assets/Resources/Data/WaveData.Json";
    private const string WaveEnemyRosterDataPath = "Assets/Resources/Data/WaveEnemyRosterData.Json";
    private const string EnemyDataPath = "Assets/Resources/Data/EnemyData.json";

    private readonly string[] tabs = { "Easy", "Normal", "Hard", "Hell", "Validate" };
    private readonly string[] difficultyPrefixes = { "EASY", "NORMAL", "HARD", "HELL" };
    private readonly string[] spawnTypes = { "Immediate", "Delayed", "Support", "Boss" };

    private WaveDataRowList waveRows = new WaveDataRowList();
    private WaveEnemyRosterRowList rosterRows = new WaveEnemyRosterRowList();
    private EnemyDataRowList enemyRows = new EnemyDataRowList();

    private int selectedTab;
    private int selectedWaveIndex = -1;
    private string searchText = string.Empty;
    private bool autoSave = true;
    private bool isDirty;

    private Vector2 waveListScroll;
    private Vector2 inspectorScroll;
    private Vector2 validateScroll;

    [MenuItem("Tools/Wave Enemy Roster Editor")]
    public static void Open()
    {
        GetWindow<WaveEnemyRosterEditorWindow>("Wave Enemy Roster");
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

        if (selectedTab == 4)
        {
            DrawValidateTab();
            return;
        }

        EditorGUILayout.BeginHorizontal();
        DrawWaveList();
        EditorGUILayout.Space(8);
        DrawRosterInspector();
        EditorGUILayout.EndHorizontal();
    }

    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Reload", EditorStyles.toolbarButton, GUILayout.Width(70)))
            LoadAll();

        using (new EditorGUI.DisabledScope(!isDirty || selectedTab == 4))
        {
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(70)))
                SaveCurrentTab();
        }

        GUILayout.Space(8);
        autoSave = GUILayout.Toggle(autoSave, "Auto Save", EditorStyles.toolbarButton, GUILayout.Width(90));
        GUILayout.Space(8);
        searchText = GUILayout.TextField(searchText, EditorStyles.toolbarSearchField, GUILayout.Width(260));

        GUILayout.FlexibleSpace();

        using (new EditorGUI.DisabledScope(selectedTab == 4 || !HasSelectedWave()))
        {
            if (GUILayout.Button("Add Roster", EditorStyles.toolbarButton, GUILayout.Width(90)))
                AddRoster();

            if (GUILayout.Button("Sort Wave", EditorStyles.toolbarButton, GUILayout.Width(80)))
                SortSelectedWaveRoster();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawWaveList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(300));

        string difficulty = GetSelectedDifficulty();
        EditorGUILayout.LabelField($"{difficulty} Waves", EditorStyles.boldLabel);

        List<WaveDataRow> waves = GetWavesByDifficulty(difficulty);
        waveListScroll = EditorGUILayout.BeginScrollView(waveListScroll, "box");

        for (int i = 0; i < waves.Count; i++)
        {
            WaveDataRow wave = waves[i];

            if (!IsMatchSearch(wave.WaveNo, wave.Wave_Type, wave.NextWave))
                continue;

            int rosterCount = GetRosterRows(wave.WaveNo).Count;
            GUIStyle style = selectedWaveIndex == i ? EditorStyles.boldLabel : EditorStyles.label;
            string warning = rosterCount <= 0 ? "  !" : string.Empty;
            string boss = wave.IsBossWave == "Y" ? " Boss" : string.Empty;
            string label = $"{wave.WaveNo} ({rosterCount}) [{wave.Wave_Type}{boss}]{warning}";

            if (GUILayout.Button(label, style, GUILayout.Height(22)))
                selectedWaveIndex = i;
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawRosterInspector()
    {
        EditorGUILayout.BeginVertical();
        inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll, "box");

        WaveDataRow wave = GetSelectedWave();
        if (wave == null)
        {
            EditorGUILayout.HelpBox("Select a wave.", MessageType.Info);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.LabelField(wave.WaveNo, EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Next Wave", wave.NextWave);
        EditorGUILayout.LabelField("Wave Type", wave.Wave_Type);
        EditorGUILayout.LabelField("Boss Wave", wave.IsBossWave);
        EditorGUILayout.Space();

        List<WaveEnemyRosterRow> rows = GetRosterRows(wave.WaveNo);

        if (rows.Count <= 0)
            EditorGUILayout.HelpBox("This wave has no enemy roster data.", MessageType.Warning);

        for (int i = 0; i < rows.Count; i++)
            DrawRosterRow(rows[i], i);

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawRosterRow(WaveEnemyRosterRow row, int index)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField($"Roster {index + 1}", EditorStyles.boldLabel);

        if (GUILayout.Button("Duplicate", GUILayout.Width(90)))
        {
            DuplicateRoster(row);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteRoster(row);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            return;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("Wave ID", row.WaveID);
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginChangeCheck();

        row.SpawnOrder = EditorGUILayout.IntField("Spawn Order", row.SpawnOrder);
        row.Enemy_UID = EditorGUILayout.TextField("Enemy UID", row.Enemy_UID);
        row.EnemyLevel = EditorGUILayout.IntField("Enemy Level", row.EnemyLevel);
        row.SpawnCount = EditorGUILayout.IntField("Spawn Count", row.SpawnCount);
        row.StartTime = EditorGUILayout.FloatField("Start Time", row.StartTime);
        row.SpawnInterval = EditorGUILayout.FloatField("Spawn Interval", row.SpawnInterval);
        row.SpawnType = DrawPopupOrText("Spawn Type", row.SpawnType, spawnTypes);

        if (EditorGUI.EndChangeCheck())
        {
            row.WaveID = GetSelectedWaveID();
            MarkChanged();
        }

        DrawRosterValidation(row);
        EditorGUILayout.EndVertical();
    }

    private void DrawRosterValidation(WaveEnemyRosterRow row)
    {
        foreach (ValidationMessage message in GetRosterValidationMessages(row))
            EditorGUILayout.HelpBox(message.Text, message.Type);
    }

    private void DrawValidateTab()
    {
        validateScroll = EditorGUILayout.BeginScrollView(validateScroll);

        foreach (string difficulty in difficultyPrefixes)
        {
            List<ValidationMessage> messages = GetDifficultyValidationMessages(difficulty);
            int waveCount = GetWavesByDifficulty(difficulty).Count;
            int rosterWaveCount = GetRosterWaveCountByDifficulty(difficulty);

            EditorGUILayout.LabelField($"{difficulty}  Waves: {waveCount} / Roster Waves: {rosterWaveCount}", EditorStyles.boldLabel);

            if (messages.Count <= 0)
            {
                EditorGUILayout.HelpBox("No validation issues.", MessageType.Info);
            }
            else
            {
                foreach (ValidationMessage message in messages)
                    EditorGUILayout.HelpBox(message.Text, message.Type);
            }

            EditorGUILayout.Space();
        }

        List<ValidationMessage> unknownMessages = GetUnknownDifficultyValidationMessages();
        EditorGUILayout.LabelField("Unknown Difficulty", EditorStyles.boldLabel);

        if (unknownMessages.Count <= 0)
        {
            EditorGUILayout.HelpBox("No unknown difficulty roster rows.", MessageType.Info);
        }
        else
        {
            foreach (ValidationMessage message in unknownMessages)
                EditorGUILayout.HelpBox(message.Text, message.Type);
        }

        EditorGUILayout.EndScrollView();
    }

    private List<ValidationMessage> GetDifficultyValidationMessages(string difficulty)
    {
        List<ValidationMessage> messages = new List<ValidationMessage>();
        List<WaveDataRow> waves = GetWavesByDifficulty(difficulty);
        HashSet<string> waveIDs = new HashSet<string>();

        foreach (WaveDataRow wave in waves)
        {
            waveIDs.Add(wave.WaveNo);

            if (GetRosterRows(wave.WaveNo).Count <= 0)
                messages.Add(Warning($"{wave.WaveNo} has no roster data."));

            if (string.IsNullOrEmpty(wave.NextWave))
                messages.Add(Error($"{wave.WaveNo} next wave is empty."));
        }

        foreach (WaveEnemyRosterRow row in rosterRows.datas)
        {
            if (!IsDifficulty(row.WaveID, difficulty))
                continue;

            if (!waveIDs.Contains(row.WaveID))
                messages.Add(Error($"{row.WaveID} exists in roster data, but not in WaveData."));

            messages.AddRange(GetRosterValidationMessages(row));
        }

        return messages;
    }

    private List<ValidationMessage> GetUnknownDifficultyValidationMessages()
    {
        List<ValidationMessage> messages = new List<ValidationMessage>();

        foreach (WaveEnemyRosterRow row in rosterRows.datas)
        {
            if (IsKnownDifficulty(row.WaveID))
                continue;

            messages.Add(Error($"Unknown difficulty WaveID: {row.WaveID}"));
        }

        return messages;
    }

    private List<ValidationMessage> GetRosterValidationMessages(WaveEnemyRosterRow row)
    {
        List<ValidationMessage> messages = new List<ValidationMessage>();

        if (row == null)
        {
            messages.Add(Error("Roster row is null."));
            return messages;
        }

        if (string.IsNullOrEmpty(row.WaveID))
            messages.Add(Error("WaveID is empty."));

        if (row.SpawnOrder <= 0)
            messages.Add(Error($"{row.WaveID} spawn order must be greater than 0."));

        if (string.IsNullOrEmpty(row.Enemy_UID))
            messages.Add(Error($"{row.WaveID} enemy UID is empty."));
        else if (!HasEnemyUID(row.Enemy_UID))
            messages.Add(Error($"{row.WaveID} enemy UID does not exist in EnemyData: {row.Enemy_UID}"));

        if (row.EnemyLevel <= 0)
            messages.Add(Error($"{row.WaveID} enemy level must be greater than 0."));

        if (row.SpawnCount <= 0)
            messages.Add(Error($"{row.WaveID} spawn count must be greater than 0."));

        if (row.StartTime < 0)
            messages.Add(Error($"{row.WaveID} start time cannot be negative."));

        if (row.SpawnInterval < 0)
            messages.Add(Error($"{row.WaveID} spawn interval cannot be negative."));

        if (string.IsNullOrEmpty(row.SpawnType))
            messages.Add(Warning($"{row.WaveID} spawn type is empty."));

        return messages;
    }

    private string DrawPopupOrText(string label, string current, string[] options)
    {
        int index = Array.IndexOf(options, current);
        string[] displayOptions = new string[options.Length + 1];

        for (int i = 0; i < options.Length; i++)
            displayOptions[i] = options[i];

        displayOptions[displayOptions.Length - 1] = "Custom";

        int selectedIndex = index >= 0 ? index : displayOptions.Length - 1;
        selectedIndex = EditorGUILayout.Popup(label, selectedIndex, displayOptions);

        if (selectedIndex < options.Length)
            return options[selectedIndex];

        return EditorGUILayout.TextField($"{label} Custom", current);
    }

    private void LoadAll()
    {
        waveRows = LoadJson<WaveDataRowList>(WaveDataPath) ?? new WaveDataRowList();
        rosterRows = LoadJson<WaveEnemyRosterRowList>(WaveEnemyRosterDataPath) ?? new WaveEnemyRosterRowList();
        enemyRows = LoadJson<EnemyDataRowList>(EnemyDataPath) ?? new EnemyDataRowList();

        if (waveRows.datas == null)
            waveRows.datas = new List<WaveDataRow>();

        if (rosterRows.datas == null)
            rosterRows.datas = new List<WaveEnemyRosterRow>();

        if (enemyRows.datas == null)
            enemyRows.datas = new List<EnemyDataRow>();

        ClampSelectedWaveIndex();
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

        string json = File.ReadAllText(path);

        if (path == WaveEnemyRosterDataPath)
            json = NormalizeRosterJsonNumbers(json);

        return JsonUtility.FromJson<T>(json);
    }

    private string NormalizeRosterJsonNumbers(string json)
    {
        json = NormalizeFloatJsonField(json, "StartTime");
        json = NormalizeFloatJsonField(json, "SpawnInterval");
        json = NormalizeIntJsonField(json, "SpawnOrder");
        json = NormalizeIntJsonField(json, "EnemyLevel");
        json = NormalizeIntJsonField(json, "SpawnCount");

        return json;
    }

    private string NormalizeFloatJsonField(string json, string fieldName)
    {
        return Regex.Replace(json, $"\"{fieldName}\"\\s*:\\s*\"([^\"]*)\"", match =>
        {
            string valueText = match.Groups[1].Value;

            if (!float.TryParse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture, out float value) &&
                !float.TryParse(valueText, out value))
            {
                value = 0;
            }

            return $"\"{fieldName}\": {value.ToString(CultureInfo.InvariantCulture)}";
        });
    }

    private string NormalizeIntJsonField(string json, string fieldName)
    {
        return Regex.Replace(json, $"\"{fieldName}\"\\s*:\\s*\"([^\"]*)\"", match =>
        {
            string valueText = match.Groups[1].Value;

            if (!int.TryParse(valueText, out int value))
                value = 0;

            return $"\"{fieldName}\": {value}";
        });
    }

    private void SaveCurrentTab()
    {
        SortAllRosterRows();
        SaveJson(WaveEnemyRosterDataPath, rosterRows);
        isDirty = false;
        AssetDatabase.Refresh();
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

    private void AddRoster()
    {
        string waveID = GetSelectedWaveID();

        if (string.IsNullOrEmpty(waveID))
            return;

        rosterRows.datas.Add(new WaveEnemyRosterRow
        {
            WaveID = waveID,
            SpawnOrder = GetNextSpawnOrder(waveID),
            Enemy_UID = string.Empty,
            EnemyLevel = 1,
            SpawnCount = 1,
            StartTime = 0,
            SpawnInterval = 1,
            SpawnType = "Immediate"
        });

        MarkChanged();
    }

    private void DuplicateRoster(WaveEnemyRosterRow source)
    {
        if (source == null)
            return;

        rosterRows.datas.Add(new WaveEnemyRosterRow
        {
            WaveID = GetSelectedWaveID(),
            SpawnOrder = GetNextSpawnOrder(GetSelectedWaveID()),
            Enemy_UID = source.Enemy_UID,
            EnemyLevel = source.EnemyLevel,
            SpawnCount = source.SpawnCount,
            StartTime = source.StartTime,
            SpawnInterval = source.SpawnInterval,
            SpawnType = source.SpawnType
        });

        MarkChanged();
    }

    private void DeleteRoster(WaveEnemyRosterRow row)
    {
        if (row == null)
            return;

        bool confirm = EditorUtility.DisplayDialog("Delete Roster", $"Delete {row.WaveID} / Order {row.SpawnOrder}?", "Delete", "Cancel");

        if (!confirm)
            return;

        rosterRows.datas.Remove(row);
        MarkChanged();
    }

    private void SortSelectedWaveRoster()
    {
        SortAllRosterRows();
        MarkChanged();
    }

    private void SortAllRosterRows()
    {
        rosterRows.datas.Sort(CompareRosterRows);
    }

    private int CompareRosterRows(WaveEnemyRosterRow a, WaveEnemyRosterRow b)
    {
        int waveCompare = string.Compare(a.WaveID, b.WaveID, StringComparison.Ordinal);

        if (waveCompare != 0)
            return waveCompare;

        int orderCompare = a.SpawnOrder.CompareTo(b.SpawnOrder);

        if (orderCompare != 0)
            return orderCompare;

        return string.Compare(a.Enemy_UID, b.Enemy_UID, StringComparison.Ordinal);
    }

    private List<WaveDataRow> GetWavesByDifficulty(string difficulty)
    {
        List<WaveDataRow> waves = new List<WaveDataRow>();

        foreach (WaveDataRow row in waveRows.datas)
        {
            if (IsDifficulty(row.WaveNo, difficulty))
                waves.Add(row);
        }

        waves.Sort((a, b) => string.Compare(a.WaveNo, b.WaveNo, StringComparison.Ordinal));
        return waves;
    }

    private List<WaveEnemyRosterRow> GetRosterRows(string waveID)
    {
        List<WaveEnemyRosterRow> rows = new List<WaveEnemyRosterRow>();

        foreach (WaveEnemyRosterRow row in rosterRows.datas)
        {
            if (row.WaveID == waveID)
                rows.Add(row);
        }

        rows.Sort((a, b) => a.SpawnOrder.CompareTo(b.SpawnOrder));
        return rows;
    }

    private int GetRosterWaveCountByDifficulty(string difficulty)
    {
        HashSet<string> waveIDs = new HashSet<string>();

        foreach (WaveEnemyRosterRow row in rosterRows.datas)
        {
            if (IsDifficulty(row.WaveID, difficulty))
                waveIDs.Add(row.WaveID);
        }

        return waveIDs.Count;
    }

    private int GetNextSpawnOrder(string waveID)
    {
        int max = 0;

        foreach (WaveEnemyRosterRow row in rosterRows.datas)
        {
            if (row.WaveID != waveID)
                continue;

            max = Mathf.Max(max, row.SpawnOrder);
        }

        return max + 1;
    }

    private bool HasEnemyUID(string enemyUID)
    {
        foreach (EnemyDataRow row in enemyRows.datas)
        {
            if (row.Enemy_UID == enemyUID)
                return true;
        }

        return false;
    }

    private WaveDataRow GetSelectedWave()
    {
        List<WaveDataRow> waves = GetWavesByDifficulty(GetSelectedDifficulty());

        if (selectedWaveIndex < 0 || selectedWaveIndex >= waves.Count)
            return null;

        return waves[selectedWaveIndex];
    }

    private string GetSelectedWaveID()
    {
        WaveDataRow wave = GetSelectedWave();
        return wave != null ? wave.WaveNo : string.Empty;
    }

    private string GetSelectedDifficulty()
    {
        return difficultyPrefixes[Mathf.Clamp(selectedTab, 0, difficultyPrefixes.Length - 1)];
    }

    private bool HasSelectedWave()
    {
        return GetSelectedWave() != null;
    }

    private void ClampSelectedWaveIndex()
    {
        List<WaveDataRow> waves = GetWavesByDifficulty(GetSelectedDifficulty());
        selectedWaveIndex = waves.Count > 0 ? Mathf.Clamp(selectedWaveIndex, 0, waves.Count - 1) : -1;
    }

    private bool IsDifficulty(string waveID, string difficulty)
    {
        return !string.IsNullOrEmpty(waveID) && waveID.StartsWith($"{difficulty}_", StringComparison.OrdinalIgnoreCase);
    }

    private bool IsKnownDifficulty(string waveID)
    {
        foreach (string difficulty in difficultyPrefixes)
        {
            if (IsDifficulty(waveID, difficulty))
                return true;
        }

        return false;
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

    private ValidationMessage Error(string text) => new ValidationMessage(text, MessageType.Error);
    private ValidationMessage Warning(string text) => new ValidationMessage(text, MessageType.Warning);

    private readonly struct ValidationMessage
    {
        public readonly string Text;
        public readonly MessageType Type;

        public ValidationMessage(string text, MessageType type)
        {
            Text = text;
            Type = type;
        }
    }
}
