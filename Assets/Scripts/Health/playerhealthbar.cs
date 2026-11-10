
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        if (slider == null)
            return;

        slider.maxValue = maxValue;
        slider.value = currentValue;
    }
}
