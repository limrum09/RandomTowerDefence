using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Managers : MonoBehaviour
{
    private static Managers instance;
    public static bool isQuitting;
    public static Managers Instance
    {
        get
        {
            if (isQuitting)
                return null;

            Init(); 
            return instance;
        }
    }
    public static bool HasInstance => instance != null && isQuitting;

    private GameManager game = new GameManager();
    private InputManager input = new InputManager();
    private TowerDataManager tower = new TowerDataManager();
    public static GameManager Game { get { return Instance.game; } }
    public static InputManager InputKey { get { return Instance.input; } }
    public static TowerDataManager TowerData { get { return Instance.tower; } }

    private void Start()
    {
        // 임시, 나중에 지워야 함
        InputKey.ResetKeyCode();
    }

    private void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        TowerData.Init();
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
    static void Init()
    {
        if (isQuitting)
            return;

        if (instance != null)
            return;

        if(instance == null)
        {
            GameObject manager = GameObject.Find("Managers");

            if(manager == null)
            {
                manager = new GameObject { name = "Managers" };
                manager.AddComponent<Managers>();
            }

            instance = manager.GetComponent<Managers>();
        }
    }
}
