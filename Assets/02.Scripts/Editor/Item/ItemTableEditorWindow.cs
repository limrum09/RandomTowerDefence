using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ItemTableEditorWindow : EditorWindow
{
    private const string ItemDataPath = "Assets/Resources/Data/ItemData.json";

    private readonly string[] tabs = { "Item", "Validate" };
    private readonly HashSet<ItemOptions> implementedApplyOptions = new HashSet<ItemOptions>
    {
        ItemOptions.AtkDamageUP,
        ItemOptions.AtkSpeedUp,
        ItemOptions.GoldDropIncrease,
        ItemOptions.HealLife,
        ItemOptions.RandomGold,
        ItemOptions.InterestBoost
    };

    private readonly HashSet<ItemOptions> implementedRemoveOptions = new HashSet<ItemOptions>
    {
        ItemOptions.AtkDamageUP,
        ItemOptions.AtkSpeedUp,
        ItemOptions.GoldDropIncrease,
        ItemOptions.InterestBoost
    };

    private ItemDataRowList itemRows = new ItemDataRowList();

    private int selectedTab;
    private int selectedItemIndex = -1;
    private string searchText = string.Empty;
    private bool autoSave = true;
    private bool isDirty;
    private int previewFontSize = 12;

    private Vector2 listScroll;
    private Vector2 inspectorScroll;
    private Vector2 validateScroll;
    private GUIStyle previewBoxStyle;

    [MenuItem("Tools/Item Table Editor")]
    public static void Open()
    {
        GetWindow<ItemTableEditorWindow>("Item Table");
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

        if (selectedTab == 1)
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

        using (new EditorGUI.DisabledScope(!isDirty || selectedTab == 1))
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

        if (selectedTab == 0 && GUILayout.Button("Add Item", EditorStyles.toolbarButton, GUILayout.Width(90)))
            AddItem();

        EditorGUILayout.EndHorizontal();
    }

    private void DrawList()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(300));

        EditorGUILayout.LabelField("Item List", EditorStyles.boldLabel);
        listScroll = EditorGUILayout.BeginScrollView(listScroll, "box");

        for (int i = 0; i < itemRows.datas.Count; i++)
        {
            ItemDataRow row = itemRows.datas[i];

            if (!IsMatchSearch(row.Item_UID, row.Item_Name, row.Grade, row.Item_Option, row.String_Key))
                continue;

            GUIStyle style = selectedItemIndex == i ? EditorStyles.boldLabel : EditorStyles.label;
            string label = $"{row.Item_UID} [{row.Grade}] {row.Item_Name}";

            if (GUILayout.Button(label, style, GUILayout.Height(22)))
                selectedItemIndex = i;
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawInspector()
    {
        EditorGUILayout.BeginVertical();
        inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll, "box");
        DrawItemInspector();
        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawItemInspector()
    {
        if (selectedItemIndex < 0 || selectedItemIndex >= itemRows.datas.Count)
        {
            EditorGUILayout.HelpBox("Select an item row.", MessageType.Info);
            return;
        }

        ItemDataRow row = itemRows.datas[selectedItemIndex];

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(row.Item_UID, EditorStyles.boldLabel);

        if (GUILayout.Button("Duplicate", GUILayout.Width(90)))
            DuplicateItem(row);

        if (GUILayout.Button("Delete", GUILayout.Width(70)))
        {
            DeleteItem();
            EditorGUILayout.EndHorizontal();
            return;
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        DrawItemIconPreview(row);
        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();

        row.Item_UID = EditorGUILayout.TextField("Item UID", row.Item_UID);
        row.Item_Name = EditorGUILayout.TextField("Item Name", row.Item_Name);
        row.Grade = DrawItemGradeField("Grade", row.Grade);
        row.Item_Option = DrawItemOptionField("Item Option", row.Item_Option);
        row.Target = DrawItemTargetField("Target", row.Target);
        row.Scope_Range = DrawScopeRangeField("Scope Range", row.Scope_Range);
        row.Value = EditorGUILayout.IntField("Value", row.Value);
        row.Buy_Price = EditorGUILayout.IntField("Buy Price", row.Buy_Price);
        row.Sale_Price = EditorGUILayout.IntField("Sale Price", row.Sale_Price);
        row.String_Key = EditorGUILayout.TextField("String Key", row.String_Key);
        row.Item_Desc = EditorGUILayout.TextField("Item Desc", row.Item_Desc);
        row.Icon_UID = EditorGUILayout.TextField("Icon UID", row.Icon_UID);

        if (EditorGUI.EndChangeCheck())
            MarkChanged();

        DrawItemPreview(row);
    }

    private void DrawItemIconPreview(ItemDataRow row)
    {
        EditorGUILayout.LabelField("Icon Preview", EditorStyles.boldLabel);

        Sprite icon = LoadItemIcon(row);
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

        EditorGUILayout.SelectableLabel(GetItemIconResourcePath(row), EditorStyles.miniLabel, GUILayout.Height(16));
    }

    private void DrawItemPreview(ItemDataRow row)
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Runtime Preview", EditorStyles.boldLabel);

        string preview =
            $"Icon Resource: {GetItemIconResourcePath(row)}\n" +
            $"Effect: {row.Item_Option} {row.Value}\n" +
            $"Target: {row.Target} / {row.Scope_Range}\n" +
            $"Buy/Sell: {row.Buy_Price} / {row.Sale_Price}";

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
                EditorGUILayout.HelpBox(message.Text, message.Type);
        }

        EditorGUILayout.EndScrollView();
    }

    private List<ValidationMessage> CollectValidationMessages()
    {
        List<ValidationMessage> messages = new List<ValidationMessage>();
        HashSet<string> itemUIDs = new HashSet<string>();

        foreach (ItemDataRow row in itemRows.datas)
        {
            if (!itemUIDs.Add(row.Item_UID))
                messages.Add(Error($"Duplicate Item_UID: {row.Item_UID}"));

            if (!Enum.TryParse(row.Grade, true, out ItemGrade grade))
                messages.Add(Error($"{row.Item_UID} has invalid Grade: {row.Grade}"));

            if (!Enum.TryParse(row.Item_Option, true, out ItemOptions option))
            {
                messages.Add(Error($"{row.Item_UID} has invalid Item_Option: {row.Item_Option}"));
            }
            else
            {
                if (!implementedApplyOptions.Contains(option))
                    messages.Add(Warning($"{row.Item_UID} option {option} is not handled in ApplyItemEffect."));

                if (!implementedRemoveOptions.Contains(option) && row.Target != ItemTarget.System.ToString())
                    messages.Add(Warning($"{row.Item_UID} option {option} may need RemoveItemEffect handling."));
            }

            if (!Enum.TryParse(row.Target, true, out ItemTarget target))
                messages.Add(Error($"{row.Item_UID} has invalid Target: {row.Target}"));

            if (!Enum.TryParse(row.Scope_Range, true, out ScopeRange scope))
                messages.Add(Error($"{row.Item_UID} has invalid Scope_Range: {row.Scope_Range}"));

            if (string.IsNullOrWhiteSpace(row.Icon_UID))
                messages.Add(Error($"{row.Item_UID} has empty Icon_UID."));
            else if (LoadItemIcon(row) == null)
                messages.Add(Warning($"{row.Item_UID} icon not found: {GetItemIconResourcePath(row)}"));

            if (row.Buy_Price < 0)
                messages.Add(Error($"{row.Item_UID} has negative Buy_Price."));

            if (row.Sale_Price < 0)
                messages.Add(Error($"{row.Item_UID} has negative Sale_Price."));

            if (target == ItemTarget.Tower && scope == ScopeRange.Global)
                messages.Add(Warning($"{row.Item_UID} targets Tower but Scope_Range is Global."));

            if (target == ItemTarget.System && IsTowerScope(scope))
                messages.Add(Warning($"{row.Item_UID} targets System but Scope_Range is tower-specific: {scope}."));
        }

        return messages;
    }

    private bool IsTowerScope(ScopeRange scope)
    {
        return scope == ScopeRange.AllTower ||
            scope == ScopeRange.HumanTower ||
            scope == ScopeRange.ElfTower ||
            scope == ScopeRange.OrcTower ||
            scope == ScopeRange.BeastTower ||
            scope == ScopeRange.DragonTower ||
            scope == ScopeRange.DwarfTower;
    }

    private string DrawItemGradeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out ItemGrade value))
            value = ItemGrade.Normal;

        value = (ItemGrade)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawItemOptionField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out ItemOptions value))
            value = ItemOptions.AtkDamageUP;

        value = (ItemOptions)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawItemTargetField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out ItemTarget value))
            value = ItemTarget.Tower;

        value = (ItemTarget)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string DrawScopeRangeField(string label, string current)
    {
        if (!Enum.TryParse(current, true, out ScopeRange value))
            value = ScopeRange.Global;

        value = (ScopeRange)EditorGUILayout.EnumPopup(label, value);
        return value.ToString();
    }

    private string GetItemIconResourcePath(ItemDataRow row)
    {
        if (row == null)
            return string.Empty;

        return $"Item/Images/{row.Icon_UID}";
    }

    private Sprite LoadItemIcon(ItemDataRow row)
    {
        string path = GetItemIconResourcePath(row);

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

    private void LoadAll()
    {
        itemRows = LoadJson<ItemDataRowList>(ItemDataPath) ?? new ItemDataRowList();
        selectedItemIndex = itemRows.datas.Count > 0 ? Mathf.Clamp(selectedItemIndex, 0, itemRows.datas.Count - 1) : -1;
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
        SaveJson(ItemDataPath, itemRows);
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

    private void AddItem()
    {
        string uid = GetNextUID("IT", itemRows.datas.ConvertAll(x => x.Item_UID), 3);
        string number = uid.Substring(2);

        itemRows.datas.Add(new ItemDataRow
        {
            Item_UID = uid,
            Item_Name = "New Item",
            Grade = ItemGrade.Normal.ToString(),
            Item_Option = ItemOptions.AtkDamageUP.ToString(),
            Target = ItemTarget.Tower.ToString(),
            Scope_Range = ScopeRange.AllTower.ToString(),
            Value = 1,
            Buy_Price = 0,
            Sale_Price = 0,
            String_Key = $"ITEM_NAME_{number}",
            Item_Desc = $"ITEM_DESC_{number}",
            Icon_UID = $"ICON_ITEM_{number}"
        });

        selectedItemIndex = itemRows.datas.Count - 1;
        MarkChanged();
    }

    private void DuplicateItem(ItemDataRow source)
    {
        string uid = GetNextUID("IT", itemRows.datas.ConvertAll(x => x.Item_UID), 3);

        itemRows.datas.Add(new ItemDataRow
        {
            Item_UID = uid,
            Item_Name = source.Item_Name,
            Grade = source.Grade,
            Item_Option = source.Item_Option,
            Target = source.Target,
            Scope_Range = source.Scope_Range,
            Value = source.Value,
            Buy_Price = source.Buy_Price,
            Sale_Price = source.Sale_Price,
            String_Key = source.String_Key,
            Item_Desc = source.Item_Desc,
            Icon_UID = source.Icon_UID
        });

        selectedItemIndex = itemRows.datas.Count - 1;
        MarkChanged();
    }

    private void DeleteItem()
    {
        if (selectedItemIndex < 0 || selectedItemIndex >= itemRows.datas.Count)
            return;

        ItemDataRow row = itemRows.datas[selectedItemIndex];
        bool confirm = EditorUtility.DisplayDialog("Delete Item", $"Delete {row.Item_UID}?", "Delete", "Cancel");

        if (!confirm)
            return;

        itemRows.datas.RemoveAt(selectedItemIndex);
        selectedItemIndex = itemRows.datas.Count > 0 ? Mathf.Clamp(selectedItemIndex, 0, itemRows.datas.Count - 1) : -1;
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
