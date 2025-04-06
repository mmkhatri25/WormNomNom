using UnityEngine;

public class PlayerMover
{
    private readonly Transform _playerTransform;
    private readonly float _speed;
    private readonly Camera _mainCamera;
    private readonly float _playerWidth = 0.5f; // Half width of the player mesh

    public PlayerMover(Transform playerTransform, float speed = 3f)
    {
        _playerTransform = playerTransform;
        _speed = speed;
        _mainCamera = Camera.main;
    }

    public void Move(Vector3 direction)
    {
        if (direction == Vector3.zero) return;

        Vector3 newPos = _playerTransform.position + direction * _speed * Time.deltaTime;

        // Get world positions of screen left and right edges
        Vector3 leftEdge = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, _mainCamera.WorldToScreenPoint(_playerTransform.position).z));
        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, _mainCamera.WorldToScreenPoint(_playerTransform.position).z));

        // Clamp X between left and right edge with player width buffer
        newPos.x = Mathf.Clamp(newPos.x, leftEdge.x + _playerWidth, rightEdge.x - _playerWidth);

       _playerTransform.position = newPos;

        RotateTowardsDirection(direction);
    }

    private void RotateTowardsDirection(Vector3 direction)
    {
        float targetY = direction.x < 0 ? -145f : 145f;
        _playerTransform.rotation = Quaternion.Euler(0, targetY, 0);
    }
}
