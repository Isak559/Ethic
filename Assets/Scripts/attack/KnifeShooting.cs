
using UnityEngine;
using UnityEngine.InputSystem;

public class KnifeShooting : MonoBehaviour
{
    [Header("Knife Settings")]
    public float damage = 50f;
    public float range = 2f;
    public float attackRate = 0.6f;

    [Header("References")]
    public Camera playerCamera;

    private float nextAttackTime = 0f;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null || playerCamera == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame &&
            Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackRate;

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log(
                "KNIFE HIT: " + hit.collider.name +
                " | Distance: " + hit.distance +
                " | Range: " + range
            );

            Health enemyHealth =
                hit.collider.GetComponentInParent<Health>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log("KNIFE DAMAGE: " + damage);
            }
        }
        else
        {
            Debug.Log("KNIFE MISSED! Range: " + range);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward * range
        );
    }
}
