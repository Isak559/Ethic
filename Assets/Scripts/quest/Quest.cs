
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Information")]
    public string questID;
    public string questName;

    [TextArea(3, 6)]
    public string description;

    [Header("Objectives")]
    public List<QuestObjective> objectives = new List<QuestObjective>();

    [Header("Rewards")]
    public int rewardXP;
    public int rewardMoney;
}
