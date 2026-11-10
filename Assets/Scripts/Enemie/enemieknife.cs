
using UnityEngine;

public class EnemyKnife : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public Transform attackPoint;

    [Header("Attack")]
    public float damage = 25f;
    public float attackRange = 2f;
    public float attackCooldown = 1.2f;

    private float nextAttackTime;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (player == null || attackPoint == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void Attack()
    {
        Vector3 direction = (
            player.position + Vector3.up * 0.8f
            - attackPoint.position
        ).normalized;

        RaycastHit hit;

        if (Physics.Raycast(
            attackPoint.position,
            direction,
            out hit,
            attackRange))
        {
            if (hit.transform.root == player.root)
            {
                Health playerHealth =
                    hit.transform.GetComponentInParent<Health>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }
}
