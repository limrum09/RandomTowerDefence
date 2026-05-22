using System;
using System.Collections.Generic;
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
        achievementDatas = Resources.Load<AchievementDatas>("Achievement/AchievementDatabase");
    }

    private void QuestRecieveReport(List<Quest> quests, string category, object target, int successCount)
    {
        foreach(var quest in quests)
        {
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

    public Quest QeustRegistger(Quest quest)
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

    public void QuestRecieveReport(Category category, TaskTarget target, int successCount)
        => QuestRecieveReport(category.CategoryUID, target.Value, successCount);
    public void QuestRecieveReport(string category, object target, int successCount)
    {
        QuestRecieveReport(activeQuest, category, target, successCount);
        QuestRecieveReport(activeAchievement, category, target, successCount);
    }

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

        if(saveDataList == null)
        {
            List<Achievement> lists = achievementDatas.GetAllAchievement();

            foreach(var achievement in lists)
            {
                QeustRegistger(achievement);
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
    }
}
