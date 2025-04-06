using UnityEngine;

public class PlayerAnimator
{
    private readonly Animator _animator;
    private static readonly int Walk = Animator.StringToHash("Walk");
    private static readonly int Fly = Animator.StringToHash("Fly");
    private static readonly int Idle = Animator.StringToHash("Idle_A");
    private static readonly int Eat = Animator.StringToHash("Attack");
    private static readonly int Death = Animator.StringToHash("Death");

    public PlayerAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void UpdateAnimation(bool isMoving, bool isEating)
    {
        if (isMoving)
        {
            _animator.Play(Fly);
            if (isEating)
                _animator.Play(Eat);
        }
        else
        {
            _animator.Play(Idle);
            if (isEating)
                _animator.Play(Eat);
        }
    }
    public void PlayDeath()
    {
        _animator.Play(Death); // Add this trigger to your Animator
    }
    public void SetPuaseAnimator(float speed)
    {
        _animator.speed = speed;
    }
}
