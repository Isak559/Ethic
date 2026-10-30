using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;

    private float _currentHealth;

    [Header("Enemy Health Bar")]
    [SerializeField] private FloatingHealthBar floatingHealthBar;

    [Header("Player Health Bar")]
    [SerializeField] private Slider playerHealthBar;

    [Header("Test Dummy Regen")]
    [SerializeField] private float healDelay = 3f;      // Seconds to wait after damage
    [SerializeField] private float healInterval = 1f;   // Seconds between heal ticks
    [SerializeField] private float healAmount = 100f;   // Health restored per tick

    private float timeSinceDamage = 0f;
    private float healTimer = 0f;

    private void Start()
    {
        _currentHealth = maxHealth;

        UpdateHealthBars();
    }

    private void Update()
    {
        if (gameObject.name != "test dummy") return;
        if (_currentHealth >= maxHealth) return;

        // Wait for the delay after the last hit
        timeSinceDamage += Time.deltaTime;
        if (timeSinceDamage < healDelay) return;

        // Then heal once per interval until full
        healTimer += Time.deltaTime;

        if (healTimer >= healInterval)
        {
            healTimer -= healInterval;

            _currentHealth = Mathf.Min(_currentHealth + healAmount, maxHealth);

            UpdateHealthBars();

            Debug.Log("Test Dummy healed " + healAmount + " health!");
        }
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        _currentHealth = Mathf.Clamp(
            _currentHealth, 0f, maxHealth
        );

        // Restart the regen delay on every hit
        timeSinceDamage = 0f;
        healTimer = 0f;

        UpdateHealthBars();

        Debug.Log(
            gameObject.name + " health: " + _currentHealth
        );

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void UpdateHealthBars()
    {
        // Enemy floating health bar
        if (floatingHealthBar != null)
        {
            floatingHealthBar.UpdateHealthBar(
                _currentHealth, maxHealth
            );
        }

        // Player screen-space health bar
        if (playerHealthBar != null)
        {
            playerHealthBar.minValue = 0f;
            playerHealthBar.maxValue = maxHealth;
            playerHealthBar.value = _currentHealth;
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}