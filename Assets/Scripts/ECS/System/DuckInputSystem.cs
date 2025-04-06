using Unity.Entities;
using UnityEngine;

public partial class DuckInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float move = 0f;

        // Check if the left mouse button is held down
        if (Input.GetMouseButton(0))
        {
            // Get the mouse position
            Vector3 mousePosition = Input.mousePosition;

            // Determine movement direction based on mouse position
            if (mousePosition.x < Screen.width / 2)
            {
                move = -1f; // Move left
            }
            else
            {
                move = 1f; // Move right
            }
        }

        // Update the PlayerInputData component with the movement value
        foreach (var input in SystemAPI.Query<RefRW<PlayerInputData>>())
        {
            input.ValueRW.Horizontal = move;
        }
    }
}
