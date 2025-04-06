using UnityEngine;

public class PlayerJumper
{
    private readonly Rigidbody _rigidbody;
    private readonly Animator _animator;
    private readonly Transform _playerTransform;
    private readonly Camera _camera;

    private readonly float _jumpPower = 10f;

    public PlayerJumper(Rigidbody rb, Animator animator, Transform player)
    {
        _rigidbody = rb;
        _animator = animator;
        _playerTransform = player;
        _camera = Camera.main;
    }

    public void JumpToHeight(float targetWorldY)
    {
        float currentY = _playerTransform.position.y;

        // Check if player is already above screen's half
        float screenHalfY = _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, _camera.WorldToScreenPoint(_playerTransform.position).z)).y;
        if (currentY >= screenHalfY) return;

        // If tap is above current Y
        if (targetWorldY > currentY)
        {
            float heightDifference = targetWorldY - currentY;

            // Reset Y velocity before applying new one
            Vector3 velocity = _rigidbody.linearVelocity;
            velocity.y = 0;
            _rigidbody.linearVelocity = velocity;

            // Use physics impulse to reach target (rough estimation)
            float jumpForce = Mathf.Sqrt(2 * 9.81f * heightDifference) * _rigidbody.mass;
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
