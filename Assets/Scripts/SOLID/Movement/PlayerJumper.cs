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
    private readonly float _jumpForce = 3f;

    // -----------------------------
    // NEW: Jump count + jump reset timer
    // -----------------------------
    private int jumpcount = 0;
    private float jumpResetDuration = 1f;  // time to reset jumps
    private float jumpResetTimer = 0f;
    // -----------------------------

    public PlayerJumper(Rigidbody rb, Animator animator, Transform transform, LayerMask groundLayer)
    {
        Debug.Log("Jumper name " + transform.gameObject.name);
        _rigidbody = rb;
        _animator = animator;
        _transform = transform;
        _groundLayer = groundLayer;
    }

    // Call this every Update() from your PlayerController
    public void Tick()
    {
        if (jumpcount > 0)
        {
            jumpResetTimer += Time.deltaTime;

            if (jumpResetTimer >= jumpResetDuration)
            {
                jumpcount = 0;
                jumpResetTimer = 0;
                Debug.Log("Jump count reset");
            }
        }
    }

    public bool IsGrounded()
    {
        Vector3 rayOrigin = _transform.position + Vector3.down * 1.0f;
        float rayDistance = _groundCheckDistance;
        Ray ray = new Ray(rayOrigin, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.blue);

        return Physics.Raycast(ray, rayDistance, _groundLayer);
    }

    public void Jump()
    {
        Debug.Log("_isJumping = " + _isJumping);

        // -----------------------------
        // NEW: Block jump if already 2 jumps used
        // -----------------------------
        if (jumpcount >= 2)
        {
            Debug.Log("Jump blocked — jumpcount = " + jumpcount);
            return;
        }
        // -----------------------------

        if (_isJumping) return;

        float screenMidY = Camera.main.ScreenToWorldPoint(
            new Vector3(0, Screen.height / 2f, Camera.main.nearClipPlane)
        ).y;

        if (_transform.position.y > screenMidY)
        {
            Debug.Log("Player too high to jump");
            return;
        }

        _isJumping = true;

        _rigidbody.linearVelocity = Vector3.zero;

        Vector3 jumpVelocity = new Vector3(0, _jumpForce, 0);
        _rigidbody.AddForce(jumpVelocity, ForceMode.Impulse);

        _animator.SetTrigger("Jump");

        _isJumping = false;

        // -----------------------------
        // NEW: Increase jump count + restart timer
        // -----------------------------
        jumpcount++;
        jumpResetTimer = 0f;

        Debug.Log("Jump performed, count = " + jumpcount);
        // -----------------------------
    }
}
