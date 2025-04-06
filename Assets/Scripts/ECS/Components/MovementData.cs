using Unity.Entities;
using Unity.Mathematics;

public struct MovementData : IComponentData
{
    public float3 Velocity;
    public float Speed;
}
