using Firebase.Firestore;
using System;

[Serializable]
[FirestoreData]
public class MetaUpgradeSaveData
{
    [FirestoreProperty] public TowerMetaUpgradeData towerMetaSaveData { get; set; } = new TowerMetaUpgradeData();
    [FirestoreProperty] public PublicMetaUpgradeData publicMetaSaveData { get; set; } = new PublicMetaUpgradeData();
}
