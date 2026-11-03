
using UnityEngine;
using UnityEngine.InputSystem;

public class QuestTester : MonoBehaviour
{
    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.kKey.wasPressedThisFrame)
        {
            QuestManager.Instance.UpdateObjective(
                "first_quest",
                "kill_enemy",
                1
            );
        }
    }
}
