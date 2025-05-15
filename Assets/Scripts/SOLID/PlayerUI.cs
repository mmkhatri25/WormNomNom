using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI
{
    private Slider _healthSlider;
    private GameObject _leftEye;
    private GameObject _rightEye;
    private int _maxHealth;
    private MonoBehaviour _coroutineRunner;  // <== This is new

    private Coroutine _leftEyeCoroutine;
    private Coroutine _rightEyeCoroutine;
    

    public PlayerUI(Slider slider, GameObject leftEye, GameObject rightEye, int maxHealth, MonoBehaviour coroutineRunner)
    {
        _healthSlider = slider;
        _leftEye = leftEye;
        _rightEye = rightEye;
        _maxHealth = maxHealth;
        _coroutineRunner = coroutineRunner;

        _healthSlider.maxValue = maxHealth;
     
    }

    public void UpdateHealth(int currentHealth)
    {
        _healthSlider.value = currentHealth;

        // Left eye logic
        if (currentHealth > 1 && currentHealth <= 2)
        {
            _leftEye.SetActive(true);
            if (_leftEyeCoroutine != null) _coroutineRunner.StopCoroutine(_leftEyeCoroutine);
            _leftEyeCoroutine = _coroutineRunner.StartCoroutine(DisableAfterSeconds(_leftEye, 2f));
        }

        // Right eye logic
        else if (currentHealth <= 1)
        {
            _rightEye.SetActive(true);
            if (_rightEyeCoroutine != null) _coroutineRunner.StopCoroutine(_rightEyeCoroutine);
            _rightEyeCoroutine = _coroutineRunner.StartCoroutine(DisableAfterSeconds(_rightEye, 2f));
        }


    }

    private IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    public void ShowDeath()
    {
        Debug.Log("🐔 Chicken is dead!");
    }
}
