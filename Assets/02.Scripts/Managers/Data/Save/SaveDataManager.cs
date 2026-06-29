using Firebase.Firestore;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public enum FirebaseSaveCheckState
{
    Exists,
    Missing,
    Failed
}

public interface IValidSaveData
{
    bool IsValid();
}

public class SaveDataManager
{
    private string CurrentUserUid
    {
        get
        {
            return FirebaseInitializer.Instance.Auth.CurrentUser.UserId;
        }
    }

    private DocumentReference GetSaveDoc(string docName) => GetSaveDoc(CurrentUserUid, docName);
    private DocumentReference GetSaveDoc(string uid, string docName)
    {
        return FirebaseInitializer.Instance.Firestore.Collection("users").
            Document(uid).Collection("save").Document(docName);
    }

    private FirestoreSaveRepository reposity;
    private const string INPUTKEY_SAVE_FILE = "inputkey_save.json";
    private const string SOUND_SAVE_FILE = "sound_save.json";
    private const string GRAPHIC_SAVE_FILE = "graphic_save.json";
    private const string META_UPGRADE_SAVE_FILE = "meta_upgrade";
    private const string PLAYER_PROGRESS_FILE = "meta_player_progress";
    private const string QUEST_SAVE_FILE = "quest";
    public bool isMetaUpgradeDirty;
    public bool isGraphicDirty;
    public bool isSoundDirty;
    public bool isInputDirty;
    public bool isPlayerDirty;
    public bool isQuestDirty;

    public SaveDataManager()
    {
        reposity = new FirestoreSaveRepository();
    }

    public string SavePath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);
    public bool IsFirebaseLoadCompleted { get; private set; }
    public bool IsFirebaseLoadFail { get; private set; }

    public async Task<FirebaseSaveCheckState> CheckFirebaseSaveData(string uid, int timeOutMs = 10000)
    {
        try
        {
            Task<DocumentSnapshot> loadTask = GetSaveDoc(uid, PLAYER_PROGRESS_FILE).GetSnapshotAsync();
            Task timeOutTask = Task.Delay(timeOutMs);

            Task completedTask = await Task.WhenAny(loadTask, timeOutTask);

            if(completedTask == timeOutTask)
            {
                Debug.LogWarning("Firebae save check Time Out");
                return FirebaseSaveCheckState.Failed;
            }

            DocumentSnapshot snapshot = await loadTask;

            return snapshot.Exists ? FirebaseSaveCheckState.Exists : FirebaseSaveCheckState.Missing;
        }
        catch(Firebase.FirebaseException e)
        {
            Debug.LogWarning($"Firebase save Check Fail : {e.ErrorCode} / {e.Message}");
            return FirebaseSaveCheckState.Failed;
        }
        catch(Exception e)
        {
            Debug.LogWarning($"Firebase save Check Fail : {e.Message}");
            return FirebaseSaveCheckState.Failed;
        }
    }
    
    public bool HasSignInUser()
    {
        return FirebaseInitializer.Instance != null &&
            FirebaseInitializer.Instance.Auth != null &&
            FirebaseInitializer.Instance.Auth.CurrentUser != null;
    }

    private PlayerProgressData CreateDefaultPlayerProgressData()
    {
        return new PlayerProgressData
        {
            level = 1,
            exp = 0,
            metaCurrency = 0,
        };
    }

    private MetaUpgradeSaveData CreateDefaultMetaUpgradeSaveDatas()
    {
        return new MetaUpgradeSaveData();
    }

    private QuestSaveDataList CreateDefaultQuestSaveData()
    {
        return new QuestSaveDataList();
    }

    private bool CanSaveFirebaseData()
    {
        if (IsFirebaseLoadFail)
            return false;
        if (!IsFirebaseLoadCompleted)
            return false;
        if (!HasSignInUser())
            return false;

        return true;
    }

    private async Task<bool> LoadMetaUpgradeData()
    {
        var result = await reposity.LoadAsync<MetaUpgradeSaveData>(GetSaveDoc(META_UPGRADE_SAVE_FILE));

        switch (result.Stat)
        {
            case FirestoreLoadStat.Success:
                Managers.PublicMetaUpgrade.LoadSaveData(result.Data.publicMetaSaveData);
                Managers.TowerMetaUpgrade.LoadSaveData(result.Data.towerMetaSaveData);
                return true;
            case FirestoreLoadStat.DocumentMissing:
                MetaUpgradeSaveData defaultData = CreateDefaultMetaUpgradeSaveDatas();
                Managers.PublicMetaUpgrade.LoadSaveData(defaultData.publicMetaSaveData);
                Managers.TowerMetaUpgrade.LoadSaveData(defaultData.towerMetaSaveData);
                await GetSaveDoc(META_UPGRADE_SAVE_FILE).SetAsync(defaultData, SetOptions.Overwrite);
                return true;
            default:
                Debug.LogWarning($"Meta Upgrade Load Fail : {result.Stat} / {result.ErrorMessage}");
                return false;
        }
    }

    private async Task<bool> LoadPlayerProgressData()
    {
        var result = await reposity.LoadAsync<PlayerProgressData>(GetSaveDoc(PLAYER_PROGRESS_FILE));

        switch (result.Stat)
        {
            case FirestoreLoadStat.Success:
                Managers.Player.LoadSaveData(result.Data);
                return true;
            case FirestoreLoadStat.DocumentMissing:
                PlayerProgressData defaultData = CreateDefaultPlayerProgressData();
                Managers.Player.LoadSaveData(defaultData);
                await GetSaveDoc(PLAYER_PROGRESS_FILE).SetAsync(defaultData, SetOptions.Overwrite);
                return true;
            default:
                Debug.LogWarning($"Progress Load Fail : {result.Stat} / {result.ErrorMessage}");
                return false;
        }
    }

    private async Task<bool> LoadAchievementData()
    {
        var result = await reposity.LoadAsync<QuestSaveDataList>(GetSaveDoc(QUEST_SAVE_FILE));

        switch (result.Stat)
        {
            case FirestoreLoadStat.Success:
                Managers.QuestMgr.LoadSaveData(result.Data);
                return true;
            case FirestoreLoadStat.DocumentMissing:
                QuestSaveDataList defaultData = CreateDefaultQuestSaveData();
                Managers.QuestMgr.LoadSaveData(defaultData);
                await GetSaveDoc(QUEST_SAVE_FILE).SetAsync(defaultData, SetOptions.Overwrite);
                return true;
            default:
                Debug.LogWarning($"Quest Load Fail : {result.Stat} / {result.ErrorMessage}");
                return false;
        }
    }

    private void LoadInputKeyData()
    {
        InputKeySaveData saveData = LoadLocalSaveData<InputKeySaveData>(SavePath(INPUTKEY_SAVE_FILE));

        if (saveData == null)
            return;

        Managers.InputData.LoadInputKeyData(saveData);
    }

    private void LoadSoundData()
    {
        SoundSaveData saveData = LoadLocalSaveData<SoundSaveData>(SavePath(SOUND_SAVE_FILE));

        if (saveData == null)
            return;

        Managers.Sound.LoadSoundSaveData(saveData);
    }

    private void LoadGraphicData()
    {
        GraphicSaveData saveData = LoadLocalSaveData<GraphicSaveData>(SavePath(GRAPHIC_SAVE_FILE));

        if (saveData == null)
            return;

        Managers.Graphic.LoadOptionSaveData(saveData);
    }

    private T LoadLocalSaveData<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(path);
            T saveData = JsonUtility.FromJson<T>(json);

            return saveData;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"False Load Player Progress Data {e.Message}");
            return null;
        }
    }

    private void SaveLocalDataToJson<T>(string path, T saveData, ref bool dirtyFlag)
    {
        string tempPath = path + ".temp";
        string backupPath = path + ".bak";

        try
        {
            string json = JsonUtility.ToJson(saveData);

            File.WriteAllText(tempPath, json);

            if (File.Exists(path))
                File.Copy(path, backupPath, true);

            File.Copy(tempPath, path, true);
            File.Delete(tempPath);

            dirtyFlag = false;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"False Save {saveData} {e.Message}");
        }
    }

    public void LoadLocalOptionData()
    {
        LoadInputKeyData();
        LoadSoundData();
        LoadGraphicData();
    }

    public async Task<bool> LoadAllData()
    {
        IsFirebaseLoadCompleted = false;
        IsFirebaseLoadFail = false;

        if (!HasSignInUser())
        {
            IsFirebaseLoadFail = true;
            return false;
        }
        try
        {
            bool isMetaUpgrade = await LoadMetaUpgradeData();
            bool isPlayerProgress =  await LoadPlayerProgressData();
            bool isAchievement = await LoadAchievementData();

            IsFirebaseLoadCompleted = isMetaUpgrade && isPlayerProgress && isAchievement;
        }
        catch (Exception e)
        {
            Debug.Log("Load Errer : " + e.Message);
            IsFirebaseLoadFail = true;
            return false;
        }

        return IsFirebaseLoadCompleted;
    }

    public async Task SaveAllData()
    {
        SaveInputKeyData();
        SaveSoundData();
        SaveGraphicData();

        await SaveMetaUpgradeData();
        await SavePlayerProgressData();
        await SaveAchievementData();
    }

    public async Task SaveMetaUpgradeData()
    {
        if (!CanSaveFirebaseData())
            return;

        if (!isMetaUpgradeDirty)
            return;

        MetaUpgradeSaveData saveData = new MetaUpgradeSaveData()
        {
            publicMetaSaveData = Managers.PublicMetaUpgrade.GetPublicMetaSaveData(),
            towerMetaSaveData = Managers.TowerMetaUpgrade.GetTowerUpgradeSaveData()
        };

        try
        {
            await GetSaveDoc(META_UPGRADE_SAVE_FILE).SetAsync(saveData, SetOptions.MergeAll);

            isMetaUpgradeDirty = false;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Meta upgrade Firestore save failed : {e.Message}");
        }
    }

    public async Task SavePlayerProgressData()
    {
        if (!CanSaveFirebaseData())
            return;

        if (!isPlayerDirty)
            return;

        PlayerProgressData saveData = Managers.Player.GetSaveData();

        try
        {
            await GetSaveDoc(PLAYER_PROGRESS_FILE).SetAsync(saveData, SetOptions.MergeAll);

            isPlayerDirty = false;
        }
        catch(Exception e)
        {
            Debug.LogWarning($"Player pregress Firestore save failed : {e.Message}");
        }

        
    }

    public async Task SaveAchievementData()
    {
        if (!CanSaveFirebaseData())
            return;

        if (!isQuestDirty)
            return;

        QuestSaveDataList saveData = Managers.QuestMgr.GetSaveData();

        try
        {
            await GetSaveDoc(QUEST_SAVE_FILE).SetAsync(saveData, SetOptions.MergeAll);

            isQuestDirty = false;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Achievement Firestore save failed : {e.Message}");
        }
    }

    public void SaveSoundData()
    {
        if (!isSoundDirty)
            return;

        SoundSaveData saveData = Managers.Sound.GetSaveData();

        SaveLocalDataToJson<SoundSaveData>(SavePath(SOUND_SAVE_FILE), saveData, ref isSoundDirty);
    }

    public void SaveInputKeyData()
    {
        if (!isInputDirty)
            return;

        InputKeySaveData saveData = Managers.InputData.GetSaveData();

        SaveLocalDataToJson<InputKeySaveData>(SavePath(INPUTKEY_SAVE_FILE), saveData, ref isInputDirty);
    }

    public void SaveGraphicData()
    {
        if (!isGraphicDirty)
            return;

        GraphicSaveData saveData = Managers.Graphic.GetSaveData();

        SaveLocalDataToJson<GraphicSaveData>(SavePath(GRAPHIC_SAVE_FILE), saveData, ref isGraphicDirty);
    }

    public async Task SaveAllDirty()
    {
        MarkMetaUpgradeDirty();
        MarkSoundDirty();
        MarkInputDirty();
        MarkPlayerDirty();
        MarkGraphicDirty();
        MarkQuestDirty();

        await SaveAllData();
    }

    public async Task CreateNewUserFirebaseSaveData(string uid)
    {
        await GetSaveDoc(uid, PLAYER_PROGRESS_FILE).SetAsync(CreateDefaultPlayerProgressData(), SetOptions.Overwrite);
        await GetSaveDoc(uid, META_UPGRADE_SAVE_FILE).SetAsync(CreateDefaultMetaUpgradeSaveDatas(), SetOptions.Overwrite);
        await GetSaveDoc(uid, QUEST_SAVE_FILE).SetAsync(CreateDefaultQuestSaveData(), SetOptions.Overwrite);
    }

    public void MarkMetaUpgradeDirty() => isMetaUpgradeDirty = true;
    public void MarkGraphicDirty() => isGraphicDirty = true;
    public void MarkSoundDirty() => isSoundDirty = true;
    public void MarkInputDirty() => isInputDirty = true;
    public void MarkPlayerDirty() => isPlayerDirty = true;
    public void MarkQuestDirty() => isQuestDirty = true;

    public void ResetSave()
    {
        
    }
}
