using System.Collections;
using DG.Tweening;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Vain : MonoBehaviour, IWorm
{
    [SerializeField] private Animator _animator;
    public AudioSource _audioPlayer;

    public bool IsPoisonous => throw new System.NotImplementedException();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Eat(GameObject warm)
    {
        // Play the destroy animation
        if (_animator != null)
        {
            _animator.Play("Death");
            _audioPlayer.Play();
        }
        // Delay returning to the pool until the animation is done
        StartCoroutine(ReturnAfterDelay(0.5f)); // adjust time to match animation
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (blastEffect != null)
            blastEffect.SetActive(false);

        WormPool.Instance.ReturnWorm(gameObject);
    }
    public Transform GetTransform()
    {
        return transform;
    }

    public GameObject blastEffect;
    public void Blast()
    {
        if (blastEffect != null)
        {

            blastEffect.SetActive(true);

            Sequence squash = DOTween.Sequence();
            squash.Append(transform.DOScale(new Vector3(0.25f, 0.15f, 1), 0.15f));
            squash.Append(transform.DOScale(new Vector3(0.1f, 0.1f, 1), 0.35f));
            _audioPlayer.Play();

            StartCoroutine(ReturnAfterDelay(1.1f));
        }
    }
}
