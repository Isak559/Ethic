using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Shooting : MonoBehaviour
{
    [Header("Gun Settings")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.25f;

    [Header("Ammo Settings")]
    public int maxAmmo = 30;
    public float reloadTime = 2f;

    private int currentAmmo;
    private bool isReloading = false;

    [Header("Ammo UI")]
    public AmmoCounter ammoCounter;

    [Header("References")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public GameObject impactEffect;
    public ParticleSystem muzzleParticles;
    public Light muzzleLight;

    [Header("Recoil")]
    public Transform gunTransform;
    public float recoilDistance = 0.08f;
    public float recoilRotation = 5f;
    public float recoilReturnSpeed = 12f;

    private float nextFireTime;
    private Vector3 gunStartPosition;
    private Quaternion gunStartRotation;
    private Coroutine recoilCoroutine;

    private void Start()
    {
        currentAmmo = maxAmmo;

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (gunTransform != null)
        {
            gunStartPosition = gunTransform.localPosition;
            gunStartRotation = gunTransform.localRotation;
        }

        if (muzzleLight != null)
            muzzleLight.enabled = false;

        UpdateAmmoUI();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame &&
            !isReloading &&
            currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        if (isReloading || Mouse.current == null || playerCamera == null)
            return;

        if (Mouse.current.leftButton.isPressed &&
            Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                nextFireTime = Time.time + fireRate;
                Shoot();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }
    }

    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleParticles != null)
            muzzleParticles.Play();

        if (muzzleLight != null)
            StartCoroutine(MuzzleFlash());

        if (gunTransform != null)
        {
            if (recoilCoroutine != null)
                StopCoroutine(recoilCoroutine);

            recoilCoroutine = StartCoroutine(ApplyRecoil());
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("GUN HIT: " + hit.collider.name);

            Health targetHealth =
                hit.collider.GetComponentInParent<Health>();

            if (targetHealth != null)
                targetHealth.TakeDamage(damage);

            if (impactEffect != null)
            {
                GameObject effect = Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );

                Destroy(effect, 2f);
            }
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        Debug.Log("Reloading...");

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;

        UpdateAmmoUI();

        isReloading = false;

        Debug.Log("Reload complete!");
    }

    private void UpdateAmmoUI()
    {
        if (ammoCounter != null)
        {
            ammoCounter.UpdateAmmo(currentAmmo, maxAmmo);
        }
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    private IEnumerator MuzzleFlash()
    {
        muzzleLight.enabled = true;

        yield return new WaitForSeconds(0.05f);

        if (muzzleLight != null)
            muzzleLight.enabled = false;
    }

    private IEnumerator ApplyRecoil()
    {
        gunTransform.localPosition =
            gunStartPosition - Vector3.forward * recoilDistance;

        gunTransform.localRotation =
            gunStartRotation *
            Quaternion.Euler(-recoilRotation, 0f, 0f);

        while (Vector3.Distance(
                   gunTransform.localPosition,
                   gunStartPosition) > 0.001f ||
               Quaternion.Angle(
                   gunTransform.localRotation,
                   gunStartRotation) > 0.1f)
        {
            gunTransform.localPosition = Vector3.Lerp(
                gunTransform.localPosition,
                gunStartPosition,
                Time.deltaTime * recoilReturnSpeed
            );

            gunTransform.localRotation = Quaternion.Slerp(
                gunTransform.localRotation,
                gunStartRotation,
                Time.deltaTime * recoilReturnSpeed
            );

            yield return null;
        }

        gunTransform.localPosition = gunStartPosition;
        gunTransform.localRotation = gunStartRotation;
    }
}