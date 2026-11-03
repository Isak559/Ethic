
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;

    private float _currentHealth;
    private bool isDead = false;

    [SerializeField]
    private FloatingHealthBar healthBar;

    [Header("Quest Settings")]
    public bool countsAsEnemyForQuests = false;

    private void Start()
    {
        _currentHealth = maxHealth;

        UpdateHealthBar();

        // Test dummy heals 100 health every 5 seconds
        if (gameObject.name == "test dummy")
        {
            InvokeRepeating(
                nameof(HealTestDummy),
                5f,
                5f
            );
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        _currentHealth -= damage;

        _currentHealth = Mathf.Clamp(
            _currentHealth,
            0,
            maxHealth
        );

        UpdateHealthBar();

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead)
            return;

        _currentHealth += amount;

        _currentHealth = Mathf.Clamp(
            _currentHealth,
            0,
            maxHealth
        );

        UpdateHealthBar();
    }

    private void HealTestDummy()
    {
        Heal(100f);
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(
                _currentHealth,
                maxHealth
            );
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log(gameObject.name + " died!");

        // Register enemy kill for quests
        if (countsAsEnemyForQuests &&
            QuestManager.Instance != null)
        {
            QuestManager.Instance.UpdateObjectiveByID(
                "kill_enemy",
                1
            );
        }

        Destroy(gameObject);
    }
}
