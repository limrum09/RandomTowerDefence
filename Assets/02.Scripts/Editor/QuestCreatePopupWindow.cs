using System;
using UnityEditor;
using UnityEngine;

public class QuestCreatePopupWindow : EditorWindow
{
    private string inputName = string.Empty;
    private string createType;
    private string creatPath;
    private Action<string> Oncreate;

    private void OnGUI()
    {
        EditorGUILayout.LabelField($"Create {createType}", EditorStyles.boldLabel);

        EditorGUILayout.Space();

        inputName = EditorGUILayout.TextField("Name", inputName);

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create"))
        {
            if (string.IsNullOrWhiteSpace(inputName))
            {
                EditorUtility.DisplayDialog("Invalid Name", "이름을 입력하세요.", "OK");
                return;
            }

            Oncreate?.Invoke(inputName.Trim());
            Close();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }

        EditorGUILayout.EndHorizontal();
    }

    public static void Open(string getCreateType, string getCreatePath, Action<string> onCreate)
    {
        QuestCreatePopupWindow window = CreateInstance<QuestCreatePopupWindow>();

        window.titleContent = new GUIContent($"Create {getCreateType}");
        window.createType = getCreateType;
        window.creatPath = getCreatePath;
        window.Oncreate = onCreate;

        window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 350, 100);

        window.ShowUtility();
    }
}
