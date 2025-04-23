using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private MouseInputHandler _inputHandler;
    private PlayerMover _mover;
    private PlayerAnimator _animator;
    [SerializeField] private PlayerJumper _jumper;
    private PowerSlideUI _powerSlideUI;
    private IAudioPlayer _audioPlayer;

    [SerializeField] private AnimationCurve jumpCurve;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wormLayer;
    [SerializeField] private JumpSettings jumpSettings;
    public Slider PowerSlider;
    public Button PowerButton;
    public AudioClip jumpSound, eatSound, powerSound;

    [Header("Health System")]
    public GameObject leftEye;
    public GameObject rightEye;
    public Slider HealthSlider;

    private PlayerHealth _playerHealth;
    private PlayerUI _playerUI;

    private float _currentFatness = 1f;
    private readonly float _maxFatness = 2.5f;
    private readonly float _fatnessStep = 0.1f;

    private float _powerSlideMeter = 0f;
    private readonly float _powerSlideFillPerWorm = 0.2f;
    private readonly float _powerSlideFullThreshold = 1f;

    [Header("Revive UI")]
    public GameObject RevivePanel;
    public Text CountdownText;
    public Slider ReviveSlider;
    public Button ContinueButton;
    public Button NoThanksButton;

    private Coroutine _reviveCoroutine;
    private bool _isReviving = false;
    public bool isGrounded;

    [Header("Gameover UI")]
    public GameObject GameOverPanel;

    private Rigidbody _rb;

    public bool isPause;

    public static Action onReviveGame;
    private void OnEnable()
    {
        onReviveGame += ResetGameOnRevive;   
    }
    private void OnDisable()
    {
        onReviveGame -= ResetGameOnRevive;

    }

    private void Awake()
    {
        _inputHandler = new MouseInputHandler();
        _rb = GetComponent<Rigidbody>();
        _mover = new PlayerMover(transform);
        _animator = new PlayerAnimator(this.transform.GetChild(0).gameObject.GetComponent<Animator>());
        _jumper = new PlayerJumper(_rb, transform.GetChild(0).GetComponent<Animator>(), transform, groundLayer);
        isGrounded = _jumper.IsGrounded();
        _powerSlideUI = new PowerSlideUI();
        _audioPlayer = GetComponent<IAudioPlayer>();
        PowerButton.gameObject.SetActive(false);

        _playerHealth = new PlayerHealth();
        _playerUI = new PlayerUI(HealthSlider, leftEye, rightEye, _playerHealth.Max);

        _playerHealth.OnHealthChanged += _playerUI.UpdateHealth;
        _playerHealth.OnDeath += ShowDeath;
    }

    private void Update()
    {
        _inputHandler.UpdateInput();

        Vector3 direction = _inputHandler.GetDirection();
        bool isMoving = _inputHandler.IsMoving();

        _mover.Move(direction);
        _animator.UpdateAnimation(isMoving, false);

        //if (_inputHandler.JumpRequested && _jumper.IsGrounded())
        //{
        //    _audioPlayer?.PlaySound(jumpSound);
        //    _jumper.Jump(direction);
        //    _inputHandler.ResetJumpFlag();
        //}

        if (_powerSlideMeter >= _powerSlideFullThreshold)
        {
            PowerButton.gameObject.SetActive(true);
        }

        if (_inputHandler.PowerSlideRequested)
        {
            StartCoroutine( ActivatePowerSlide());
            _inputHandler.ResetPowerSlideFlag();
        }
    }
    public bool isVainActive;
    private void OnTriggerEnter(Collider other)
    {
        bool isMoving = _inputHandler.IsMoving();

        IWorm worm = other.GetComponent<IWorm>();
        if (worm != null)
        {
            worm.Eat();
            _audioPlayer?.PlaySound(eatSound);
            _animator.UpdateAnimation(isMoving, true);
            if (worm is PoisonousWorm)
            {
                _playerHealth.EatPoisonousWorm();
            }
            else if (worm is Vain)
            {
                if (isVainActive == false)
                {
                    if(_currentFatness>1)
                    ResetHalfFatness();

                }


            }
            else
            {
                GrowFat();
                FillPowerSlide();
            }
        }
    }

    private void GrowFat()
    {
        if (_currentFatness < _maxFatness)
        {
            _currentFatness += _fatnessStep;
            transform.localScale = Vector3.one * _currentFatness;
        }
    }

    private void ResetFatness()
    {
        _currentFatness = 1f;
        transform.localScale = Vector3.one;
        ResetPowerSlideMeter();

    }
    private void ResetHalfFatness()
    {
        float extraFat = _currentFatness - 1f;         // Extra fatness above the base
        extraFat /= 2f;                                // Reduce the extra by half
        _currentFatness = 1f + extraFat;               // New fatness
        transform.localScale = Vector3.one * _currentFatness; // Apply new scale
        _powerSlideMeter = _powerSlideUI.CurrentPoweSliderValue(PowerSlider)*0.5f;
        _powerSlideUI?.UpdatePowerSlider(_powerSlideMeter, PowerSlider);
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
        _powerSlideMeter += _powerSlideFillPerWorm;
        _powerSlideMeter = Mathf.Clamp01(_powerSlideMeter);
        _powerSlideUI?.UpdatePowerSlider(_powerSlideMeter, PowerSlider);
    }

    private void ResetPowerSlideMeter()
    {
        _powerSlideMeter = 0f;
        _powerSlideUI?.UpdatePowerSlider(_powerSlideMeter, PowerSlider);
        PowerButton.gameObject.SetActive(false);
    }

    IEnumerator ActivatePowerSlide()
    {
        _audioPlayer?.PlaySound(powerSound);
        yield return new WaitForSeconds(2f);
        var normalWorms = GameObject.FindGameObjectsWithTag("Worm");
        var poisonWorms = GameObject.FindGameObjectsWithTag("PoisonousWorm");

        List<GameObject> allWorms = new List<GameObject>();
        allWorms.AddRange(normalWorms);
        allWorms.AddRange(poisonWorms);

        foreach (var worm in allWorms)
        {
            IWorm w = worm.GetComponent<IWorm>();
            w?.Eat();
        }

        ResetFatness();
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
       // _animator.SetPuaseAnimator(0);

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

        GoogleMobileAdsScript.Instance.ShowRewardBasedVideoForCoin();
    }

    private void ResetGameOnRevive()
    {
        _animator.SetPuaseAnimator(1);
        isPause = false;

        _isReviving = false;
        if (_reviveCoroutine != null) StopCoroutine(_reviveCoroutine);

        RevivePanel.GetComponent<Animation>().Play("Window-Out");

        ContinueButton.onClick.RemoveListener(OnContinueClicked);
        NoThanksButton.onClick.RemoveListener(OnNoThanksClicked);

        _playerHealth.Reset();
        _playerUI.UpdateHealth(_playerHealth.Max);
        ResetFatness();
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
        //_jumper = new PlayerJumper(_rb, GetComponent<Animator>(), transform, groundLayer);

        //if (_jumper != null)
        //{
        Debug.Log("_jumper 111 " +_jumper.IsGrounded());

            if (_jumper.IsGrounded())
            {

                _audioPlayer?.PlaySound(jumpSound);
                _jumper.Jump();
                _inputHandler.ResetJumpFlag();
            }
        //}
        //else
        //{
        //    _jumper = new PlayerJumper(_rb, GetComponent<Animator>(), transform, groundLayer);
        //    if (_jumper.IsGrounded())
        //    {
        //        _audioPlayer?.PlaySound(jumpSound);
        //        _jumper.Jump();
        //        _inputHandler.ResetJumpFlag();
        //    }
        //}
       
    }
}
