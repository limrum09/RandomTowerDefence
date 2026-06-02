using Firebase.Firestore;
using System;
using System.Collections.Generic;

/// <summary>
/// 이름은 QuestSaveData이지만 Achievement만 저장되기에 AchievementSaveData로 보면됨
/// UID로 업적을 찾고, stat에 상태를 넣고, currentSuccess에 진행도를 넣는다.
/// </summary>
[Serializable]
[FirestoreData]
public class QuestSaveData : IValidSaveData
{
    [FirestoreProperty] public string UID { get; set; }
    [FirestoreProperty] public int stat { get; set; }
    [FirestoreProperty] public int currentSuccess { get; set; }

    public bool IsValid()
    {
        bool isUID = !string.IsNullOrEmpty(UID);
        bool isStat = Enum.IsDefined(typeof(QuestStat), stat);
        bool isCurrentSuccess = currentSuccess >= 0;

        return isUID && isStat && isCurrentSuccess;
    }
}

[Serializable]
[FirestoreData]
public class QuestSaveDataList : IValidSaveData
{
    [FirestoreProperty] public List<QuestSaveData> datas { get; set; } = new List<QuestSaveData>();

    public bool IsValid()
    {
        if(datas == null)
            return false;

        foreach(QuestSaveData data in datas)
        {
            if (!data.IsValid())
                return false;
        }

        return true;
    }
}
