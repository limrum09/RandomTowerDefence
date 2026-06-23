using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public enum SelectLanguege
{
    KR,
    EN
}

[Serializable]
public class LocalizationData
{
    public string key;
    public string KR;
    public string EN;

    public LocalizationData(string st, string kR, string eN)
    {
        key = st;
        KR = kR;
        EN = eN;
    }
}

public class LocalizationDataManager
{
    Dictionary<string, LocalizationData> datas = new Dictionary<string, LocalizationData>();
    private SelectLanguege language = SelectLanguege.KR;

    public event Action OnLanguageChanged;

    private void LoadDataToJson()
    { 
        LocalizationRowList rowList = JsonLoader.LoadFromResources<LocalizationRowList>("Data/Localization");

        if (rowList == null || rowList.datas == null)
            return;

        foreach (LocalizationDataRow row in rowList.datas)
        {
            LocalizationData data = new LocalizationData(row.String_Key, row.KR, row.EN);

            datas[data.key] = data;
        }
    }

    public void Init()
    {
        datas.Clear();
        SetLanguage(SelectLanguege.KR);
        LoadDataToJson();
    }

    public void SetLanguage(SelectLanguege la)
    {
        language = la;

        OnLanguageChanged?.Invoke();
    }

    public string GetString(string key)
    {
        if(datas.TryGetValue(key, out LocalizationData data)){
            switch(language)
            {
                case SelectLanguege.KR:
                    return data.KR; 
                case SelectLanguege.EN:
                    return data.EN;
            }
        }

        return $"{key}를 찾을 수 없음";
    }

    public void SetKey(LocalizeStringEvent strLocal, string tableName, string key)
    {
        strLocal.StringReference.TableReference = tableName;
        strLocal.StringReference.TableEntryReference = key;
        strLocal.RefreshString();
    }

    public string GetString(string tableName, string key)
    {
        string str = LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);

        return str;
    }
}
