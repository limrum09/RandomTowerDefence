using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ResourceCache
{

    /*private static readonly Dictionary<System.Type, Dictionary<string, Object>>*/

    private static readonly Dictionary<string, Object> cache = new Dictionary<string, Object>();
    private static readonly Dictionary<string, Object[]> cacheFolder = new Dictionary<string, Object[]>();

    private static string GetKey<T> (string path) where T : Object
    {
        return $"{typeof(T).Name}.{path}";
    }

    public static T Load<T> (string path) where T : Object
    {
        string key = GetKey<T>(path);

        if (cache.TryGetValue(key, out Object obj))
            return obj as T;

        T loadObject = Resources.Load<T>(path);

        if (loadObject != null)
        {
            cache.Add(key, loadObject);
        }
        else
        {
            Debug.Log("불러오기 실패 " + key);
        }

        return loadObject;
    }

    public static T[] LoadAll<T>(string path) where T : Object
    {
        if (cacheFolder.TryGetValue(path, out Object[] objs))
            return objs as T[];

        T[] loadObejcts = Resources.LoadAll<T>(path);

        if(loadObejcts != null && loadObejcts.Length > 0)
        {
            cacheFolder.Add(path, loadObejcts);
        }

        return loadObejcts;
    }

    public static void ClearCache()
    {
        cache.Clear();
        cacheFolder.Clear();

        Resources.UnloadUnusedAssets();
    }
}
