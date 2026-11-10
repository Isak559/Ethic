
using UnityEngine;
using System.Collections;

public class EnemyShooting : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public Transform firePoint;

    [Header("Shooting")]
    public float damage = 10f;
    public float fireRate = 1f;
    public float shootingRange = 20f;

    [Range(0f, 1f)]
    public float accuracy = 1f / 6f;

    public float missAngle = 15f;

    [Header("Effects")]
    public GameObject muzzleFlash;
    public LineRenderer bulletLine;
    public float lineDuration = 0.1f;

    private float nextFireTime;

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
        if (player == null || firePoint == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        if (distance <= shootingRange && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    private void Shoot()
    {
        Vector3 direction = (player.position + Vector3.up * 0.8f
            - firePoint.position).normalized;

        if (Random.value > accuracy)
        {
            direction = Quaternion.Euler(
                Random.Range(-missAngle, missAngle),
                Random.Range(-missAngle, missAngle),
                0f
            ) * direction;
        }

        if (muzzleFlash != null)
            muzzleFlash.SetActive(true);

        RaycastHit hit;

        if (Physics.Raycast(
            firePoint.position,
            direction,
            out hit,
            shootingRange))
        {
            if (hit.transform.root == player.root)
            {
                Health playerHealth =
                    hit.transform.GetComponentInParent<Health>();

                if (playerHealth != null)
                    playerHealth.TakeDamage(damage);
            }

            if (bulletLine != null)
            {
                StartCoroutine(ShowBulletLine(
                    firePoint.position,
                    hit.point
                ));
            }
        }

        if (muzzleFlash != null)
            StartCoroutine(DisableMuzzleFlash());
    }

    private IEnumerator ShowBulletLine(Vector3 start, Vector3 end)
    {
        bulletLine.enabled = true;
        bulletLine.SetPosition(0, start);
        bulletLine.SetPosition(1, end);

        yield return new WaitForSeconds(lineDuration);

        bulletLine.enabled = false;
    }

    private IEnumerator DisableMuzzleFlash()
    {
        yield return new WaitForSeconds(0.1f);

        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);
    }
}
