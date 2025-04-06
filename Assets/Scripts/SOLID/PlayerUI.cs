using UnityEngine;
using UnityEngine.UI;

public class PlayerUI
{
    private Slider _healthSlider;
    private GameObject _leftEye;
    private GameObject _rightEye;

    private int _maxHealth;

    public PlayerUI(Slider slider, GameObject leftEye, GameObject rightEye, int maxHealth)
    {
        _healthSlider = slider;
        _leftEye = leftEye;
        _rightEye = rightEye;
        _maxHealth = maxHealth;

        _healthSlider.maxValue = maxHealth;
    }

    public void UpdateHealth(int currentHealth)
    {
        _healthSlider.value = currentHealth;

        // Eye popping logic
        _leftEye.SetActive(currentHealth <= 2);
        _rightEye.SetActive(currentHealth <= 1);
    }

    public void ShowDeath()
    {
        Debug.Log("🐔 Chicken is dead!");
        // You can trigger animation or game over screen from here if needed
    }
}
