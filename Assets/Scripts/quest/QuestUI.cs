
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [Header("UI References")]
    public Transform questList;
    public QuestUIItem questPrefab;

    private void Start()
    {
        QuestManager.Instance.OnQuestsChanged += RefreshUI;

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestsChanged -= RefreshUI;
        }
    }

    public void RefreshUI()
    {
        // Remove old quest items
        foreach (Transform child in questList)
        {
            Destroy(child.gameObject);
        }

        // Create UI for active quests
        foreach (Quest quest in QuestManager.Instance.activeQuests)
        {
            QuestUIItem item = Instantiate(
                questPrefab,
                questList
            );

            item.Setup(quest, QuestManager.Instance);
        }
    }
}
