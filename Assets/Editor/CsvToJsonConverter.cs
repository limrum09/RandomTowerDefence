using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CsvToJsonConverter
{
    [MenuItem("Tools/Data/Convert Tower CXSV To JSON")]
    public static void ConvertTower()
    {
        string csvPath = "Assets/RawData/Tower_Table.csv";
        string jsonPath = "Assets/Resources/Data/TowerData.json";

        string csvText = File.ReadAllText(csvPath); ;
        List<Dictionary<string, string>> rows = CsvUtility.Parse(csvText);

        TowerDataRowList data = new TowerDataRowList();

        foreach(var row in rows)
        {
            TowerDataRow item = new TowerDataRow
            {
                TowerUID = DataParseHelper.GetString(row, "TowerUID"),
                TowerType = DataParseHelper.GetString(row, "TowerType"),
                StringKey = DataParseHelper.GetString(row, "StringKey"),
                Grade = DataParseHelper.GetInt(row, "Grade"),
                BaseAtk = DataParseHelper.GetInt(row, "BaseAtk"),
                BaseAtkSpeed = DataParseHelper.GetFloat(row, "BaseAtkSpeed"),
                Range = DataParseHelper.GetFloat(row, "Range"),
                CostType = DataParseHelper.GetString(row, "CostType"),
                BuyPrice = DataParseHelper.GetInt(row, "BuyPrice"),
                SellPrice = DataParseHelper.GetInt(row, "SellPrice"),
                SkillID = DataParseHelper.GetString(row, "SkillID"),
                IconPath = DataParseHelper.GetString(row, "IconPath"),
                NextGradeUID = DataParseHelper.GetString(row, "NextGradeUID")
            };

            data.datas.Add(item);
        }

        SaveJson(jsonPath, data);
    }

    [MenuItem("Tools/Data/Convert Localization CSV To JSON")]
    public static void ConvertLocalization()
    {
        string csvPath = "Assets/RawData/Localization.csv";
        string jsonPath = "Assets/Resources/Data/Localization.json";

        string csvText = File.ReadAllText(csvPath);
        List<Dictionary<string, string>> rows = CsvUtility.Parse(csvText);

        LocalizationRowList data = new LocalizationRowList();

        if (rows.Count > 0)
        {
            foreach (var pair in rows[0])
            {
                Debug.Log($"HEADER=[{pair.Key}] VALUE=[{pair.Value}]");
            }
        }

        foreach (var row in rows)
        {
            LocalizationDataRow item = new LocalizationDataRow
            {
                key = DataParseHelper.GetString(row, "StringKey"),
                KR = DataParseHelper.GetString(row, "KR"),
                EN = DataParseHelper.GetString(row, "EN")
            };

            data.datas.Add(item);
        }

        SaveJson(jsonPath, data);
    }

    private static void SaveJson<T>(string jsonPath, T data)
    {
        string dir = Path.GetDirectoryName(jsonPath);
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(jsonPath, json);
        AssetDatabase.Refresh();

        Debug.Log("Json 저장 완료");
    }
}
