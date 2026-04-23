using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Unity.VisualScripting;
public static class CsvUtility
{
    public static List<Dictionary<string, string>> Parse(string csvText)
    {
        List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

        if (string.IsNullOrEmpty(csvText))
            return result;

        List<List<string>> rows = ParseRows(csvText);
        if (rows.Count < 2)
            return result;

        List<string> headers = rows[0];
        int headersLen = headers.Count;

        for (int i = 0; i < headersLen; i++)
        {
            headers[i] = headers[i].Trim().Trim('\uFEFF');
        }

        int rowLen = rows.Count;
        for(int i = 1; i < rowLen; i++)
        {
            List<string> values = rows[i];
            Dictionary<string, string> row = new Dictionary<string, string>();

            for(int j = 0; j < headersLen; j++)
            {
                string header = headers[j];
                if (string.IsNullOrEmpty(header))
                    continue;

                string value = j < values.Count ? values[j] : string.Empty; ;
                row[header] = value;
            }

            result.Add(row);
        }

        return result;
    }

    private static List<List<string>> ParseRows(string csvText)
    {
        List<List<string>> rows = new List<List<string>>();
        List<string> currentRow = new List<string>();
        StringBuilder currentField = new StringBuilder();

        bool inQu = false;
        int csvLen = csvText.Length;

        for(int i = 0; i < csvLen; i++)
        {
            char c = csvText[i];

            // 큰 따움표에서 ""는 실체 "문자
            if (c == '"')
            {
                if(inQu && i + 1 < csvLen && csvText[i + 1] == '"')
                {
                    currentField.Append('"');
                    i++;
                }
                else
                {
                    inQu = !inQu;
                }
            }
            // 필드 종료
            else if(c == ',' && !inQu)
            {
                currentRow.Add(currentField.ToString());
                currentField.Clear();
            }
            // 행 종료
            else if((c == '\n' || c == '\r') && !inQu)
            {
                if (c == '\r' && i + 1 < csvLen && csvText[i + 1] == '\n')
                    i++;

                currentRow.Add(currentField.ToString());
                currentField.Clear();

                if (!IsRowEmpty(currentRow))
                    rows.Add(currentRow);

                currentRow = new List<string>();
            }
            else
            {
                currentField.Append(c);
            }
        }

        currentRow.Add(currentField.ToString());

        if (!IsRowEmpty(currentRow))
            rows.Add(currentRow);

        return rows;
    }

    private static bool IsRowEmpty(List<string> row)
    {
        int rowLen = row.Count;
        for(int i = 0; i < rowLen; i++)
        {
            if (!string.IsNullOrEmpty(row[i]))
                return false;
        }

        return true;
    }

    private static string[] SplitCsvLine(string line)
    {
        List<string> result = new List<string>();
        StringBuilder sb = new StringBuilder();
        bool inQu = false;

        int length = line.Length;
        for(int i = 0; i < length; i++)
        {
            char c = line[i];

            if(c == '"')
            {
                if(inQu && i + 1 < length && line[i + 1] == '"')
                {
                    sb.Append('"');
                    i++;
                }
                else
                {
                    inQu = !inQu;
                }
            }
            else if(c == ',' && !inQu)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        result.Add(sb.ToString());
        return result.ToArray();
    }
}
