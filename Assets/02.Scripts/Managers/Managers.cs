using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers instance;
    public static Managers Instance { get { Init(); return instance; } }

    private GameManager game = new GameManager();
    private GridManager grid = new GridManager();
    public static GameManager Game { get { return instance.game; } }
    public static GridManager Grid { get { return instance.grid; } }

    private void Awake()
    {
        Init();
    }
    static void Init()
    {
        if(instance == null)
        {
            GameObject manager = GameObject.Find("Managers");

            if(manager == null)
            {
                manager = new GameObject { name = "Managers" };
                manager.AddComponent<Managers>();
            }

            DontDestroyOnLoad(manager);
            instance = manager.GetComponent<Managers>();
        }
    }
}
