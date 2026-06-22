using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TowerTableEditorWindow : EditorWindow
{
    private const string TowerDataPath = "Assets/Resources/Data/TowerData.json";
    private const string TowerSkillDataPath = "Assets/Resources/Data/TowerSkillData.json";
    private const string TowerSessionUpgradeDataPath = "Assets/Resources/Data/TowerSessionUpgradeData.json";

    private readonly string[] tabs = { "Tower", "Tower Skill", "Session Upgrade", "Validate" };
    private readonly string[] bossApplyOptions = { "N", "Y" };
    private readonly string[] bossModifierOptions = { "None", "0", "0.25", "0.5", "1" };

    private TowerDataRowList towerRows = new TowerDataRowList();
    private TowerSkillDataRowList skillRows = new TowerSkillDataRowList();
    private TowerSessionUpgradeDataRowList sessionRows = new TowerSessionUpgradeDataRowList();

    private int selectedTab;
    private int selectedTowerIndex = -1;
    private int selectedSkillIndex = -1;
    private int selectedSessionIndex = -1;
    private string searchText = string.Empty;
    private bool autoSave = true;
    private bool isDirty;
    private int previewFontSize = 12;

    private Vector2 listScroll;
    private Vector2 inspectorScroll;
    private Vector2 validateScroll;
    private GUIStyle previewBoxStyle;

    [MenuItem("Tools/Tower Table Editor")]
    public static void Open()
    {
        GetWindow<TowerTableEditorWindow>("Tower Table");
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

        if (selectedTab == 3)
        {
            DrawValidateTab();
            return;
        }

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

        using (new EditorGUI.DisabledScope(!isDirty || selectedTab == 3))
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

        if (selectedTab == 0 && GUILayout.Button("Add Tower", EditorStyles.toolbarButton, GUILayout.Width(90)))
            AddTower();

        if (selectedTab == 1 && GUILayout.Button("Add Skill", EditorStyles.toolbarButton, GUILayout.Width(90)))
            AddSkill();

        if (selectedTab == 2 && GUILayout.Button("Add Upgrade", EditorStyles.toolbarButton, GUILayout.Width(100)))
            AddSessionUpgrade();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(280));

        EditorGUILayout.LabelField(tabs[selectedTab], EditorStyles.boldLabel);
        listScroll = EditorGUILayout.BeginScrollView(listScroll, "box");

        switch (selectedTab)
        {
            case 0:
                DrawTowerList();
                break;
            case 1:
                DrawSkillList();
                break;
            case 2:
                DrawSessionList();
                break;
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawTowerList()
    {
        for (int i = 0; i < towerRows.datas.Count; i++)
        {
            TowerDataRow row = towerRows.datas[i];

            if (!IsMatchSearch(row.TowerUID, row.TowerType, row.StringKey, row.SkillID))
                continue;

            GUIStyle style = selectedTowerIndex == i ? EditorStyles.boldLabel : EditorStyles.label;
            string label = $"{row.TowerUID} [{row.TowerType} G{row.Grade}]";

            if (GUILayout.Button(label, style, GUILayout.Height(22)))
                selectedTowerIndex = i;
        }
    }

    private void DrawSkillList()
    {
        for (int i = 0; i < skillRows.datas.Count; i++)
        {
            TowerSkillDataRow row = skillRows.datas[i];

            if (!IsMatchSearch(row.Tower_Skill_UID, row.Type, row.EffectType, row.String_Key))
                continue;

            GUIStyle style = selectedSkillIndex == i ? EditorStyles.boldLabel : EditorStyles.label;
            string label = $"{row.Tower_Skill_UID} [{row.Type} Step {row.Step}]";

            if (GUILayout.Button(label, style, GUILayout.Height(22)))
                selectedSkillIndex = i;
        }
    }

    private void DrawSessionList()
    {
        for (int i = 0; i < sessionRows.datas.Count; i++)
        {
            TowerSessionUpgradeDataRow row = sessionRows.datas[i];

            if (!IsMatchSearch(row.Tower_UID, row.Upgrade_Type, row.Tower_Grade.ToString()))
                continue;

            GUIStyle style = selectedSessionIndex == i ? EditorStyles.boldLabel : EditorStyles.label;
            string label = $"{row.Tower_UID} G{row.Tower_Grade} [{row.Upgrade_Type}]";

            if (GUILayout.Button(label, style, GUILayout.Height(22)))
                selectedSessionIndex = i;
        }
    }

    private void DrawInspector()
    {
        EditorGUILayout.BeginVertical();
        inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll, "box");

        switch (selectedTab)
        {
            case 0:
                DrawTowerInspector();
                break;
            case 1:
                DrawSkillInspector();
                break;
            case 2:
                DrawSessionInspector();
                break;
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawTowerInspector()
    {
        if (selectedTowerIndex < 0 || selectedTowerIndex >= towerRows.datas.Count)
        {
            EditorGUILayout.HelpBox("Select a tower row.", MessageType.Info);
            return;
        }

        TowerDataRow row = towerRows.datas[selectedTowerIndex];

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(row.TowerUID, EditorStyles.boldLabel);

        if (GUILayout.Button("Duplicate", GUILayout.Width(90)))
            DuplicateTower(row);

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteTower();
            EditorGUILayout.EndHorizontal();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        DrawTowerIconPreview(row);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        row.TowerUID = EditorGUILayout.TextField("Tower UID", row.TowerUID);
        row.TowerType = DrawTowerTypeField("Tower Type", row.TowerType);
        row.StringKey = EditorGUILayout.TextField("String Key", row.StringKey);
        row.Grade = EditorGUILayout.IntField("Grade", row.Grade);
        row.BaseAtk = EditorGUILayout.IntField("Base Atk", row.BaseAtk);
        row.BaseAtkSpeed = EditorGUILayout.FloatField("Base Atk Speed", row.BaseAtkSpeed);
        row.Range = EditorGUILayout.FloatField("Range", row.Range);
        row.CostType = DrawCostTypeField("Cost Type", row.CostType);
        row.BuyPrice = EditorGUILayout.IntField("Buy Price", row.BuyPrice);
        row.SellPrice = EditorGUILayout.IntField("Sell Price", row.SellPrice);
        row.SkillID = DrawSkillUIDField(row.SkillID);
        row.IconPath = EditorGUILayout.TextField("Icon Path", row.IconPath);
        row.NextGradeUID = DrawNextGradeUIDField(row.NextGradeUID);

        if (EditorGUI.EndChangeCheck())
            MarkChanged();

        DrawTowerPreview(row);
    }

    private void DrawSkillInspector()
    {
        if (selectedSkillIndex < 0 || selectedSkillIndex >= skillRows.datas.Count)
        {
            EditorGUILayout.HelpBox("Select a tower skill row.", MessageType.Info);
            return;
        }

        TowerSkillDataRow row = skillRows.datas[selectedSkillIndex];

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(row.Tower_Skill_UID, EditorStyles.boldLabel);

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

        row.Tower_Skill_UID = EditorGUILayout.TextField("Skill UID", row.Tower_Skill_UID);
        row.String_Key = EditorGUILayout.TextField("String Key", row.String_Key);
        row.Des_String_Key = EditorGUILayout.TextField("Description Key", row.Des_String_Key);
        row.Type = DrawTowerTypeField("Type", row.Type);
        row.Step = EditorGUILayout.IntField("Step", row.Step);
        row.RequiredCount = EditorGUILayout.IntField("Required Count", row.RequiredCount);
        row.RequiredTowerGrade = EditorGUILayout.IntField("Required Tower Grade", row.RequiredTowerGrade);
        row.EffectType = DrawSkillEffectTypeField("Effect Type", row.EffectType);
        row.EffectValue = EditorGUILayout.IntField("Effect Value", row.EffectValue);
        row.EffectValueUnit = DrawEffectValueUnitField("Effect Value Unit", row.EffectValueUnit);
        row.Duration = EditorGUILayout.FloatField("Duration", row.Duration);
        row.BossApply = DrawPopupOrText("Boss Apply", row.BossApply, bossApplyOptions);
        row.BossModifier = DrawPopupOrText("Boss Modifier", row.BossModifier, bossModifierOptions);
        row.Icon_UID = EditorGUILayout.TextField("Icon UID", row.Icon_UID);
        row.Note = EditorGUILayout.TextField("Note", row.Note);

        if (EditorGUI.EndChangeCheck())
            MarkChanged();

        DrawSkillPreview(row);
    }

    private void DrawSessionInspector()
    {
        if (selectedSessionIndex < 0 || selectedSessionIndex >= sessionRows.datas.Count)
        {
            EditorGUILayout.HelpBox("Select a session upgrade row.", MessageType.Info);
            return;
        }

        TowerSessionUpgradeDataRow row = sessionRows.datas[selectedSessionIndex];

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"{row.Tower_UID} {row.Upgrade_Type}", EditorStyles.boldLabel);

        if (GUILayout.Button("Duplicate", GUILayout.Width(90)))
            DuplicateSessionUpgrade(row);

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteSessionUpgrade();
            EditorGUILayout.EndHorizontal();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        row.Tower_UID = DrawTowerUIDField(row.Tower_UID);
        row.Tower_Grade = EditorGUILayout.IntField("Tower Grade", row.Tower_Grade);
        row.Upgrade_Type = DrawUpgradeTypeField("Upgrade Type", row.Upgrade_Type);
        row.Increase_Value = EditorGUILayout.FloatField("Increase Value", row.Increase_Value);
        row.Base_Cost = EditorGUILayout.IntField("Base Cost", row.Base_Cost);
        row.Increase_Cost = EditorGUILayout.IntField("Increase Cost", row.Increase_Cost);

        if (EditorGUI.EndChangeCheck())
            MarkChanged();

        DrawSessionPreview(row);
    }

    private void DrawTowerPreview(TowerDataRow row)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Preview", EditorStyles.boldLabel);

        string preview =
            $"UID: {row.TowerUID}\n" +
            $"Type/Grade: {row.TowerType} / {row.Grade}\n" +
            $"Upgrade Chain: {row.TowerUID} -> {row.NextGradeUID}\n" +
            $"Icon Resource: Tower/Images/Icon_Tower_{row.IconPath}_{row.Grade}_Idle\n" +
            $"SpriteLibrary: Tower/SpriteLibrary/{row.IconPath}/{row.IconPath}_{row.Grade}";

        EditorGUILayout.LabelField(preview, GetPreviewBoxStyle());
    }

    private void DrawTowerIconPreview(TowerDataRow row)
    {
        EditorGUILayout.LabelField("Icon Preview", EditorStyles.boldLabel);

        Sprite icon = LoadTowerIcon(row);
        Texture texture = GetSpritePreviewTexture(icon);

        Rect rect = GUILayoutUtility.GetRect(72, 72, GUILayout.Width(72), GUILayout.Height(72));

        if (texture != null)
        {
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
        }
        else
        {
            EditorGUI.DrawRect(rect, new Color(0.2f, 0.2f, 0.2f, 0.35f));
            EditorGUI.LabelField(rect, "No Icon", EditorStyles.centeredGreyMiniLabel);
        }

        EditorGUILayout.SelectableLabel(GetTowerIconResourcePath(row), EditorStyles.miniLabel, GUILayout.Height(16));
    }

    private void DrawSkillPreview(TowerSkillDataRow row)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Preview", EditorStyles.boldLabel);

        string preview =
            $"Condition: {row.Type} count >= {row.RequiredCount}, grade >= {row.RequiredTowerGrade}\n" +
            $"Effect: {row.EffectType} {row.EffectValue} {row.EffectValueUnit}\n" +
            $"Duration: {row.Duration}\n" +
            $"Boss: {row.BossApply}, Modifier: {row.BossModifier}";

        EditorGUILayout.LabelField(preview, GetPreviewBoxStyle());
    }

    private void DrawSessionPreview(TowerSessionUpgradeDataRow row)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Preview", EditorStyles.boldLabel);

        string preview =
            $"Cost = {row.Base_Cost} + {row.Increase_Cost} * CurrentStep\n" +
            $"Increase = {row.Increase_Value} * CurrentStep";

        EditorGUILayout.LabelField(preview, GetPreviewBoxStyle());
    }

    private void DrawValidateTab()
    {
        validateScroll = EditorGUILayout.BeginScrollView(validateScroll, "box");

        List<ValidationMessage> messages = CollectValidationMessages();

        if (messages.Count == 0)
        {
            EditorGUILayout.HelpBox("No validation issues found.", MessageType.Info);
        }
        else
        {
            foreach (ValidationMessage message in messages)
            {
                EditorGUILayout.HelpBox(message.Text, message.Type);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private List<ValidationMessage> CollectValidationMessages()
    {
        List<ValidationMessage> messages = new List<ValidationMessage>();
        HashSet<string> towerUIDs = new HashSet<string>();
        HashSet<string> skillUIDs = new HashSet<string>();
        HashSet<string> sessionKeys = new HashSet<string>();
        Dictionary<string, TowerDataRow> towerMap = new Dictionary<string, TowerDataRow>();

        foreach (TowerDataRow tower in towerRows.datas)
        {
            if (!towerUIDs.Add(tower.TowerUID))
                messages.Add(Error($"Duplicate TowerUID: {tower.TowerUID}"));
            else
                towerMap[tower.TowerUID] = tower;

            if (!Enum.TryParse(tower.TowerType, true, out TowerType _))
                messages.Add(Error($"{tower.TowerUID} has invalid TowerType: {tower.TowerType}"));

            if (!Enum.TryParse(tower.CostType, true, out CostType _))
                messages.Add(Error($"{tower.TowerUID} has invalid CostType: {tower.CostType}"));
        }

        foreach (TowerSkillDataRow skill in skillRows.datas)
        {
            if (!skillUIDs.Add(skill.Tower_Skill_UID))
                messages.Add(Error($"Duplicate TowerSkill UID: {skill.Tower_Skill_UID}"));

            if (!Enum.TryParse(skill.Type, true, out TowerType _))
                messages.Add(Error($"{skill.Tower_Skill_UID} has invalid Type: {skill.Type}"));

            if (!Enum.TryParse(skill.EffectType, true, out SkillEffectType _))
                messages.Add(Error($"{skill.Tower_Skill_UID} has invalid EffectType: {skill.EffectType}"));

            if (!Enum.TryParse(skill.EffectValueUnit, true, out EffectValueUnit _))
                messages.Add(Error($"{skill.Tower_Skill_UID} has invalid EffectValueUnit: {skill.EffectValueUnit}"));
        }

        foreach (TowerDataRow tower in towerRows.datas)
        {
            if (!skillUIDs.Contains(tower.SkillID))
                messages.Add(Error($"{tower.TowerUID} references missing SkillID: {tower.SkillID}"));

            if (!string.Equals(tower.NextGradeUID, "MASTER", StringComparison.OrdinalIgnoreCase) &&
                !towerUIDs.Contains(tower.NextGradeUID))
            {
                messages.Add(Error($"{tower.TowerUID} references missing NextGradeUID: {tower.NextGradeUID}"));
            }
        }

        foreach (TowerSessionUpgradeDataRow session in sessionRows.datas)
        {
            string key = $"{session.Tower_UID}:{session.Upgrade_Type}";
            if (!sessionKeys.Add(key))
                messages.Add(Error($"Duplicate session upgrade row: {key}"));

            if (!towerMap.TryGetValue(session.Tower_UID, out TowerDataRow tower))
            {
                messages.Add(Error($"Session upgrade references missing Tower_UID: {session.Tower_UID}"));
                continue;
            }

            if (session.Tower_Grade != tower.Grade)
            {
                messages.Add(Error($"{session.Tower_UID} {session.Upgrade_Type} has Tower_Grade {session.Tower_Grade}, expected {tower.Grade}."));
            }

            if (!Enum.TryParse(session.Upgrade_Type, true, out UpgradeType _))
                messages.Add(Error($"{session.Tower_UID} has invalid Upgrade_Type: {session.Upgrade_Type}"));
        }

        foreach (TowerDataRow tower in towerRows.datas)
        {
            if (!sessionKeys.Contains($"{tower.TowerUID}:Damge"))
                messages.Add(Warning($"{tower.TowerUID} is missing Damge session upgrade row."));

            if (!sessionKeys.Contains($"{tower.TowerUID}:Speed"))
                messages.Add(Warning($"{tower.TowerUID} is missing Speed session upgrade row."));
        }

        foreach (TowerType type in Enum.GetValues(typeof(TowerType)))
        {
            for (int grade = 1; grade <= 6; grade++)
            {
                bool exists = towerRows.datas.Exists(x => x.TowerType == type.ToString() && x.Grade == grade);
                if (!exists)
                    messages.Add(Warning($"{type} grade {grade} tower data is missing."));
            }
        }

        return messages;
    }

    private string DrawTowerUIDField(string current)
    {
        List<string> uids = towerRows.datas.ConvertAll(x => x.TowerUID);
        return DrawPopupListOrText("Tower UID", current, uids);
    }

    private string GetTowerIconResourcePath(TowerDataRow row)
    {
        if (row == null)
            return string.Empty;

        return $"Tower/Images/Icon_Tower_{row.IconPath}_{row.Grade}_Idle";
    }

    private Sprite LoadTowerIcon(TowerDataRow row)
    {
        string path = GetTowerIconResourcePath(row);

        if (string.IsNullOrEmpty(path))
            return null;

        return Resources.Load<Sprite>(path);
    }

    private Texture GetSpritePreviewTexture(Sprite sprite)
    {
        if (sprite == null)
            return null;

        Texture texture = AssetPreview.GetAssetPreview(sprite);

        if (texture != null)
            return texture;

        return sprite.texture;
    }

    private string DrawSkillUIDField(string current)
    {
        List<string> uids = skillRows.datas.ConvertAll(x => x.Tower_Skill_UID);
        return DrawPopupListOrText("Skill ID", current, uids);
    }

    private string DrawNextGradeUIDField(string current)
    {
        List<string> uids = towerRows.datas.ConvertAll(x => x.TowerUID);
        uids.Insert(0, "MASTER");
        return DrawPopupListOrText("Next Grade UID", current, uids);
    }

    private string DrawPopupListOrText(string label, string current, List<string> values)
    {
        if (values.Count == 0)
            return EditorGUILayout.TextField(label, current);

        int index = values.IndexOf(current);

        if (index < 0)
        {
            EditorGUILayout.HelpBox($"Current value does not exist in list: {current}", MessageType.Warning);
            return EditorGUILayout.TextField(label, current);
        }

        int nextIndex = EditorGUILayout.Popup(label, index, values.ToArray());
        return values[nextIndex];
    }

    private string DrawPopupOrText(string label, string current, string[] values)
    {
        int index = Array.IndexOf(values, current);

        if (index < 0)
            return EditorGUILayout.TextField(label, current);

        int next = EditorGUILayout.Popup(label, index, values);
        return values[next];
    }

    private string DrawTowerTypeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out TowerType value))
            value = TowerType.Human;

        value = (TowerType)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawCostTypeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out CostType value))
            value = CostType.Gold;

        value = (CostType)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawSkillEffectTypeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out SkillEffectType value))
            value = SkillEffectType.AtkDamage;

        value = (SkillEffectType)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawEffectValueUnitField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out EffectValueUnit value))
            value = EffectValueUnit.Flat;

        value = (EffectValueUnit)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawUpgradeTypeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out UpgradeType value))
            value = UpgradeType.Damge;

        value = (UpgradeType)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private void LoadAll()
    {
        towerRows = LoadJson<TowerDataRowList>(TowerDataPath) ?? new TowerDataRowList();
        skillRows = LoadJson<TowerSkillDataRowList>(TowerSkillDataPath) ?? new TowerSkillDataRowList();
        sessionRows = LoadJson<TowerSessionUpgradeDataRowList>(TowerSessionUpgradeDataPath) ?? new TowerSessionUpgradeDataRowList();

        selectedTowerIndex = towerRows.datas.Count > 0 ? Mathf.Clamp(selectedTowerIndex, 0, towerRows.datas.Count - 1) : -1;
        selectedSkillIndex = skillRows.datas.Count > 0 ? Mathf.Clamp(selectedSkillIndex, 0, skillRows.datas.Count - 1) : -1;
        selectedSessionIndex = sessionRows.datas.Count > 0 ? Mathf.Clamp(selectedSessionIndex, 0, sessionRows.datas.Count - 1) : -1;

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
        switch (selectedTab)
        {
            case 0:
                SaveJson(TowerDataPath, towerRows);
                break;
            case 1:
                SaveJson(TowerSkillDataPath, skillRows);
                break;
            case 2:
                SaveJson(TowerSessionUpgradeDataPath, sessionRows);
                break;
        }

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

    private void AddTower()
    {
        string uid = GetNextUID("T", towerRows.datas.ConvertAll(x => x.TowerUID), 4);

        towerRows.datas.Add(new TowerDataRow
        {
            TowerUID = uid,
            TowerType = TowerType.Human.ToString(),
            StringKey = "TOWER_NAME_NEW",
            Grade = 1,
            BaseAtk = 10,
            BaseAtkSpeed = 1f,
            Range = 5f,
            CostType = CostType.Gold.ToString(),
            BuyPrice = 10,
            SellPrice = 5,
            SkillID = skillRows.datas.Count > 0 ? skillRows.datas[0].Tower_Skill_UID : string.Empty,
            IconPath = TowerType.Human.ToString(),
            NextGradeUID = "MASTER"
        });

        selectedTowerIndex = towerRows.datas.Count - 1;
        MarkChanged();
    }

    private void AddSkill()
    {
        string uid = GetNextUID("TS", skillRows.datas.ConvertAll(x => x.Tower_Skill_UID), 4);

        skillRows.datas.Add(new TowerSkillDataRow
        {
            Tower_Skill_UID = uid,
            String_Key = "TOWER_SKILL_NEW",
            Des_String_Key = "TOWER_SKILL_DES_NEW",
            Type = TowerType.Human.ToString(),
            Step = 1,
            RequiredCount = 3,
            RequiredTowerGrade = 0,
            EffectType = SkillEffectType.AtkDamage.ToString(),
            EffectValue = 1,
            EffectValueUnit = EffectValueUnit.Flat.ToString(),
            Duration = 0,
            BossApply = "N",
            BossModifier = "None",
            Icon_UID = "ICON_TOWER_SKILL_NEW",
            Note = string.Empty
        });

        selectedSkillIndex = skillRows.datas.Count - 1;
        MarkChanged();
    }

    private void AddSessionUpgrade()
    {
        string towerUID = towerRows.datas.Count > 0 ? towerRows.datas[0].TowerUID : string.Empty;
        int grade = towerRows.datas.Count > 0 ? towerRows.datas[0].Grade : 1;

        sessionRows.datas.Add(new TowerSessionUpgradeDataRow
        {
            Tower_UID = towerUID,
            Tower_Grade = grade,
            Upgrade_Type = UpgradeType.Damge.ToString(),
            Increase_Value = 1,
            Base_Cost = 100,
            Increase_Cost = 50
        });

        selectedSessionIndex = sessionRows.datas.Count - 1;
        MarkChanged();
    }

    private void DuplicateTower(TowerDataRow source)
    {
        string uid = GetNextUID("T", towerRows.datas.ConvertAll(x => x.TowerUID), 4);

        towerRows.datas.Add(new TowerDataRow
        {
            TowerUID = uid,
            TowerType = source.TowerType,
            StringKey = source.StringKey,
            Grade = source.Grade,
            BaseAtk = source.BaseAtk,
            BaseAtkSpeed = source.BaseAtkSpeed,
            Range = source.Range,
            CostType = source.CostType,
            BuyPrice = source.BuyPrice,
            SellPrice = source.SellPrice,
            SkillID = source.SkillID,
            IconPath = source.IconPath,
            NextGradeUID = source.NextGradeUID
        });

        selectedTowerIndex = towerRows.datas.Count - 1;
        MarkChanged();
    }

    private void DuplicateSkill(TowerSkillDataRow source)
    {
        string uid = GetNextUID("TS", skillRows.datas.ConvertAll(x => x.Tower_Skill_UID), 4);

        skillRows.datas.Add(new TowerSkillDataRow
        {
            Tower_Skill_UID = uid,
            String_Key = source.String_Key,
            Des_String_Key = source.Des_String_Key,
            Type = source.Type,
            Step = source.Step,
            RequiredCount = source.RequiredCount,
            RequiredTowerGrade = source.RequiredTowerGrade,
            EffectType = source.EffectType,
            EffectValue = source.EffectValue,
            EffectValueUnit = source.EffectValueUnit,
            Duration = source.Duration,
            BossApply = source.BossApply,
            BossModifier = source.BossModifier,
            Icon_UID = source.Icon_UID,
            Note = source.Note
        });

        selectedSkillIndex = skillRows.datas.Count - 1;
        MarkChanged();
    }

    private void DuplicateSessionUpgrade(TowerSessionUpgradeDataRow source)
    {
        sessionRows.datas.Add(new TowerSessionUpgradeDataRow
        {
            Tower_UID = source.Tower_UID,
            Tower_Grade = source.Tower_Grade,
            Upgrade_Type = source.Upgrade_Type,
            Increase_Value = source.Increase_Value,
            Base_Cost = source.Base_Cost,
            Increase_Cost = source.Increase_Cost
        });

        selectedSessionIndex = sessionRows.datas.Count - 1;
        MarkChanged();
    }

    private void DeleteTower()
    {
        if (selectedTowerIndex < 0 || selectedTowerIndex >= towerRows.datas.Count)
            return;

        TowerDataRow row = towerRows.datas[selectedTowerIndex];
        bool confirm = EditorUtility.DisplayDialog("Delete Tower", $"Delete {row.TowerUID}?", "Delete", "Cancel");
        if (!confirm)
            return;

        towerRows.datas.RemoveAt(selectedTowerIndex);
        selectedTowerIndex = towerRows.datas.Count > 0 ? Mathf.Clamp(selectedTowerIndex, 0, towerRows.datas.Count - 1) : -1;
        MarkChanged();
    }

    private void DeleteSkill()
    {
        if (selectedSkillIndex < 0 || selectedSkillIndex >= skillRows.datas.Count)
            return;

        TowerSkillDataRow row = skillRows.datas[selectedSkillIndex];
        bool confirm = EditorUtility.DisplayDialog("Delete Skill", $"Delete {row.Tower_Skill_UID}?", "Delete", "Cancel");
        if (!confirm)
            return;

        skillRows.datas.RemoveAt(selectedSkillIndex);
        selectedSkillIndex = skillRows.datas.Count > 0 ? Mathf.Clamp(selectedSkillIndex, 0, skillRows.datas.Count - 1) : -1;
        MarkChanged();
    }

    private void DeleteSessionUpgrade()
    {
        if (selectedSessionIndex < 0 || selectedSessionIndex >= sessionRows.datas.Count)
            return;

        TowerSessionUpgradeDataRow row = sessionRows.datas[selectedSessionIndex];
        bool confirm = EditorUtility.DisplayDialog("Delete Session Upgrade", $"Delete {row.Tower_UID} {row.Upgrade_Type}?", "Delete", "Cancel");
        if (!confirm)
            return;

        sessionRows.datas.RemoveAt(selectedSessionIndex);
        selectedSessionIndex = sessionRows.datas.Count > 0 ? Mathf.Clamp(selectedSessionIndex, 0, sessionRows.datas.Count - 1) : -1;
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
