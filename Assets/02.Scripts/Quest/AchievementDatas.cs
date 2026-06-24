using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Quest/AchievementDatabase")]
public class AchievementDatas : ScriptableObject
{
    [SerializeField]
    private List<Achievement> quests;

    public List<Achievement> GetAllAchievement()
    {
        return quests;
    }

    public Achievement FindByCode(string code)
    {
        Debug.Log("Find UID : " + code);
        return quests.FirstOrDefault(x => x.QuestUID == code);
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Achievements")]
    private void Refresh()
    {
        RefreshDatabase();
    }

    public void RefreshDatabase()
    {
        quests = new List<Achievement>();
        string[] guids = AssetDatabase.FindAssets("Achievement_ t:Quest");

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<Achievement>(assetPath);

            quests.Add(data);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
