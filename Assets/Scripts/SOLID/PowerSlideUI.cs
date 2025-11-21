using UnityEngine;
using UnityEngine.UI;

public class PowerSlideUI
{
    public void UpdatePowerSlider(float value, Slider slider)
    {
        slider.value = Mathf.Clamp01(value);
    }
    public float CurrentPoweSliderValue(Slider slider)
    {
        return slider.value;
    }
}
