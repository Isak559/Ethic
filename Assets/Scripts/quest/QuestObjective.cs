
using UnityEngine;

[System.Serializable]
public class QuestObjective
{
    [Header("Objective Information")]
    public string objectiveID;

    [TextArea(2, 4)]
    public string description;

    [Header("Requirements")]
    public int requiredAmount = 1;
}
