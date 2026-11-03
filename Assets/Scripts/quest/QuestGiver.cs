
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestGiver : MonoBehaviour
{
    [Header("Quest")]
    public Quest questToGive;

    [Header("Interaction")]
    public float interactionDistance = 3f;

    private bool playerInRange;

    private void Update()
    {
        if (playerInRange &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            GiveQuest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press E to interact with " + gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GiveQuest()
    {
        if (questToGive == null)
        {
            Debug.LogWarning("No quest assigned to NPC!");
            return;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager not found!");
            return;
        }

        QuestManager.Instance.AcceptQuest(questToGive);
    }
}
