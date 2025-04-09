using System.Collections;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class Worm : MonoBehaviour, IWorm
{
    private Animator _animator;
    private IAudioPlayer _audioPlayer;

    [SerializeField] private AudioClip deathSound;

    public bool IsPoisonous => throw new System.NotImplementedException();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioPlayer = GetComponent<IAudioPlayer>();
    }

    public void Eat()
    {
        // Play the destroy animation
        if (_animator != null)
        {
            _animator.Play("Death");
            _audioPlayer?.PlaySound(deathSound);
        }
        // Delay returning to the pool until the animation is done
        StartCoroutine(ReturnAfterDelay(0.5f)); // adjust time to match animation
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        WormPool.Instance.ReturnWorm(gameObject);
    }
    public Transform GetTransform()
    {
        return transform;
    }
}
