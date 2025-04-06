using System.Linq;
using UnityEngine;

public class PowerSlideHandler
{
    private ChickenFatController _fatController;
    private AudioSource _audioSource;
    private AudioClip _powerSlideSound;
    private Animator _animator;


    public PowerSlideHandler(ChickenFatController fatController, AudioSource audioSource, AudioClip powerSlideSound, Animator animator)
    {
        _fatController = fatController;
        _audioSource = audioSource;
        _powerSlideSound = powerSlideSound;
        _animator = animator;
    }

    public void Execute()
    {
        // Play sound
        _audioSource.PlayOneShot(_powerSlideSound);

        // Trigger animation
        _animator.SetTrigger("PowerSlide");

        // Destroy all worms
        IWorm[] worms = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IWorm>().ToArray();
        foreach (var worm in worms)
        {
            worm.Eat();
        }

        // Reset fat
        _fatController.ResetFat();
    }
}
