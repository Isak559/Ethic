using UnityEngine;

public class EnemyKnife : MonoBehaviour
{
    [Header("Knife Settings")]
    public float damage = 25f;
    public float attackRange = 2f;
    public float attackCooldown = 1.2f;

    [Header("References")]
    public Transform player;
    public Transform attackPoint;

    private float nextAttackTime;

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

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

        if (distance <= attackRange &&
            Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackCooldown;

        Vector3 targetPosition =
            player.position + Vector3.up * 0.8f;

        Vector3 direction =
            (targetPosition - attackPoint.position).normalized;

        float distance = Vector3.Distance(
            attackPoint.position,
            targetPosition
        );

        if (Physics.Raycast(
            attackPoint.position,
            direction,
            out RaycastHit hit,
            distance))
        {
            if (hit.transform.root == player.root)
            {
                Health playerHealth =
                    hit.collider.GetComponentInParent<Health>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Knife enemy attacked the player!");
                }
            }
        }
    }
}