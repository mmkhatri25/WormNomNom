using UnityEngine;
using UnityEngine.EventSystems;

public class MouseInputHandler
{
    private Vector3 _direction = Vector3.zero;
    public bool JumpRequested { get; private set; }
    public bool PowerSlideRequested { get; private set; }
    public Vector3 JumpDirection { get; private set; }

    private bool _dragging = false;
    private bool _moved = false;
    private Vector3 _startPos;

    public void UpdateInput()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Start touch/tap
        if (Input.GetMouseButtonDown(0))
        {
            _startPos = Input.mousePosition;
            _dragging = true;
            _moved = false;
            JumpRequested = false;
            _direction = Vector3.zero;
        }

        // While dragging
        if (_dragging && Input.GetMouseButton(0))
        {
            Vector3 currentPos = Input.mousePosition;
            float deltaX = currentPos.x - _startPos.x;

            if (Mathf.Abs(deltaX) > 10f) // Adjust threshold if needed
            {
                _direction = deltaX > 0 ? Vector3.right : Vector3.left;
                _moved = true;
            }
            else
            {
                _direction = Vector3.zero;
            }
        }

        // On touch release
        if (_dragging && Input.GetMouseButtonUp(0))
        {
            _dragging = false;

            if (_moved)
            {
                // It was a swipe → move only
                JumpRequested = false;
                JumpDirection = Vector3.zero;
            }
            //else
            //{
            //    // It was a tap → jump
            //    JumpRequested = true;
            //    JumpDirection = Vector3.zero;
            //}

            // Reset direction after release
            _direction = Vector3.zero;
        }
    }

    public void TriggerPowerSlide() => PowerSlideRequested = true;
    public Vector3 GetDirection() => _direction;
    public bool IsMoving() => _direction != Vector3.zero;
    public void ResetJumpFlag() => JumpRequested = false;
    public void ResetPowerSlideFlag() => PowerSlideRequested = false;
}
