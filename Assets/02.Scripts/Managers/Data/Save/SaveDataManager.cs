using Firebase.Firestore;
using System;
using System.IO;
using UnityEngine;

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

    public string SavePath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);
    public bool IsFirebaseLoadCompleted { get; private set; }
    public bool IsFirebaseLoadFail { get; private set; }
    public async System.Threading.Tasks.Task<bool> HasFirebaseSaveData(string uid)
    {
        DocumentSnapshot snapshot = await GetSaveDoc(uid, PLAYER_PROGRESS_FILE).GetSnapshotAsync();
        return snapshot.Exists;
    }
    
    public bool HasSignInUser()
    {
        return FirebaseInitializer.Instance != null &&
            FirebaseInitializer.Instance.Auth != null &&
            FirebaseInitializer.Instance.Auth.CurrentUser != null;
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

    private async System.Threading.Tasks.Task LoadMetaUpgradeData()
    {
        try
        {
            DocumentSnapshot snapshot = await GetSaveDoc(META_UPGRADE_SAVE_FILE).GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Managers.PublicMetaUpgrade.LoadSaveData(null);
                Managers.TowerMetaUpgrade.LoadSaveData(null);
                return;
            }

            MetaUpgradeSaveData saveData = snapshot.ConvertTo<MetaUpgradeSaveData>();

            Managers.PublicMetaUpgrade.LoadSaveData(saveData.publicMetaSaveData);
            Managers.TowerMetaUpgrade.LoadSaveData(saveData.towerMetaSaveData);
        }
        catch(Exception e)
        {
            Debug.LogError("Load Fail : Meta Upgrade Data - " + e.Message);
        }
    }

    private async System.Threading.Tasks.Task LoadPlayerProgressData()
    {
        try
        {
            DocumentSnapshot snapshot = await GetSaveDoc(PLAYER_PROGRESS_FILE).GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Managers.Player.LoadSaveData(null);
                return;
            }

            PlayerProgressData saveData = snapshot.ConvertTo<PlayerProgressData>();

            Managers.Player.LoadSaveData(saveData);
        }
        catch (Exception e)
        {
            Debug.LogError("Load Fail : Player Progress - " + e.Message);
        }
    }

    private async System.Threading.Tasks.Task LoadAchievementData()
    {
        try
        {
            DocumentSnapshot snapshot = await GetSaveDoc(QUEST_SAVE_FILE).GetSnapshotAsync();

            if (!snapshot.Exists)
            {
                Managers.QuestMgr.LoadSaveData(null);
                return;
            }

            QuestSaveDataList saveData = snapshot.ConvertTo<QuestSaveDataList>();

            Managers.QuestMgr.LoadSaveData(saveData);
        }
        catch (Exception e)
        {
            Debug.LogError("Load Fail : Achievement Data - " + e.Message);
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
            Debug.LogError($"False Load Player Progress Data {e.Message}");
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
            Debug.LogError($"False Save {saveData} {e.Message}");
        }
    }

    public async System.Threading.Tasks.Task<bool> LoadAllData()
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
            await LoadMetaUpgradeData();
            await LoadPlayerProgressData();
            await LoadAchievementData();

            IsFirebaseLoadCompleted = true;
        }
        catch (Exception e)
        {
            IsFirebaseLoadFail = true;
            return false;
        }
        

        LoadInputKeyData();
        LoadSoundData();
        LoadGraphicData();

        return true;
    }

    public async System.Threading.Tasks.Task SaveAllData()
    {
        SaveInputKeyData();
        SaveSoundData();
        SaveGraphicData();

        await SaveMetaUpgradeData();
        await SavePlayerProgressData();
        await SaveAchievementData();
    }

    public async System.Threading.Tasks.Task SaveMetaUpgradeData()
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

        await GetSaveDoc(META_UPGRADE_SAVE_FILE).SetAsync(saveData, SetOptions.MergeAll);

        isMetaUpgradeDirty = false;

        Debug.Log("Meta Upgrade Firestore Save Completed");
    }

    public async System.Threading.Tasks.Task SavePlayerProgressData()
    {
        if (!CanSaveFirebaseData())
            return;

        if (!isPlayerDirty)
            return;

        PlayerProgressData saveData = Managers.Player.GetSaveData();

        await GetSaveDoc(PLAYER_PROGRESS_FILE).SetAsync(saveData, SetOptions.MergeAll);

        isPlayerDirty = false;

        Debug.Log("Player Progress Firestore Save Completed");
    }

    public async System.Threading.Tasks.Task SaveAchievementData()
    {
        if (!CanSaveFirebaseData())
            return;

        if (!isQuestDirty)
            return;

        QuestSaveDataList saveData = Managers.QuestMgr.GetSaveData();

        await GetSaveDoc(QUEST_SAVE_FILE).SetAsync(saveData, SetOptions.MergeAll);

        isQuestDirty = false;

        Debug.Log("Achievement Firestore Save Completed");
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

    public async System.Threading.Tasks.Task SaveAllDirty()
    {
        MarkMetaUpgradeDirty();
        MarkSoundDirty();
        MarkInputDirty();
        MarkPlayerDirty();
        MarkGraphicDirty();
        MarkQuestDirty();

        await SaveAllData();
    }

    public async System.Threading.Tasks.Task CreateNewUserFirebaseSaveData(string uid)
    {
        PlayerProgressData playerData = new PlayerProgressData()
        {
            level = 1,
            exp = 0,
            metaCurrency = 0
        };

        await GetSaveDoc(uid, PLAYER_PROGRESS_FILE).SetAsync(playerData, SetOptions.Overwrite);
        await GetSaveDoc(uid, META_UPGRADE_SAVE_FILE).SetAsync(new MetaUpgradeSaveData(), SetOptions.Overwrite);
        await GetSaveDoc(uid, QUEST_SAVE_FILE).SetAsync(new QuestSaveDataList(), SetOptions.Overwrite);
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
