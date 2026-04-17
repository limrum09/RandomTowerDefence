using System;
using System.Collections.Generic;

[Serializable]
public class LocalizationDataRow
{
    public string key;
    public string KR;
    public string EN;
}

[Serializable]
public class LocalizationRowList
{
    public List<LocalizationDataRow> datas = new List<LocalizationDataRow>();
}
