using UnityEngine;
using UnityEngine.EventSystems;
public class MouseInputHandler
{
    private Vector3 _direction = Vector3.zero;

    public bool JumpRequested { get; private set; }
    public float TargetTapWorldY { get; private set; }

    public bool PowerSlideRequested { get; private set; }

    // You can customize this key to something else if needed (e.g., space, swipe gesture, etc.)
    private KeyCode _powerSlideKey = KeyCode.Space;

    public void UpdateInput()
    {
        // PowerSlide trigger (keyboard)
        //if (Input.GetKeyDown(_powerSlideKey))
        //{
        //    PowerSlideRequested = true;
        //}
        // Block input if clicking UI
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        // Mouse-based Jump Input
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Input.mousePosition;

            // Determine movement direction
            _direction = (mousePos.x < Screen.width / 2f) ? Vector3.left : Vector3.right;

            // Convert mouse Y to world Y
            var camera = Camera.main;
            Vector3 tapWorld = camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, camera.WorldToScreenPoint(Vector3.zero).z + 10f));
            TargetTapWorldY = tapWorld.y;

            JumpRequested = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _direction = Vector3.zero;
        }
    }
    public void TriggerPowerSlide() => PowerSlideRequested = true;

    public Vector3 GetDirection() => _direction;
    public bool IsMoving() => _direction != Vector3.zero;

    public void ResetJumpFlag() => JumpRequested = false;
    public void ResetPowerSlideFlag() => PowerSlideRequested = false;
}
