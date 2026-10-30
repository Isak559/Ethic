
using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public Transform firePoint;

    [Header("Shooting Settings")]
    public float damage = 10f;
    public float fireRate = 1f;
    public float shootingRange = 20f;

    [Header("Accuracy Settings")]
    [Range(0f, 1f)]
    public float accuracy = 1f / 6f;

    public float missAngle = 15f;

    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public LineRenderer bulletLine;
    public float lineDuration = 0.1f;

    private float nextFireTime;
    private Coroutine lineCoroutine;

    private void Start()
    {
        if (bulletLine != null)
        {
            bulletLine.positionCount = 2;
            bulletLine.useWorldSpace = true;
            bulletLine.enabled = false;
        }
    }

    private void Update()
    {
        if (player == null || firePoint == null)
            return;

        // Check distance to player
        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance > shootingRange || fireRate <= 0f)
            return;

        // Shoot at set intervals
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot()
    {
        // Direction towards player
        Vector3 direction =
            (player.position - firePoint.position).normalized;

        // Decide if this shot is accurate
        bool accurateShot = Random.value < accuracy;

        if (!accurateShot)
        {
            // Add random bullet spread
            float randomX = Random.Range(
                -missAngle, missAngle
            );

            float randomY = Random.Range(
                -missAngle, missAngle
            );

            direction = Quaternion.Euler(
                randomX,
                randomY,
                0f
            ) * direction;
        }

        // Default bullet endpoint
        Vector3 endPoint =
            firePoint.position + direction * shootingRange;

        // Play muzzle flash
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }

        // Fire raycast
        if (Physics.Raycast(
            firePoint.position,
            direction,
            out RaycastHit hit,
            shootingRange))
        {
            endPoint = hit.point;

            // Check for player health
            Health health =
                hit.collider.GetComponentInParent<Health>();

            if (health != null &&
                hit.transform.root == player.root)
            {
                health.TakeDamage(damage);
            }
        }

        // Draw bullet tracer
        if (bulletLine != null)
        {
            if (lineCoroutine != null)
            {
                StopCoroutine(lineCoroutine);
            }

            lineCoroutine = StartCoroutine(
                ShowBulletLine(
                    firePoint.position,
                    endPoint
                )
            );
        }
    }

    private IEnumerator ShowBulletLine(
        Vector3 start,
        Vector3 end)
    {
        bulletLine.SetPosition(0, start);
        bulletLine.SetPosition(1, end);

        bulletLine.enabled = true;

        yield return new WaitForSeconds(lineDuration);

        bulletLine.enabled = false;
        lineCoroutine = null;
    }
}
