using System;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

public class SaveDataManager
{
    private const string META_UPGRADE_SAVE_FILE = "meta_upgrade_save.json";
    private const string PLAYER_PROGRESS_FILE = "meta_player_progress_save.json";
    public bool isMetaUpgradeDirty;
    public bool isOptionDirty;
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

    public void SaveAllData()
    {
        SaveMetaUpgradeData();
        SavePlayerProgressData();
    }

    public void LoadAllData()
    {
        LoadMetaUpgradeData();
        LoadPlayerProgressData();
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

        SaveMetaUpgradeData(saveData);
    }

    public void SavePlayerProgressData()
    {
        if (!isPlayerDirty)
            return;

        PlayerProgressData saveData = Managers.Player.GetSaveData();

        SavePlayerProgressData(saveData);
    }

    public void MarkMetaUpgradeDirty()
    {
        isMetaUpgradeDirty = true;
    }

    public void MarkOptionDirty()
    {
        isOptionDirty = true;
    }

    public void MarkInpuDirty()
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
        MarkOptionDirty();
        MarkInpuDirty();
        MarkPlayerDirty();

        SaveAllData();
    }

    public void ResetSave()
    {
        SaveMetaUpgradeData(new MetaUpgradeSaveData());
    }
}
