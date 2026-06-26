using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine.Localization;
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

    private Locale FindLocaleByCode(string code)
    {
        var locales = LocalizationSettings.AvailableLocales.Locales;

        for(int i = 0; i < locales.Count; i++)
        {
            if (locales[i].Identifier.Code == code)
                return locales[i];
        }

        return null;
    }

    private SelectLanguege GetSelectLanguege(Locale locale)
    {
        string code = locale.Identifier.Code.ToLower();

        if (code.StartsWith("en"))
            return SelectLanguege.EN;

        return SelectLanguege.KR;
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

    public void ApplyLocale(Locale locale)
    {
        if (locale == null)
            return;

        LocalizationSettings.SelectedLocale = locale;
        SetLanguage(GetSelectLanguege(locale));
    }

    public async Task ApplySavedLocale(string localeCode)
    {
        while (!LocalizationSettings.InitializationOperation.IsDone)
            await Task.Yield();

        if (string.IsNullOrEmpty(localeCode))
            return;

        Locale target = FindLocaleByCode(localeCode);

        if (target == null)
            return;

        ApplyLocale(target);
    }
}
