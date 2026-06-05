using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static bool isQuit = false;

    public static T Instance
    {
        get
        {
            if (isQuit)
                return null;

            if(instance == null)
            {
                instance = FindAnyObjectByType<T>();

                if(instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    instance = go.AddComponent<T>();
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    protected virtual void OnApplicationQuit()
    {
        isQuit = true;
    }
}
