using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class PlayerUI
{
    private Slider _healthSlider;
    private List<GameObject> _healths;
    private GameObject _leftEye;
    private GameObject _rightEye;
    private int _maxHealth;
    private MonoBehaviour _coroutineRunner;  // <== This is new

    private Coroutine _leftEyeCoroutine;
    private Coroutine _rightEyeCoroutine;
    

    public PlayerUI(Slider slider, GameObject leftEye, GameObject rightEye, int maxHealth, MonoBehaviour coroutineRunner, List <GameObject> healths)
    {
        _healthSlider = slider;
        _leftEye = leftEye;
        _rightEye = rightEye;
        _maxHealth = maxHealth;
        _coroutineRunner = coroutineRunner;

        _healthSlider.maxValue = maxHealth;
        _healths = healths;
        foreach (var item in healths)
        {
            item.SetActive(true);
        }
     
    }

    public void UpdateHealth(int currentHealth)
    {
        Debug.Log("UpdateHealth on "+ currentHealth);
        _healthSlider.value = currentHealth;
        foreach (var item in _healths)
        {
            item.SetActive(false);
        }
        for (int i = 0; i <= currentHealth; i++)
        {
            _healths[i].SetActive(true);
        }

        // Left eye logic: enable once at health == 2
        if (currentHealth == 2 && !_leftEye.activeSelf)
        {
            _leftEye.SetActive(true);
            //_leftEye.transform.DOPunchScale(new Vector3(1.010707f, 1.010707f, 1.010707f), 0.2f);

        }

        // Right eye logic: enable once at health == 1
        if (currentHealth == 1 && !_rightEye.activeSelf)
        {
            _rightEye.SetActive(true);
           // _rightEye.transform.DOPunchScale(new Vector3(0.010707f, 0.010707f, 0.010707f), 0.5f);
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
