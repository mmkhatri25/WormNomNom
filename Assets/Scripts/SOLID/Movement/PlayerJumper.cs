using UnityEngine;
using DG.Tweening;
public class PlayerJumper
{
    private readonly Rigidbody _rigidbody;
    private readonly Animator _animator;
    private readonly Transform _transform;
    private readonly LayerMask _groundLayer;

    private bool _isJumping = false;
    private readonly float _groundCheckDistance = 0.2f;
    private readonly float _jumpForce = 4f;
    private readonly float _horizontalMoveForce = 5f;

    public PlayerJumper(Rigidbody rb, Animator animator, Transform transform, LayerMask groundLayer)
    {
        _rigidbody = rb;
        _animator = animator;
        _transform = transform;
        _groundLayer = groundLayer;
    }

    public bool IsGrounded()
    {
        Ray ray = new Ray(_transform.position, Vector3.down);
        if (Physics.Raycast(ray, _groundCheckDistance, _groundLayer))
        {
            _isJumping = false;
            return true;
        }
        return false;
    }

    public void Jump()
    {
        if (_isJumping) return;
        _isJumping = true;

        // Reset velocity to avoid stacking jump forces
        _rigidbody.linearVelocity = new Vector3(0, 0, 0);

        // Apply horizontal and vertical jump force
        Vector3 jumpVelocity = new Vector3(0, _jumpForce, 0);
        _rigidbody.AddForce(jumpVelocity, ForceMode.Impulse);
        _animator.Play("Jump");

    }
}
