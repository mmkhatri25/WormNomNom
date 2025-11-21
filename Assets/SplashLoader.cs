using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SplashLoader : MonoBehaviour
{
    [SerializeField] private Slider loadingBar;
    [SerializeField] private GameObject splashScreen;
    [SerializeField] private TextMeshProUGUI percentageText; // For showing %

    [SerializeField] private float fillSpeed = 0.5f;
    private float targetProgress = 1f;

    private void Start()
    {
        loadingBar.gameObject.SetActive(false);
        loadingBar.value = 0f;
        percentageText.text = "0%";
        StartCoroutine(FillBar());
        if (!PlayerPrefs.HasKey("WalletInitialized"))
        {
            PlayerPrefs.SetInt("WalletAmount", 5000);
            PlayerPrefs.SetInt("WalletInitialized", 1);
            PlayerPrefs.Save();
        }
    }

    private IEnumerator FillBar()
    {
        yield return new WaitForSeconds(1.2f);
        loadingBar.gameObject.SetActive(true);

        while (loadingBar.value < targetProgress)
        {
            loadingBar.value += Time.deltaTime * fillSpeed;
            percentageText.text = Mathf.RoundToInt(loadingBar.value * 100f) + "%";
            yield return null;
        }

        percentageText.text = "100%";
        yield return new WaitForSeconds(1f);
        splashScreen.SetActive(false);
    }
}
