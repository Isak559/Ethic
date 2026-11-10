
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform questList;
    public QuestUIItem questPrefab;

    private QuestManager questManager;

    private void Start()
    {
        questManager = QuestManager.Instance;

        if (questManager == null)
        {
            Debug.LogError("QuestManager not found!");
            return;
        }

        if (questList == null || questPrefab == null)
        {
            Debug.LogError("Quest UI references are missing!");
            return;
        }

        questManager.OnQuestsChanged += RefreshUI;

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (questManager != null)
        {
            questManager.OnQuestsChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        if (questManager == null ||
            questList == null ||
            questPrefab == null)
            return;

        foreach (Transform child in questList)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest quest in questManager.activeQuests)
        {
            if (quest == null)
                continue;

            QuestUIItem item = Instantiate(
                questPrefab,
                questList
            );

            item.Setup(quest, questManager);
        }
    }
}
