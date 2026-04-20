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

        string csvText = File.ReadAllText(csvPath);
        List<Dictionary<string, string>> rows = CsvUtility.Parse(csvText);

        TowerDataRowList data = new TowerDataRowList();

        foreach(var row in rows)
        {
            TowerDataRow item = new TowerDataRow
            {
                TowerUID = DataParseHelper.GetString(row, "Tower_UID"),
                TowerType = DataParseHelper.GetString(row, "Type"),
                StringKey = DataParseHelper.GetString(row, "String_Key"),
                Grade = DataParseHelper.GetInt(row, "Grade"),
                BaseAtk = DataParseHelper.GetInt(row, "Base_ATK"),
                BaseAtkSpeed = DataParseHelper.GetFloat(row, "Base_Atk_Speed(s)"),
                Range = DataParseHelper.GetFloat(row, "Range(Tile)"),
                CostType = DataParseHelper.GetString(row, "Cost_Type"),
                BuyPrice = DataParseHelper.GetInt(row, "Buy_Price"),
                SellPrice = DataParseHelper.GetInt(row, "Sell_Price"),
                SkillID = DataParseHelper.GetString(row, "SkillID"),
                IconPath = DataParseHelper.GetString(row, "Icon_UID"),
                NextGradeUID = DataParseHelper.GetString(row, "Next_UID")
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

        foreach (var row in rows)
        {
            LocalizationDataRow item = new LocalizationDataRow
            {
                key = DataParseHelper.GetString(row, "String_Key"),
                KR = DataParseHelper.GetString(row, "KR"),
                EN = DataParseHelper.GetString(row, "EN")
            };

            data.datas.Add(item);
        }

        SaveJson(jsonPath, data);
    }

    [MenuItem("Tools/Data/Convert Tower Skill CXSV To JSON")]
    public static void ConvertTowerSkill()
    {
        string csvPath = "Assets/RawData/TowerSkill_Table .csv";
        string jsonPath = "Assets/Resources/Data/TowerSkillData.json";

        string csvText = File.ReadAllText(csvPath);
        List<Dictionary<string, string>> rows = CsvUtility.Parse(csvText);

        TowerDataRowList data = new TowerDataRowList();

        foreach (var row in rows)
        {
            TowerDataRow item = new TowerDataRow
            {
                TowerUID = DataParseHelper.GetString(row, "Tower_UID"),
                TowerType = DataParseHelper.GetString(row, "Type"),
                StringKey = DataParseHelper.GetString(row, "String_Key"),
                Grade = DataParseHelper.GetInt(row, "Grade"),
                BaseAtk = DataParseHelper.GetInt(row, "Base_ATK"),
                BaseAtkSpeed = DataParseHelper.GetFloat(row, "Base_Atk_Speed(s)"),
                Range = DataParseHelper.GetFloat(row, "Range(Tile)"),
                CostType = DataParseHelper.GetString(row, "Cost_Type"),
                BuyPrice = DataParseHelper.GetInt(row, "Buy_Price"),
                SellPrice = DataParseHelper.GetInt(row, "Sell_Price"),
                SkillID = DataParseHelper.GetString(row, "SkillID"),
                IconPath = DataParseHelper.GetString(row, "Icon_UID"),
                NextGradeUID = DataParseHelper.GetString(row, "Next_UID")
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
