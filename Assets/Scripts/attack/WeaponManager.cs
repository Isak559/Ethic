using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Setup")]
    public Transform weaponHolder;
    public GameObject startingWeapon;

    [Header("Ammo UI")]
    public TMP_Text ammoText;

    private List<GameObject> unlockedWeapons = new List<GameObject>();

    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;

    private void Start()
    {
        if (startingWeapon != null)
        {
            UnlockWeapon(startingWeapon);
        }
    }

    private void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame)
                EquipWeapon(0);

            if (Keyboard.current.digit2Key.wasPressedThisFrame)
                EquipWeapon(1);

            if (Keyboard.current.digit3Key.wasPressedThisFrame)
                EquipWeapon(2);
        }

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

        foreach (GameObject weapon in unlockedWeapons)
        {
            if (weapon.name == weaponPrefab.name + "(Clone)")
            {
                Debug.Log("Weapon already unlocked!");
                return;
            }
        }

        GameObject newWeapon = Instantiate(weaponPrefab, weaponHolder);

        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;

        newWeapon.SetActive(false);

        unlockedWeapons.Add(newWeapon);

        Debug.Log("Unlocked weapon: " + weaponPrefab.name);

        EquipWeapon(unlockedWeapons.Count - 1);
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= unlockedWeapons.Count)
            return;

        foreach (GameObject weapon in unlockedWeapons)
        {
            Shooting gun = weapon.GetComponentInChildren<Shooting>(true);
            KnifeShooting knife = weapon.GetComponentInChildren<KnifeShooting>(true);

            if (gun != null)
                gun.enabled = false;

            if (knife != null)
                knife.enabled = false;

            weapon.SetActive(false);
        }

        currentWeaponIndex = index;
        currentWeapon = unlockedWeapons[index];

        currentWeapon.SetActive(true);

        Shooting selectedGun =
            currentWeapon.GetComponentInChildren<Shooting>(true);

        KnifeShooting selectedKnife =
            currentWeapon.GetComponentInChildren<KnifeShooting>(true);

        if (selectedGun != null)
        {
            selectedGun.enabled = true;

            AmmoCounter counter = GetAmmoCounter();

            selectedGun.ammoCounter = counter;

            if (counter != null)
                counter.UpdateAmmo(
                    GetCurrentAmmo(selectedGun),
                    selectedGun.maxAmmo
                );
        }
        else
        {
            if (ammoText != null)
                ammoText.text = "";
        }

        if (selectedKnife != null)
            selectedKnife.enabled = true;

        Debug.Log("Equipped: " + currentWeapon.name);
    }

    private AmmoCounter GetAmmoCounter()
    {
        if (ammoText == null)
            return null;

        AmmoCounter counter = ammoText.GetComponent<AmmoCounter>();

        if (counter == null)
            counter = ammoText.gameObject.AddComponent<AmmoCounter>();

        counter.ammoText = ammoText;

        return counter;
    }

    private int GetCurrentAmmo(Shooting gun)
    {
        return gun.GetCurrentAmmo();
    }

    private void SwitchWeapon(int direction)
    {
        if (unlockedWeapons.Count <= 1)
            return;

        int newIndex = currentWeaponIndex + direction;

        if (newIndex < 0)
            newIndex = unlockedWeapons.Count - 1;

        if (newIndex >= unlockedWeapons.Count)
            newIndex = 0;

        EquipWeapon(newIndex);
    }
}