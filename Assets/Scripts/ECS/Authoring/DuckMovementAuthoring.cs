using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public class DuckMovementAuthoring : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpForce = 7f;
    public float gravity = -9.8f;

    class Baker : Baker<DuckMovementAuthoring>
    {
        public override void Bake(DuckMovementAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovementData
            {
                Speed = 5f, // You can also expose this in DuckMovementAuthoring
                Velocity = float3.zero
            });
        }
    }
}