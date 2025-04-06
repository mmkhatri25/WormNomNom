using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class DuckAuthoring : MonoBehaviour
{
    public float moveSpeed = 5f;

    class Baker : Baker<DuckAuthoring>
    {
        public override void Bake(DuckAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MovementData
            {
                Speed = authoring.moveSpeed,
                Velocity = float3.zero
            });
        }
    }
}