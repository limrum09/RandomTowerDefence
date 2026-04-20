using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class DataParseHelper
{
    public static string GetString(Dictionary<string, string> row, string key, string defaultValue = "")
    {
        if (row.TryGetValue(key, out string value))
            return value;
        return defaultValue;
    }

    public static int GetInt(Dictionary<string, string> row, string key, int defaultValue = 0)
    {
        if (row.TryGetValue(key, out string value) && int.TryParse(value, out int result))
            return result;

        Debug.LogWarning("Int 파싱 실패");
        return defaultValue; ;
    }

    public static float GetFloat(Dictionary<string, string> row, string key,  float defaultValue = 0f)
    {
        if (row.TryGetValue(key, out string value) && float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
            return result;

        Debug.Log("Float 파싱 실패");
        return defaultValue;
    }

    public static bool GetBool(Dictionary<string, string> row, string key, bool defaultValue = false)
    {
        if(row.TryGetValue(key, out string value))
        {
            if(bool.TryParse(value, out bool result))
                return result;

            if (value == "1") return true;
            if (value == "0") return false;
        }

        Debug.LogWarning("Bool 파싱 실패");
        return defaultValue;
    }

    public static T GetEnum<T>(Dictionary<string,string> row, string key, T defalutValue) where T : struct
    {
        if (row.TryGetValue(key, out string value) && Enum.TryParse(value, true, out T result))
            return result;

        Debug.LogWarning("Enum 파싱 실패");
        return defalutValue;
    }

    public static string valueOrEmpty(Dictionary<string, string> row, string key)
    {
        return row.TryGetValue(key, out string value) ? value : "<missing>";
    }
}
