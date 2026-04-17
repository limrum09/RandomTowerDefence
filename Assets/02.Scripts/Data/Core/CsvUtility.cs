using System;
using System.Collections.Generic;
using System.Text;

public static class CsvUtility
{
    public static List<Dictionary<string, string>> Parse(string csvText)
    {
        List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

        if (string.IsNullOrEmpty(csvText))
            return result;

        string[] lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
            return result;

        string[] headers = SplitCsvLine(lines[0]);

        for (int i = 0; i < headers.Length; i++)
        {
            headers[i] = headers[i].Trim().Trim('\uFEFF');
        }


        int length = lines.Length;
        for(int i = 1; i < length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] values = SplitCsvLine(line);
            Dictionary<string, string> row = new Dictionary<string, string>();

            int headerLength = headers.Length;
            for(int j = 0; j < headerLength; j++)
            {
                string header = headers[j].Trim();
                string value = j < values.Length ? values[j].Trim() : string.Empty;
                row[header] = value;
            }

            result.Add(row);
        }

        return result;
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
