
using UnityEngine;
using System;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Quest Tracking")]
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();

    private Dictionary<string, int> objectiveProgress =
        new Dictionary<string, int>();

    public event Action OnQuestsChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AcceptQuest(Quest quest)
    {
        if (quest == null)
            return;

        if (activeQuests.Contains(quest) ||
            completedQuests.Contains(quest))
        {
            Debug.Log("Quest already accepted or completed.");
            return;
        }

        activeQuests.Add(quest);

        foreach (QuestObjective objective in quest.objectives)
        {
            string key = quest.questID + "_" + objective.objectiveID;
            objectiveProgress[key] = 0;
        }

        Debug.Log("Quest accepted: " + quest.questName);

        OnQuestsChanged?.Invoke();
    }

    public void UpdateObjective(
        string questID,
        string objectiveID,
        int amount = 1)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);

        if (quest == null)
            return;

        QuestObjective objective = quest.objectives.Find(
            o => o.objectiveID == objectiveID
        );

        if (objective == null)
            return;

        string key = questID + "_" + objectiveID;

        if (!objectiveProgress.ContainsKey(key))
            return;

        objectiveProgress[key] = Mathf.Clamp(
            objectiveProgress[key] + amount,
            0,
            objective.requiredAmount
        );

        Debug.Log(
            objective.description + ": " +
            objectiveProgress[key] + "/" +
            objective.requiredAmount
        );

        OnQuestsChanged?.Invoke();

        CheckQuestCompletion(quest);
    }

    // Updates matching objectives in ALL active quests
    public void UpdateObjectiveByID(
        string objectiveID,
        int amount = 1)
    {
        // Create a copy because quests can be removed
        // from activeQuests when completed.
        List<Quest> quests = new List<Quest>(activeQuests);

        foreach (Quest quest in quests)
        {
            UpdateObjective(
                quest.questID,
                objectiveID,
                amount
            );
        }
    }

    private void CheckQuestCompletion(Quest quest)
    {
        foreach (QuestObjective objective in quest.objectives)
        {
            string key = quest.questID + "_" + objective.objectiveID;

            if (!objectiveProgress.ContainsKey(key))
                return;

            if (objectiveProgress[key] < objective.requiredAmount)
                return;
        }

        CompleteQuest(quest);
    }

    private void CompleteQuest(Quest quest)
    {
        activeQuests.Remove(quest);
        completedQuests.Add(quest);

        Debug.Log("Quest completed: " + quest.questName);
        Debug.Log("XP Reward: " + quest.rewardXP);
        Debug.Log("Money Reward: " + quest.rewardMoney);

        OnQuestsChanged?.Invoke();
    }

    public int GetObjectiveProgress(
        string questID,
        string objectiveID)
    {
        string key = questID + "_" + objectiveID;

        if (objectiveProgress.ContainsKey(key))
            return objectiveProgress[key];

        return 0;
    }
}
