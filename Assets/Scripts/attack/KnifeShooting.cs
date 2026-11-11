
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
    public Animator knifeAnimator;

    private float nextAttackTime = 0f;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current != null &&
            Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        nextAttackTime = Time.time + attackRate;

        if (knifeAnimator != null)
        {
            knifeAnimator.SetTrigger("Attack");
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("KnifeShooting: Player Camera is missing!");
            return;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Health targetHealth =
                hit.collider.GetComponentInParent<Health>();

            if (targetHealth != null)
            {
                Debug.Log(
                    "KNIFE DAMAGE: " + damage +
                    " | Target: " + hit.collider.name +
                    " | Distance: " + hit.distance
                );

                targetHealth.TakeDamage(damage);
            }
            else
            {
                Debug.Log(
                    "KNIFE HIT: " + hit.collider.name +
                    " | No Health component found"
                );
            }
        }
        else
        {
            Debug.Log("KNIFE MISSED");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawRay(
                playerCamera.transform.position,
                playerCamera.transform.forward * range
            );
        }
    }
}
