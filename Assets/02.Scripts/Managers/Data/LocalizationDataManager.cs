using System;
using System.Collections.Generic;
using Unity.VisualScripting;

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
}
