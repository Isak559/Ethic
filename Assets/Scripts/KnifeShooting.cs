
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

    private float nextAttackTime;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current == null || playerCamera == null)
            return;

        // Attack when left mouse button is pressed
        if (Mouse.current.leftButton.wasPressedThisFrame &&
            Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            Attack();
        }
    }

    private void Attack()
    {
        Debug.Log("Knife attack!");

        // Play knife attack animation
        if (knifeAnimator != null)
        {
            knifeAnimator.SetTrigger("Attack");
        }

        // Raycast from the centre of the screen
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Knife hit: " + hit.collider.name);

            // Find enemy health component
            Health targetHealth =
                hit.collider.GetComponentInParent<Health>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);

                Debug.Log("Knife dealt " + damage + " damage!");
            }
        }
    }
}
