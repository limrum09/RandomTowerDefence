using Firebase.Firestore;
using System;

[Serializable]
[FirestoreData]
public class MetaUpgradeSaveData : IValidSaveData
{
    [FirestoreProperty] public TowerMetaUpgradeData towerMetaSaveData { get; set; } = new TowerMetaUpgradeData();
    [FirestoreProperty] public PublicMetaUpgradeData publicMetaSaveData { get; set; } = new PublicMetaUpgradeData();

    public bool IsValid()
    {
        return towerMetaSaveData.IsValid() && publicMetaSaveData.IsValid();
    }
}
