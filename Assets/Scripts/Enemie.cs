
using UnityEngine;

public class Enemie : MonoBehaviour
{
    private Health health;

    private void Start()
    {
        health = GetComponent<Health>();
    }

    public void TakeDamage(float damage)
    {
        if (health != null)
        {
            health.TakeDamage(damage);
        }
    }
}
