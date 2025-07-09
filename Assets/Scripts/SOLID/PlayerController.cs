using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
//using UnityEngine.UIElements;
using TMPro;
using UnityEngine.UIElements;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private MouseInputHandler _inputHandler;
    private PlayerMover _mover;
    private PlayerEater _eater;
    private PlayerAnimator _animator;
    [SerializeField] private PlayerJumper _jumper;
    private IAudioPlayer _audioPlayer;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wormLayer;
    [SerializeField] private JumpSettings jumpSettings;

    public UnityEngine.UI.Image PowerSlider;
    public UnityEngine.UI.Button PowerButton;
    public List<GameObject> healths;

    public AudioClip jumpSound, eatSound, powerSound, chickenPain;

    [Header("Health System")]
    public GameObject leftEye;
    public GameObject rightEye;
    public GameObject Mouth;
    private GameObject _deathParticle;

    public ParticleSystem fatIncreasePaticle;
    public ParticleSystem fartParticle;



    public float _currentHealth;
    public float _maxHealth;
    private float _currentFatness = 1f;
    private readonly float _maxFatness = 1.5f;
    private readonly float _fatnessStep = 0.01f;// original

    private float _powerSlideMeter = 0f;
    private readonly float _powerSlideFillPerWorm = 0.2f;
    private readonly float _powerSlideFullThreshold = 1f;

    [Header("Revive UI")]
    public GameObject RevivePanel;
    public Text CountdownText;
    public UnityEngine.UI.Slider ReviveSlider;
    public UnityEngine.UI.Button ContinueButton;
    public UnityEngine.UI.Button NoThanksButton;

    private Coroutine _reviveCoroutine;
    private bool _isReviving = false;
    public bool isGrounded;

    [Header("Gameover UI")]
    public GameObject GameOverPanel;

    private Rigidbody _rb;

    public bool isPause;

    public static Action onReviveGame;


    [Header("Characters")]
    public List<GameObject> Characters;
    public GameObject ActiveCharacter;

    public TextMeshProUGUI scoreText, highScoreText, currentCoins;
    public int current_Score, current_Coins;
    
    

    private void OnEnable()
    {
       
        onReviveGame += ResetGameOnRevive;
        doubleCoinVideoButton.SetActive(true);

        //fatIncreasePaticle = fatParticle.GetComponent<ParticleSystem>();
    }
    private void OnDisable()
    {
        onReviveGame -= ResetGameOnRevive;

    }
    private void Start()
    {
        highScoreText.text = "TOP :" + PlayerPrefs.GetInt("topscore");
    }

    private void Awake()
    {
        foreach (var item in Characters)
        {
            item.SetActive(false);
        }
        Debug.Log("CurrentCharacter " + PlayerPrefs.GetInt("CurrentCharacter"));

        switch (PlayerPrefs.GetInt("CurrentCharacter"))
        {
            case 0:
                ActiveCharacter = Characters[0];
                ActiveCharacter.SetActive(true);
                break;
            case 1:
                ActiveCharacter = Characters[1];
                ActiveCharacter.SetActive(true);
                break;
            case 2:
                ActiveCharacter = Characters[2];
                ActiveCharacter.SetActive(true);
                break;
            default:
                ActiveCharacter = Characters[0];
                ActiveCharacter.SetActive(true);
                break;
        }


        _inputHandler = new MouseInputHandler();
        _rb = GetComponent<Rigidbody>();
        _mover = new PlayerMover(transform);
        _animator = new PlayerAnimator(ActiveCharacter.gameObject.GetComponent<Animator>());
        _jumper = new PlayerJumper(_rb, ActiveCharacter.GetComponent<Animator>(), transform, groundLayer);
        _eater = new PlayerEater(transform, ActiveCharacter.GetComponent<Animator>());

        isGrounded = _jumper.IsGrounded();
        _audioPlayer = GetComponent<IAudioPlayer>();
        PowerButton.interactable = false;
        StartCoroutine(RotateImage(false));

        leftEye = GetChildGameObjectWithTag(ActiveCharacter.transform, "Eye1");
        rightEye = GetChildGameObjectWithTag(ActiveCharacter.transform, "Eye2");
        Mouth  = GetChildGameObjectWithTag(ActiveCharacter.transform, "Mouth");

        _deathParticle = ActiveCharacter.transform.GetChild(2).gameObject;
    }
    GameObject GetChildGameObjectWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child.gameObject;
            }

            GameObject result = GetChildGameObjectWithTag(child, tag); // recurse
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }


    private void Update()
    {
        _inputHandler.UpdateInput();

        Vector3 direction = _inputHandler.GetDirection();
        bool isMoving = _inputHandler.IsMoving();

        _mover.Move(direction);
        _animator.UpdateAnimation(isMoving, false);

        if (PowerSlider.fillAmount >= _powerSlideFullThreshold)
        {
            PowerButton.interactable = true;
            StartCoroutine(RotateImage(true));

        }

        if (_inputHandler.PowerSlideRequested)
        {
            StartCoroutine(ActivatePowerSlide());
            _inputHandler.ResetPowerSlideFlag();
        }
    }
    public bool isVainActive;
    private HashSet<Collider> _eatenWorms = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        // 🟢 Skip if already processed this worm
        if (_eatenWorms.Contains(other))
            return;

        bool isMoving = _inputHandler.IsMoving();

        IWorm worm = other.GetComponent<IWorm>();
        if (worm != null)
        {
            _audioPlayer?.PlaySound(eatSound);
            _eatenWorms.Add(other); // 🟢 Mark worm as eaten

            worm.Eat(Mouth);
            _audioPlayer?.PlaySound(eatSound);
            _animator.UpdateAnimation(isMoving, true);

            if (worm is PoisonousWorm)
            {
                _currentHealth--;
                UpdateHealth(_currentHealth);
                _eater.PlayEatAnimation();
                //_playerHealth.EatPoisonousWorm();
                _audioPlayer?.PlaySound(chickenPain);
            }
            else if (worm is Vain)
            {
                if (isVainActive == false)
                {
                    if (_currentFatness > 1)
                        ResetHalfFatness();
                }
            }
            else
            {
                _eater.PlayEatAnimation();
                GrowFat();
                FillPowerSlide();
                AddScore(1);
            }

            // 🟢 Optional: destroy worm GameObject to remove from scene
            Destroy(other.gameObject);
        }
    }

    public void UpdateHealth(float currentHealth)
    {
        Debug.Log("UpdateHealth on " + currentHealth);
        foreach (var item in healths)
        {
            item.SetActive(false);
        }
        for (int i = 0; i < currentHealth; i++)
        {
            healths[i].SetActive(true);
        }

        // Left eye logic: enable once at health == 2
        if (currentHealth == 2 && !leftEye.activeSelf)
        {
            leftEye.SetActive(true);
        }

        // Right eye logic: enable once at health == 1
        if (currentHealth == 1 && !rightEye.activeSelf)
        {
            rightEye.SetActive(true);
        }
        if (_currentHealth <= 0)
        {
            ShowDeath();
            _currentHealth = 5f;
        }
    }

    private void GrowFat()
    {
        if (_currentFatness < _maxFatness)
        {
            fatIncreasePaticle.Play();
            //transform.localScale = Vector3.one * _currentFatness;
            _currentFatness += _fatnessStep;
            Vector3 scale = transform.localScale;
            scale.x = _currentFatness;
            transform.localScale = scale;
        }
    }

    private void ResetFatness()
    {


        _currentFatness = 1f;
        transform.localScale = Vector3.one;

        // Shrink to 0.7, then grow back to 1
        transform.DOScale(Vector3.one * 0.7f, 0.3f)
                 .SetEase(Ease.InOutQuad)
                 .OnUpdate(() => {
                     _currentFatness = transform.localScale.x;
                 })
                 .OnComplete(() => {
                     // Scale back to original
                     transform.DOScale(Vector3.one, 0.3f)
                              .SetEase(Ease.InOutQuad)
                              .OnUpdate(() => {
                                  _currentFatness = transform.localScale.x;
                              });
                 });



        ResetPowerSlideMeter();
        leftEye.SetActive(false);
        rightEye.SetActive(false);

    }
    private void ResetHalfFatness()
    {
        //if (_currentFatness > 1f)
        //{
            float extraFat = _currentFatness - 1f;                         // Extra fatness above the base
            float reduction = extraFat * 0.5f;                             // 50% of the extra
            _currentFatness -= reduction;
             Vector3 scale = transform.localScale;
             scale.x = _currentFatness;
        // Reduce fatness
           transform.localScale = scale;         // Apply new scale

            // Assuming power fill amount increases with fatness linearly
            PowerSlider.fillAmount -= reduction;                          // Reduce fill by same extra amount
            PowerSlider.fillAmount = Mathf.Clamp01(PowerSlider.fillAmount); // Clamp between 0 and 1
        //}
        PowerButton.interactable = false;
        Debug.Log("_currentFatness on vain  = " + _currentFatness + " , transform.localScale - "+ transform.localScale);
        StartCoroutine(ResetVainTrigger());
    }
    IEnumerator ResetVainTrigger()
    {
        yield return new WaitForSeconds(2f);
        isVainActive = false;

    }

    private void FillPowerSlide()
    {
        //PowerSlider.fillAmount += _powerSlideFillPerWorm;
        PowerSlider.fillAmount += _fatnessStep / 0.5f;
    }

    private void ResetPowerSlideMeter()
    {
        _powerSlideMeter = 0f;
        PowerSlider.fillAmount = _powerSlideMeter;
        PowerButton.interactable = false;
        StartCoroutine(RotateImage(false));
    }
    float rotationDuration = 1f;
    private IEnumerator RotateImage(bool active)
    {
        float elapsed = 0f;
        float startRotation = PowerSlider.rectTransform.eulerAngles.z;
        float endRotation = startRotation + 360f;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, elapsed / rotationDuration);
            PowerSlider.rectTransform.rotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }

        PowerSlider.rectTransform.rotation = Quaternion.Euler(0, 0, endRotation);
    }

    public List<GameObject> allWorms;
    IEnumerator ActivatePowerSlide()
    {
        Vector3 rotation = PowerSlider.transform.localEulerAngles;
        rotation.z = 0f;
        PowerSlider.transform.localEulerAngles = rotation;
        PowerButton.interactable = false;
        PowerSlider.fillAmount = 0;
        isPause = true;
        yield return new WaitForSeconds(.3f);

        Vector3 currentRotation = transform.eulerAngles;
        Vector3 targetRotation = new Vector3(30f, currentRotation.y, currentRotation.z);

        transform.DORotate(targetRotation, 0.5f)
          .SetEase(Ease.OutQuad)
          .OnComplete(() =>
          {
              
              fartParticle.Play();
              _audioPlayer?.PlaySound(powerSound);
              ResetFatness();
              transform.DOScaleY(0.9f, 0.025f)
                .SetLoops(20, LoopType.Yoyo) 
                .SetEase(Ease.InOutSine);

          });
        yield return new WaitForSeconds(.4f);

        var normalWorms = GameObject.FindGameObjectsWithTag("Worm");
        var poisonWorms = GameObject.FindGameObjectsWithTag("PoisonousWorm");
        var vain = GameObject.FindGameObjectsWithTag("Vain");

        allWorms = new List<GameObject>();
        allWorms.AddRange(normalWorms);
        allWorms.AddRange(poisonWorms);
        allWorms.AddRange(vain);

        foreach (var worm in allWorms)
        {
            IWorm w = worm.GetComponent<IWorm>();
            w?.Blast();
        }
        yield return new WaitForSeconds(1f);

        targetRotation = new Vector3(0f, currentRotation.y, currentRotation.z);

        transform.DORotate(targetRotation, 0.5f)
                 .SetEase(Ease.OutQuad);
        isPause = false;
    }

    public void OnPowerSlideButtonPressed()
    {
        _inputHandler.TriggerPowerSlide();
    }

    private void ShowDeath()
    {
        Debug.Log("💀 Chicken Died");
        ShowRevivePopup();
    }

    private void ShowRevivePopup()
    {
        isPause = true;
        RevivePanel.GetComponent<Animation>().Play("Window-In");
        ContinueButton.onClick.AddListener(OnContinueClicked);
        NoThanksButton.onClick.AddListener(OnNoThanksClicked);
        _reviveCoroutine = StartCoroutine(ReviveCountdown());
    }

    private IEnumerator ReviveCountdown()
    {
        _isReviving = true;
        int countdown = 5;

        while (countdown > 0)
        {
            // Update the countdown text.
            CountdownText.text = countdown.ToString();

            // Calculate the starting and ending slider values for this interval.
            float startValue = (float)countdown / 5f;
            float endValue = (float)(countdown - 1) / 5f;
            float elapsed = 0f;
            float duration = 1f; // Duration for each countdown step.

            // Smoothly interpolate the slider from startValue to endValue over "duration".
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                ReviveSlider.value = Mathf.Lerp(startValue, endValue, elapsed / duration);
                yield return null; // Wait for the next frame.
            }

            countdown--;
        }

        // Ensure that the slider and text update to their final state.
        CountdownText.text = "0";
        ReviveSlider.value = 0f;

        if (_isReviving)
        {
            OnNoThanksClicked();
            AnalyticsManager.Instance.SendEvent_PlayerDie();
        }
    }


    private void OnContinueClicked()
    {
        AnalyticsManager.Instance.SendEvent_PlayerRevive();
        foreach (var item in healths)
        {
            item.SetActive(true);
        }

        GoogleMobileAdsScript.Instance.ShowRewardBasedVideoForCoin();
    }

    public GameObject doubleCoinVideoButton;
    public void OnDoubleCoinVideoAds()
    {
        //AnalyticsManager.Instance.SendEvent_PlayerRevive();
        

        GoogleMobileAdsScript.Instance.ShowRewardBasedVideoForDoubleCoin(current_Coins);
        doubleCoinVideoButton.SetActive(false);
    }

    private void ResetGameOnRevive()
    {
        ResetFatness();

        _animator.SetPuaseAnimator(1);
        isPause = false;

        _isReviving = false;
        if (_reviveCoroutine != null) StopCoroutine(_reviveCoroutine);

        RevivePanel.GetComponent<Animation>().Play("Window-Out");

        ContinueButton.onClick.RemoveListener(OnContinueClicked);
        NoThanksButton.onClick.RemoveListener(OnNoThanksClicked);
        leftEye.gameObject.SetActive(false);
        rightEye.gameObject.SetActive(false);
        _animator.UpdateAnimation(false, false); // Idle again
    }
    private void OnNoThanksClicked()
    {

        _animator.PlayDeath(); // Make sure Animator has "Die" trigger

        _isReviving = false;
        if (_reviveCoroutine != null) StopCoroutine(_reviveCoroutine);

        RevivePanel.GetComponent<Animation>().Play("Window-Out");

        ContinueButton.onClick.RemoveListener(OnContinueClicked);
        NoThanksButton.onClick.RemoveListener(OnNoThanksClicked);

        GameOver();
    }

    private void GameOver()
    {
        if (current_Score > PlayerPrefs.GetInt("topscore"))
        {
            PlayerPrefs.SetInt("topscore", current_Score);
            highScoreText.text = "TOP :" + current_Score;
        }
        current_Coins = current_Score / 2;
        currentCoins.text = current_Coins.ToString();
        Wallet.SetAmount(Wallet.GetAmount() + current_Coins);
        Wallet.DisplayAmount();
        _deathParticle.SetActive(true);
        _animator.PlayDeath(); // Make sure Animator has "Die" trigger
        StartCoroutine(WaitForShowGameover());
        Debug.Log("Game Over - implement scene change or restart here.");
    }
    IEnumerator WaitForShowGameover()
    {

        yield return new WaitForSeconds(1f);

        GameOverPanel.GetComponent<Animation>().Play("Game-Over-In");

    }
    public void OnJump()
    {
        _audioPlayer?.PlaySound(jumpSound);
        _jumper.Jump();
        _inputHandler.ResetJumpFlag();
    }

    public void AddScore(int amount)
    {
        current_Score += amount;
        scoreText.text = "" + current_Score;

        int topScore = PlayerPrefs.GetInt("topscore", 0);
        if (current_Score > topScore)
        {
            PlayerPrefs.SetInt("topscore", current_Score);
            //highScoreText.text = "TOP :" + current_Score;
        }
        else
        {
            //highScoreText.text = "" + topScore;
        }
    }
}
