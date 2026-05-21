using System;
using System.IO;
using UnityEngine;

public class SaveDataManager
{
    private const string META_UPGRADE_SAVE_FILE = "meta_upgrade_save.json";
    private const string PLAYER_PROGRESS_FILE = "meta_player_progress_save.json";
    private const string INPUTKEY_SAVE_FILE = "inputkey_save.json";
    private const string SOUND_SAVE_FILE = "sound_save.json";
    private const string GRAPHIC_SAVE_FILE = "graphic_save.json";
    public bool isMetaUpgradeDirty;
    public bool isGraphicDirty;
    public bool isSoundDirty;
    public bool isInputDirty;
    public bool isPlayerDirty;
    public string SavePath(string fileName) => Path.Combine(Application.persistentDataPath, fileName);


    private void SaveMetaUpgradeData(MetaUpgradeSaveData saveData)
    {
        string path = SavePath(META_UPGRADE_SAVE_FILE);
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

            isMetaUpgradeDirty = false;
        }
        catch(Exception e)
        {
            Debug.LogError($"False Save Meta Upgrade Data {e.Message}");
        }
    }

    private void SavePlayerProgressData(PlayerProgressData saveData)
    {
        string path = SavePath(PLAYER_PROGRESS_FILE);
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

            isPlayerDirty = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"False Save Player Progresses Data {e.Message}");
        }
    }

    private void SaveInputKeyData(InputKeySaveData saveData)
    {
        string path = SavePath(INPUTKEY_SAVE_FILE);
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

            isInputDirty = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"False Save Input Key Data {e.Message}");
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
    
    private void LoadMetaUpgradeData()
    {
        string path = SavePath(META_UPGRADE_SAVE_FILE);

        if (!File.Exists(path))
        {
            Managers.PublicMetaUpgrade.Init(null);
            Managers.TowerMetaUpgrade.Init(null);
            Debug.Log(path + "에 파일이 없음");
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            MetaUpgradeSaveData saveData = JsonUtility.FromJson<MetaUpgradeSaveData>(json);

            Managers.PublicMetaUpgrade.Init(saveData.publicMetaSaveData);
            Managers.TowerMetaUpgrade.Init(saveData.towerMetaSaveData);
        }
        catch (Exception e)
        {
            Debug.LogError($"False Load Meta Upgrade Data {e.Message}");
        }
    }

    private void LoadPlayerProgressData()
    {
        string path = SavePath(PLAYER_PROGRESS_FILE);

        if (!File.Exists(path))
        {
            Managers.Player.Init(null);
            return;
        }

        try
        {
            string json = File.ReadAllText(path);
            PlayerProgressData saveData = JsonUtility.FromJson<PlayerProgressData>(json);

            Managers.Player.Init(saveData);
        }
        catch(Exception e)
        {
            Debug.LogError($"False Load Player Progress Data {e.Message}");
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

    public void SaveAllData()
    {
        SaveMetaUpgradeData();
        SavePlayerProgressData();
        SaveSoundData();
        SaveInputKeyData();
        SaveGraphicData();
    }

    public void LoadAllData()
    {
        LoadMetaUpgradeData();
        LoadPlayerProgressData();
        LoadInputKeyData();
        LoadSoundData();
        LoadGraphicData();
    }

    public void SaveMetaUpgradeData()
    {
        if (!isMetaUpgradeDirty)
            return;

        MetaUpgradeSaveData saveData = new MetaUpgradeSaveData()
        {
            publicMetaSaveData = Managers.PublicMetaUpgrade.GetPublicMetaSaveData(),
            towerMetaSaveData = Managers.TowerMetaUpgrade.GetTowerUpgradeSaveData()
        };
    }

    public void SavePlayerProgressData()
    {
        if (!isPlayerDirty)
            return;

        PlayerProgressData saveData = Managers.Player.GetSaveData();

        SavePlayerProgressData(saveData);
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

    public void MarkMetaUpgradeDirty()
    {
        isMetaUpgradeDirty = true;
    }

    public void MarkGraphicDirty()
    {
        isGraphicDirty = true;
    }

    public void MarkSoundDirty()
    {
        isSoundDirty = true;
    }

    public void MarkInputDirty()
    {
        isInputDirty = true;
    }

    public void MarkPlayerDirty()
    {
        isPlayerDirty = true;
    }

    public void SaveAllDirty()
    {
        MarkMetaUpgradeDirty();
        MarkSoundDirty();
        MarkInputDirty();
        MarkPlayerDirty();
        MarkGraphicDirty();

        SaveAllData();
    }

    public void ResetSave()
    {
        SaveMetaUpgradeData(new MetaUpgradeSaveData());
    }
}
