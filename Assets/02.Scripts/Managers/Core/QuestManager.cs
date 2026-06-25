using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager
{
    public event Action<Quest> OnQuestRegister;
    public event Action<Quest> OnQuestComplete;
    public event Action<Quest> OnAchievementComplete;

    private List<Quest> activeQuest = new List<Quest>();
    private List<Quest> activeAchievement = new List<Quest>();
    private List<Quest> completeAchievement = new List<Quest>();

    private AchievementDatas achievementDatas;

    public IReadOnlyList<Quest> ActiveAchievement => activeAchievement;
    public IReadOnlyList<Quest> CompleteAchievement => completeAchievement;

    public void Init()
    {
        achievementDatas = ResourceCache.Load<AchievementDatas>("Achievement/AchievementDatabase");
    }

    private void QuestRecieveReport(List<Quest> quests, QuestCategory category, object target, int successCount)
    {
        List<Quest> questCopy = new List<Quest>(quests);
        foreach(var quest in questCopy)
        {
            if (quest == null)
                continue;

            if (!quests.Contains(quest))
                continue;

            quest.QuestRecieveReport(category, target, successCount);
        }
    }

    private void LoadAchievement(QuestSaveData saveData, Quest quest)
    {
        var newQuest = quest.Clone();
        newQuest.LoadQuestSaveData(saveData);

        if (newQuest.IsQuestComplete)
        {
            completeAchievement.Add(newQuest);
            return;
        }

        newQuest.OnQuestComplete += AchievementComplete;
        newQuest.QuestOnRegister();
        activeAchievement.Add(newQuest);
    }

    private void AchievementComplete(Quest quest)
    {
        OnAchievementComplete?.Invoke(quest);
        completeAchievement.Add(quest);
        activeAchievement.Remove(quest);

        Managers.Save.MarkQuestDirty();
        _ = Managers.Save.SaveAchievementData();
    }

    private void OnQuestCompleted(Quest quest)
    {
        OnQuestComplete?.Invoke(quest);
        activeQuest.Remove(quest);
    }

    private Achievement FindAchievementByUID(string questUID)
    {
        Achievement newQuest = achievementDatas.FindByCode(questUID);

        if (newQuest == null)
            return null;

        return newQuest;
    }

    public Quest QuestRegistger(Quest quest)
    {
        Quest newQuest = quest.Clone();

        if(newQuest is Achievement)
        {
            newQuest.OnQuestComplete += AchievementComplete;

            activeAchievement.Add(newQuest);

            newQuest.QuestOnRegister();
        }
        else
        {
            newQuest.OnQuestComplete += OnQuestCompleted;

            activeQuest.Add(newQuest);

            newQuest.QuestOnRegister();
            OnQuestRegister?.Invoke(newQuest);
        }

        return newQuest;
    }

    public void QuestRecieveReport(QuestCategory category, TaskTarget target, int successCount)
        => QuestRecieveReport(category, target.Value, successCount);
    public void QuestRecieveReport(QuestCategory category, object target, int successCount)
    {
        QuestRecieveReport(activeQuest, category, target, successCount);
        QuestRecieveReport(activeAchievement, category, target, successCount);
    }

    public bool ContainsActiveAchievement(Achievement achievement) => activeAchievement.Any(x => x.QuestUID == achievement.QuestUID);
    public bool ContainsCompletedAchievement(Achievement achievement) => completeAchievement.Any(x => x.QuestUID == achievement.QuestUID);

    public QuestSaveDataList GetSaveData()
    {
        QuestSaveDataList dataList = new QuestSaveDataList();

        foreach(var achievement in activeAchievement)
        {
            dataList.datas.Add(achievement.GetSaveData());
        }

        foreach(var achievement in completeAchievement)
        {
            dataList.datas.Add(achievement.GetSaveData());
        }

        return dataList;
    }

    public void LoadSaveData(QuestSaveDataList saveDataList)
    {
        if (achievementDatas == null)
            return;

        activeAchievement.Clear();
        completeAchievement.Clear();

        if(saveDataList == null || saveDataList.datas == null || saveDataList.datas.Count == 0)
        {
            List<Achievement> lists = achievementDatas.GetAllAchievement();

            foreach(var achievement in lists)
            {
                QuestRegistger(achievement);
            }

            return;
        }

        foreach (var saveData in saveDataList.datas)
        {
            Achievement newAchievement = FindAchievementByUID(saveData.UID);

            if (newAchievement == null)
                continue;

            LoadAchievement(saveData, newAchievement);
        }

        List<Achievement> allAchievement = achievementDatas.GetAllAchievement();

        foreach(var achievement in allAchievement)
        {
            bool isOldAchievement = activeAchievement.Any(x => x.QuestUID == achievement.QuestUID) ||
                completeAchievement.Any(x => x.QuestUID == achievement.QuestUID);

            if (isOldAchievement)
                continue;

            QuestRegistger(achievement);
        }
    }
}
