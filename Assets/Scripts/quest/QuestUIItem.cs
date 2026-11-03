
using UnityEngine;
using TMPro;

public class QuestUIItem : MonoBehaviour
{
    public TMP_Text questName;
    public TMP_Text questDescription;
    public TMP_Text questObjectives;

    public void Setup(Quest quest, QuestManager manager)
    {
        if (quest == null || manager == null)
        {
            Debug.LogError("Quest or QuestManager is missing!");
            return;
        }

        if (questName == null)
        {
            Debug.LogError("Quest Name reference is missing!");
            return;
        }

        if (questDescription == null)
        {
            Debug.LogError("Quest Description reference is missing!");
            return;
        }

        if (questObjectives == null)
        {
            Debug.LogError("Quest Objectives reference is missing!");
            return;
        }

        questName.text = quest.questName;
        questDescription.text = quest.description;

        string objectiveText = "Objectives:\n";

        foreach (QuestObjective objective in quest.objectives)
        {
            int progress = manager.GetObjectiveProgress(
                quest.questID,
                objective.objectiveID
            );

            objectiveText += objective.description + ": "
                + progress + "/" + objective.requiredAmount + "\n";
        }

        questObjectives.text = objectiveText;
    }
}
