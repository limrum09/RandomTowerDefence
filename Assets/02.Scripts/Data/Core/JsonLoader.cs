using UnityEngine;

public static class JsonLoader
{
    public static T LoadFromResources<T>(string resourcePath) where T : class
    {
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
        if(textAsset == null)
        {
            Debug.LogWarning("Json 파일을 찾을 ㅜㅅ 없음 : " +  resourcePath);
            return null;
        }

        T data = JsonUtility.FromJson<T>(textAsset.text);
        if(data == null)
        {
            Debug.Log("Json 역질렬화 실패 : " + resourcePath);
            return null;
        }

        return data;
    }
}
