
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform weaponHolder;
    public GameObject startingWeapon;
    public List<GameObject> unlockedWeapons = new List<GameObject>();

    [Header("UI")]
    public AmmoCounter ammoCounter;

    private List<GameObject> weaponInstances = new List<GameObject>();

    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;

    private void Start()
    {
        if (startingWeapon != null &&
            !unlockedWeapons.Contains(startingWeapon))
        {
            unlockedWeapons.Insert(0, startingWeapon);
        }

        for (int i = 0; i < unlockedWeapons.Count; i++)
        {
            GameObject weapon = Instantiate(
                unlockedWeapons[i],
                weaponHolder
            );

            weapon.SetActive(false);
            weaponInstances.Add(weapon);
        }

        if (weaponInstances.Count > 0)
        {
            EquipWeapon(0);
        }
    }

    private void Update()
    {
        HandleWeaponSwitching();
        UpdateAmmoUI();
    }

    private void HandleWeaponSwitching()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                EquipWeapon(0);

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                EquipWeapon(1);

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
                EquipWeapon(2);

            // Also support the numpad keys.
            if (Keyboard.current.numpad1Key.wasPressedThisFrame)
                EquipWeapon(0);

            if (Keyboard.current.numpad2Key.wasPressedThisFrame)
                EquipWeapon(1);

            if (Keyboard.current.numpad3Key.wasPressedThisFrame)
                EquipWeapon(2);
        }

        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;

            if (scroll > 0f)
                SwitchWeapon(1);

            if (scroll < 0f)
                SwitchWeapon(-1);
        }
    }

    private void SwitchWeapon(int direction)
    {
        if (weaponInstances.Count <= 1)
            return;

        int newIndex = currentWeaponIndex + direction;

        if (newIndex >= weaponInstances.Count)
            newIndex = 0;

        if (newIndex < 0)
            newIndex = weaponInstances.Count - 1;

        EquipWeapon(newIndex);
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponInstances.Count)
            return;

        if (currentWeapon != null)
            currentWeapon.SetActive(false);

        currentWeaponIndex = index;
        currentWeapon = weaponInstances[index];

        currentWeapon.SetActive(true);

        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoCounter == null)
            return;

        if (currentWeapon == null)
        {
            ammoCounter.HideAmmo();
            return;
        }

        Shooting shooting =
            currentWeapon.GetComponentInChildren<Shooting>();

        KnifeShooting knife =
            currentWeapon.GetComponentInChildren<KnifeShooting>();

        if (shooting != null)
        {
            shooting.ammoCounter = ammoCounter;
            shooting.UpdateAmmoUI();
        }
        else if (knife != null)
        {
            ammoCounter.HideAmmo();
        }
        else
        {
            ammoCounter.HideAmmo();
        }
    }

    public void UnlockWeapon(GameObject weaponPrefab)
    {
        if (weaponPrefab == null)
            return;

        if (unlockedWeapons.Contains(weaponPrefab))
            return;

        unlockedWeapons.Add(weaponPrefab);

        GameObject weapon = Instantiate(
            weaponPrefab,
            weaponHolder
        );

        weapon.SetActive(false);
        weaponInstances.Add(weapon);
    }

    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
