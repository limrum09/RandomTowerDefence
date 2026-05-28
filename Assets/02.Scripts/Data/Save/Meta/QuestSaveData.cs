using Firebase.Firestore;
using System;
using System.Collections.Generic;

/// <summary>
/// 이름은 QuestSaveData이지만 Achievement만 저장되기에 AchievementSaveData로 보면됨
/// UID로 업적을 찾고, stat에 상태를 넣고, currentSuccess에 진행도를 넣는다.
/// </summary>
[Serializable]
[FirestoreData]
public class QuestSaveData
{
    [FirestoreProperty] public string UID { get; set; }
    [FirestoreProperty] public int stat { get; set; }
    [FirestoreProperty] public int currentSuccess { get; set; }
}

[Serializable]
[FirestoreData]
public class QuestSaveDataList
{
    [FirestoreProperty] public List<QuestSaveData> datas { get; set; } = new List<QuestSaveData>();
}
