
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    public Transform weaponHolder;
    public GameObject startingWeapon;

    private List<GameObject> unlockedWeapons = new List<GameObject>();

    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;

    private void Start()
    {
        // Give the player their starting weapon
        if (startingWeapon != null)
        {
            UnlockWeapon(startingWeapon);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        // Number key weapon switching
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            EquipWeapon(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            EquipWeapon(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            EquipWeapon(2);

        // Mouse scroll wheel weapon switching
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll > 0)
                SwitchWeapon(-1);

            if (scroll < 0)
                SwitchWeapon(1);
        }
    }

    public void UnlockWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null || weaponHolder == null)
            return;

        // Prevent duplicate weapons
        foreach (GameObject weapon in unlockedWeapons)
        {
            if (weapon.name == weaponPrefab.name + "(Clone)")
            {
                Debug.Log("Weapon already unlocked!");
                return;
            }
        }

        // Instantiate weapon under WeaponHolder
        GameObject newWeapon = Instantiate(
            weaponPrefab,
            weaponHolder
        );

        // Reset position and rotation
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        // Preserve original prefab scale

        // Disable weapon until equipped
        newWeapon.SetActive(false);

        // Add weapon to inventory
        unlockedWeapons.Add(newWeapon);

        Debug.Log("Unlocked weapon: " + weaponPrefab.name);

        // Automatically equip new weapon
        EquipWeapon(unlockedWeapons.Count - 1);
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= unlockedWeapons.Count)
            return;

        // Hide previous weapon
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }

        // Equip selected weapon
        currentWeaponIndex = index;
        currentWeapon = unlockedWeapons[index];

        currentWeapon.SetActive(true);

        Debug.Log("Equipped: " + currentWeapon.name);
    }

    private void SwitchWeapon(int direction)
    {
        if (unlockedWeapons.Count <= 1)
            return;

        int newIndex = currentWeaponIndex + direction;

        // Wrap around to last weapon
        if (newIndex < 0)
            newIndex = unlockedWeapons.Count - 1;

        // Wrap around to first weapon
        if (newIndex >= unlockedWeapons.Count)
            newIndex = 0;

        EquipWeapon(newIndex);
    }
}
