
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Shooting : MonoBehaviour
{
    [Header("Shooting")]
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.25f;

    [Header("Ammo")]
    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;

    [Header("References")]
    public Camera playerCamera;
    public Transform muzzlePoint;
    public GameObject impactEffect;
    public ParticleSystem muzzleParticles;
    public Light muzzleLight;
    public AmmoCounter ammoCounter;

    [Header("Recoil")]
    public Transform gunTransform;
    public float recoilDistance = 0.08f;
    public float recoilRotation = 5f;
    public float recoilReturnSpeed = 12f;

    private float nextTimeToFire;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Start()
    {
        currentAmmo = maxAmmo;

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (gunTransform != null)
        {
            originalPosition = gunTransform.localPosition;
            originalRotation = gunTransform.localRotation;
        }

        UpdateAmmoUI();
    }

    private void Update()
    {
        if (isReloading)
            return;

        if (Keyboard.current != null &&
            Keyboard.current.rKey.wasPressedThisFrame)
        {
            StartCoroutine(Reload());
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Mouse.current != null &&
            Mouse.current.leftButton.isPressed &&
            Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }

        ReturnRecoil();
    }

    private void Shoot()
    {
        currentAmmo--;
        UpdateAmmoUI();

        if (muzzleParticles != null)
            muzzleParticles.Play();

        if (muzzleLight != null)
            StartCoroutine(MuzzleFlash());

        if (playerCamera == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Health targetHealth =
                hit.transform.GetComponentInParent<Health>();

            if (targetHealth != null)
            {
                Debug.Log(
                    "GUN DAMAGE: " + damage +
                    " | Weapon: " + gameObject.name +
                    " | Target: " + hit.collider.name +
                    " | Distance: " + hit.distance
                );

                targetHealth.TakeDamage(damage);
            }

            if (impactEffect != null)
            {
                GameObject impact = Instantiate(
                    impactEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );

                Destroy(impact, 2f);
            }
        }

        ApplyRecoil();
    }

    private IEnumerator Reload()
    {
        if (isReloading || currentAmmo == maxAmmo)
            yield break;

        isReloading = true;

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;

        UpdateAmmoUI();
    }

    private void ApplyRecoil()
    {
        if (gunTransform == null)
            return;

        gunTransform.localPosition =
            originalPosition - new Vector3(0f, 0f, recoilDistance);

        gunTransform.localRotation =
            originalRotation * Quaternion.Euler(-recoilRotation, 0f, 0f);
    }

    private void ReturnRecoil()
    {
        if (gunTransform == null)
            return;

        gunTransform.localPosition = Vector3.Lerp(
            gunTransform.localPosition,
            originalPosition,
            Time.deltaTime * recoilReturnSpeed
        );

        gunTransform.localRotation = Quaternion.Lerp(
            gunTransform.localRotation,
            originalRotation,
            Time.deltaTime * recoilReturnSpeed
        );
    }

    private IEnumerator MuzzleFlash()
    {
        muzzleLight.enabled = true;

        yield return new WaitForSeconds(0.05f);

        muzzleLight.enabled = false;
    }

    public void UpdateAmmoUI()
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

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    public bool IsReloading()
    {
        return isReloading;
    }
}
