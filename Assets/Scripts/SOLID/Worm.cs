using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Worm : MonoBehaviour, IWorm
{
    private Animator _animator;
    private IAudioPlayer _audioPlayer;

    [SerializeField] private AudioClip deathSound;

    private Tween idleTween; // Tween reference for idle animation

    public bool IsPoisonous => throw new System.NotImplementedException();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioPlayer = GetComponent<IAudioPlayer>();

        
    }

    private void OnEnable()
    {
        StartShiverEffect();
       // spinCoroutine = StartCoroutine(RandomSpinRoutine());
    }

    private void OnDisable()
    {
        StopShiverEffect();
    }
    private Tween shiverTween;
    private Coroutine spinCoroutine;
    private void StartShiverEffect()
    {
        // Ensure starting at correct base rotation
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        // Small rotation back and forth on Z axis to simulate shiver
        shiverTween = transform.DORotate(new Vector3(0f, 180f, 5f), 0.1f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopShiverEffect()
    {
        if (shiverTween != null && shiverTween.IsActive())
        {
            shiverTween.Kill();
            transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Reset rotation
        }
    }

    private IEnumerator RandomSpinRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(waitTime);

            // Do a full Z-axis spin while keeping Y=180
            transform.DORotate(new Vector3(0f, 180f, 360f), 0.4f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutSine);
        }
    }
    private void PlayIdleEffect()
    {
        idleTween?.Kill();

        Vector3 startPos = transform.position;

        // Do NOT change rotation here, keep worm's initial rotation (180 Y) intact

        idleTween = DOTween.Sequence()
            // Subtle vertical bobbing only, no horizontal move
            .Append(transform.DOMoveY(startPos.y + 0.005f, 0.5f).SetEase(Ease.InOutSine))
            .Append(transform.DOMoveY(startPos.y - 0.005f, 1f).SetEase(Ease.InOutSine))
            .Append(transform.DOMoveY(startPos.y, 0.5f).SetEase(Ease.InOutSine))
            .SetLoops(-1, LoopType.Restart);
    }




    public void Eat(GameObject mouth)
    {
        // Stop the idle tween smoothly before eating animation starts
        idleTween?.Kill();

        if (_animator != null)
        {
            _animator.Play("Death");
            _audioPlayer?.PlaySound(deathSound);

            // Start moving with effects
            StartCoroutine(FlyIntoMouth(mouth, 0.5f));

            // Fun spin while moving
            transform.DORotate(new Vector3(0, 0, 1080f), 0.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);

            // Stretch effect (squash and stretch)
            Sequence squash = DOTween.Sequence();
            squash.Append(transform.DOScale(new Vector3(0.25f, 0.15f, 1), 0.15f));
            squash.Append(transform.DOScale(new Vector3(0.1f, 0.1f, 1), 0.35f));
        }
    }

    private IEnumerator FlyIntoMouth(GameObject mouth, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Wiggly motion
            float wiggle = Mathf.Sin(t * 10f * Mathf.PI) * 0.1f;
            Vector3 targetPos = Vector3.Lerp(startPos, mouth.transform.position, t);
            targetPos.x += wiggle;

            transform.position = targetPos;

            yield return null;
        }

        // Optionally play pop/gulp particle or sound here

        StartCoroutine(ReturnAfterDelay(0.1f));
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        WormPool.Instance.ReturnWorm(gameObject);

        // Reset scale and rotation after returning to pool, ready for reuse
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;

        // Restart idle effect after returning to pool (if reused immediately)
        PlayIdleEffect();
    }

    public Transform GetTransform()
    {
        return transform;
    }
}
