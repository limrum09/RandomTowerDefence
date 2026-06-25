using System;
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
    public static bool HasInstance => instance != null && !isQuitting;

    private GameManager game = new GameManager();
    private StageScoreCalculator stageScoreCalculator = new StageScoreCalculator();
    private SaveDataManager saveDataManager = new SaveDataManager();
    private SoundManager sound = new SoundManager();
    private GraphicManager graphic = new GraphicManager();
    private PlayerProgressManager playerProgressData = new PlayerProgressManager();
    private InputManager input = new InputManager();
    private TowerDataManager tower = new TowerDataManager();
    private EnemyDataManager enemy = new EnemyDataManager();
    private EnemySkillDataManager enemySkill = new EnemySkillDataManager();
    private LocalizationDataManager local = new LocalizationDataManager();
    private TowerSkillDataManager towerSkill = new TowerSkillDataManager();
    private ItemDataManager item = new ItemDataManager();
    private TowerSessionUpgradeManager sessionUpgrade = new TowerSessionUpgradeManager();
    private StageLevelRule stageLevel = new StageLevelRule();
    private StageRuleDataManager stageRuleData = new StageRuleDataManager();
    private WaveDataManager wave = new WaveDataManager();
    private WaveEnemyRosterDataManager waveRoster = new WaveEnemyRosterDataManager();
    private PoolManager pool = new PoolManager();
    private QuestManager quest = new QuestManager();
    private GameEffectManager effectManager = new GameEffectManager();
    private StageStartOptionBaseDataManager startOptionDataManager = new StageStartOptionBaseDataManager();
    private MetaResearchUpgradeDataManager metaResearchUpgrade = new MetaResearchUpgradeDataManager();
    private MetaResearchLevelDataManager metaResearchLevel = new MetaResearchLevelDataManager();

    [Header("SaveDatas")]
    private TowerMetaUpgradeManager towerMetaUpgrade = new TowerMetaUpgradeManager();
    private PublicMetaUpgradeManager pulbicMeraUpgrade = new PublicMetaUpgradeManager();

    public static GameManager Game { get { return Instance.game; } }
    public static StageScoreCalculator ScoreCal { get { return Instance.stageScoreCalculator; } }
    public static SaveDataManager Save { get { return Instance.saveDataManager; } }
    public static SoundManager Sound {  get { return Instance.sound; } }
    public static GraphicManager Graphic {  get { return Instance.graphic; } }
    public static PlayerProgressManager Player {  get { return Instance.playerProgressData; } }
    public static InputManager InputData { get { return Instance.input; } }
    public static TowerDataManager TowerData { get { return Instance.tower; } }
    public static EnemyDataManager EnemyData { get { return Instance.enemy; } }
    public static EnemySkillDataManager EnemySkillData { get { return Instance.enemySkill; } }
    public static LocalizationDataManager Local { get { return Instance.local; } }
    public static TowerSkillDataManager TowerSkill { get { return Instance.towerSkill; } } 
    public static ItemDataManager Item {  get { return Instance.item; } }
    public static TowerSessionUpgradeManager SessionTowerUpgrade {  get { return Instance.sessionUpgrade; } }
    public static StageLevelRule StageLevelRules { get { return Instance.stageLevel; } }
    public static StageRuleDataManager StageRules { get { return Instance.stageRuleData; } }
    public static WaveDataManager Wave {  get { return Instance.wave; } }
    public static WaveEnemyRosterDataManager WaveRoster { get { return Instance.waveRoster; } }
    public static PoolManager Pool { get { return Instance.pool; } }
    public static QuestManager QuestMgr { get { return Instance.quest; } }
    public static GameEffectManager Effect { get { return Instance.effectManager; } }
    public static MetaResearchUpgradeDataManager ResearchUpgrade { get { return Instance.metaResearchUpgrade; } }
    public static StageStartOptionBaseDataManager StartOption { get { return Instance.startOptionDataManager; } }
    public static MetaResearchLevelDataManager ResearchLevel { get { return Instance.metaResearchLevel; } }

    public static TowerMetaUpgradeManager TowerMetaUpgrade { get { return Instance.towerMetaUpgrade; } }
    public static PublicMetaUpgradeManager PublicMetaUpgrade { get { return Instance.pulbicMeraUpgrade; } }


    public event Action OnEndLoadDatas;

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
        StageLevelRules.Init();
        StageRules.Init();
        Wave.Init();
        WaveRoster.Init();
        Pool.Init();
        Effect.Init();
        StartOption.Init();
        ResearchUpgrade.Init();
        ResearchLevel.Init();
        QuestMgr.Init();

        Sound.Init();
        Graphic.Init();
        // 임시, 나중에 지워야 함
        InputData.Init();;
    }

    private async void OnApplicationQuit()
    {
        if(saveDataManager != null)
            await saveDataManager.SaveAllData();

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

    private async void Start()
    {
        if (!Save.HasSignInUser())
        {
            Player.LoadSaveData(null);
            pulbicMeraUpgrade.LoadSaveData(null);
            towerMetaUpgrade.LoadSaveData(null);
            QuestMgr.LoadSaveData(null);

            LoadSceneManager.Instance.NotifyDataLoaded();
            OnEndLoadDatas?.Invoke();
            return;
        }

        string uid = FirebaseInitializer.Instance.Auth.CurrentUser.UserId;

        bool exists = await Save.HasFirebaseSaveData(uid);

        if (!exists)
            await Save.CreateNewUserFirebaseSaveData(uid);

        bool loadSuccess = await Save.LoadAllData();

        if (!loadSuccess)
        {
            // 실패 시, 여기서 팝업듸우든 뭐든해서 종료해야하나
            return;
        }

        LoadSceneManager.Instance.NotifyDataLoaded();
        OnEndLoadDatas?.Invoke();
    }
}
