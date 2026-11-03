using UnityEngine;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    public TMP_Text ammoText;

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }
    }
}