using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private MouseInputHandler _inputHandler;
    private PlayerMover _mover;
    private PlayerAnimator _animator;
    private PlayerJumper _jumper;
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
    public Button ContinueButton;
    public Button NoThanksButton;

    private Coroutine _reviveCoroutine;
    private bool _isReviving = false;

    [Header("Gameover UI")]
    public GameObject GameOverPanel;

    private Rigidbody _rb;

    private void Awake()
    {
        _inputHandler = new MouseInputHandler();
        _rb = GetComponent<Rigidbody>();
        _mover = new PlayerMover(transform);
        _animator = new PlayerAnimator(GetComponent<Animator>());
        _jumper = new PlayerJumper(_rb, GetComponent<Animator>(), transform, groundLayer);
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

        if (_inputHandler.JumpRequested && _jumper.IsGrounded())
        {
            _audioPlayer?.PlaySound(jumpSound);
            _jumper.Jump(direction);
            _inputHandler.ResetJumpFlag();
        }

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
        ResetPowerSlideMeter();
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
        _animator.SetPuaseAnimator(0);

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
            CountdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        if (_isReviving)
        {
            OnNoThanksClicked();
        }
    }

    private void OnContinueClicked()
    {
        _animator.SetPuaseAnimator(1);

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

        Debug.Log("Game Over - implement scene change or restart here.");
        GameOverPanel.GetComponent<Animation>().Play("Game-Over-In");
    }
}
