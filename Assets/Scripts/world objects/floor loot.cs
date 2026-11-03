
using UnityEngine;

public class FloorLoot : MonoBehaviour
{
    [Header("Floating")]
    public float floatAmplitude = 0.25f;
    public float floatFrequency = 2f;

    [Header("Rotation")]
    public float rotationSpeed = 90f;

    [Header("Weapon")]
    public GameObject weaponPrefab;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float newY = startPosition.y +
            Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        transform.Rotate(
            Vector3.up * rotationSpeed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        WeaponManager weaponManager =
            other.GetComponentInParent<WeaponManager>();

        if (weaponManager != null && weaponPrefab != null)
        {
            weaponManager.UnlockWeapon(weaponPrefab);
            Destroy(gameObject);
        }
    }
}
