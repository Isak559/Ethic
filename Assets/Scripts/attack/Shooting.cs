
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Shooting : MonoBehaviour
{
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 0.25f;

    public Camera playerCamera;
    public Transform muzzlePoint;
    public GameObject impactEffect;
    public ParticleSystem muzzleParticles;
    public Light muzzleLight;

    public Transform gunTransform;
    public float recoilDistance = 0.08f;
    public float recoilRotation = 5f;
    public float recoilReturnSpeed = 12f;

    private float _nextFireTime;
    private Vector3 _gunStartPosition;
    private Quaternion _gunStartRotation;
    private Coroutine _recoilCoroutine;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (gunTransform != null)
        {
            _gunStartPosition = gunTransform.localPosition;
            _gunStartRotation = gunTransform.localRotation;
        }

        if (muzzleLight != null)
            muzzleLight.enabled = false;
    }


private void Update()
{
    if (Mouse.current == null || playerCamera == null)
        return;

    // Keep firing while left mouse button is held
    if (Mouse.current.leftButton.isPressed &&
        Time.time >= _nextFireTime)
    {
        _nextFireTime = Time.time + fireRate;
        Shoot();
    }
}


    private void Shoot()
    {
        // Muzzle flash particles
        if (muzzleParticles != null)
            muzzleParticles.Play();

        // Brief flash of light
        if (muzzleLight != null)
            StartCoroutine(MuzzleFlash());

        // Gun recoil
        if (gunTransform != null)
        {
            if (_recoilCoroutine != null)
                StopCoroutine(_recoilCoroutine);

            _recoilCoroutine = StartCoroutine(ApplyRecoil());
        }

        // Shoot towards the centre of the screen
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            Debug.Log("Hit: " + hit.collider.name);

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
            _gunStartPosition - Vector3.forward * recoilDistance;

        gunTransform.localRotation =
            _gunStartRotation *
            Quaternion.Euler(-recoilRotation, 0f, 0f);

        while (Vector3.Distance(
                   gunTransform.localPosition,
                   _gunStartPosition) > 0.001f ||
               Quaternion.Angle(
                   gunTransform.localRotation,
                   _gunStartRotation) > 0.1f)
        {
            gunTransform.localPosition = Vector3.Lerp(
                gunTransform.localPosition,
                _gunStartPosition,
                Time.deltaTime * recoilReturnSpeed
            );

            gunTransform.localRotation = Quaternion.Slerp(
                gunTransform.localRotation,
                _gunStartRotation,
                Time.deltaTime * recoilReturnSpeed
            );

            yield return null;
        }

        gunTransform.localPosition = _gunStartPosition;
        gunTransform.localRotation = _gunStartRotation;
    }
}
