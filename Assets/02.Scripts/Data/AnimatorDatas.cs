using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "AnimatorDatas", menuName = "Scriptable Objects/AnimatorDatas")]
public class AnimatorDatas : ScriptableObject
{
    [SerializeField]
    private List<RuntimeAnimatorController> anis;

    public RuntimeAnimatorController FindByCode(string code)
    {
        Debug.Log("Find Name : " + code);
        return anis.FirstOrDefault(x => x.name == code);
    }

#if UNITY_EDITOR
    [ContextMenu("RefreshAnimators")]
    private void Refresh()
    {
        RefreshDatabase();
    }

    public void RefreshDatabase()
    {
        anis = new List<RuntimeAnimatorController>();
        string[] guids = AssetDatabase.FindAssets("t:AnimatorController");

        foreach(var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var data = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(assetPath);

            if (data != null && data.name.EndsWith("Ani"))
            {
                anis.Add(data);
            }
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
#endif
}
