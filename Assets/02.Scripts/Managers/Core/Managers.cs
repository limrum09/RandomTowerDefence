using System;
using System.Net.NetworkInformation;
using UnityEngine;

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
    private EnemyDataManager enemy = new EnemyDataManager();
    private EnemySkillDataManager enemySkill = new EnemySkillDataManager();
    private LocalizationDataManager local = new LocalizationDataManager();
    private TowerSkillDataManager towerSkill = new TowerSkillDataManager();
    private ItemDataManager item = new ItemDataManager();
    private TowerSessionUpgradeManager sessionUpgrade = new TowerSessionUpgradeManager();
    private WaveDataManager wave = new WaveDataManager();
    private WaveEnemyRosterDataManager waveRoster = new WaveEnemyRosterDataManager();
    private PoolManager pool = new PoolManager();
    private GameEffectManager effectManager = new GameEffectManager();
    private StageStartOptionBaseDataManager startOptionDataManager = new StageStartOptionBaseDataManager();
    private MetaResearchDataManager metaResearch = new MetaResearchDataManager();

    [Header("SaveDatas")]
    private TowerMetaUpgradeManager towerMetaUpgrade = new TowerMetaUpgradeManager();
    private PublicMetaUpgradeManager pulbicMeraUpgrade = new PublicMetaUpgradeManager();

    public static GameManager Game { get { return Instance.game; } }
    public static InputManager InputData { get { return Instance.input; } }
    public static TowerDataManager TowerData { get { return Instance.tower; } }
    public static EnemyDataManager EnemyData { get { return Instance.enemy; } }
    public static EnemySkillDataManager EnemySkillData { get { return Instance.enemySkill; } }
    public static LocalizationDataManager Local { get { return Instance.local; } }
    public static TowerSkillDataManager TowerSkill { get { return Instance.towerSkill; } } 
    public static ItemDataManager Item {  get { return Instance.item; } }
    public static TowerSessionUpgradeManager SessionTowerUpgrade {  get { return Instance.sessionUpgrade; } }
    public static WaveDataManager Wave {  get { return Instance.wave; } }
    public static WaveEnemyRosterDataManager WaveRoster { get { return Instance.waveRoster; } }
    public static PoolManager Pool { get { return Instance.pool; } }
    public static GameEffectManager Effect { get { return Instance.effectManager; } }
    public static MetaResearchDataManager ResearchData { get { return Instance.metaResearch; } }
    public static StageStartOptionBaseDataManager StartOption { get { return Instance.startOptionDataManager; } }

    public static TowerMetaUpgradeManager TowerMetaUpgrade { get { return Instance.towerMetaUpgrade; } }
    public static PublicMetaUpgradeManager PublicMetaUpgrade { get { return Instance.pulbicMeraUpgrade; } }


    public event Action OnEndLoadDatas;

    private void Start()
    {
        // 임시, 나중에 지워야 함
        InputData.Init();
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

        Game.Init();
        TowerData.Init();
        EnemyData.Init();
        EnemySkillData.Init();
        Local.Init();
        TowerSkill.Init();
        item.Init();
        SessionTowerUpgrade.Inti();
        Wave.Init();
        WaveRoster.Init();
        Pool.Init();
        Effect.Init();
        StartOption.Init();
        ResearchData.Init();


        // SaveData에서 데이터들을 다 넣어 줘야할 듯
        OnEndLoadDatas?.Invoke();
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
