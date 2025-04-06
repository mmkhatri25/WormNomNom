using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct DuckMovementSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerInputData>();
        state.RequireForUpdate<MovementData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (movement, transform, input) in SystemAPI.Query<RefRW<MovementData>, RefRW<LocalTransform>, RefRO<PlayerInputData>>())
        {
            float moveDir = input.ValueRO.Horizontal;
            float3 velocity = movement.ValueRW.Velocity;
            velocity.x = moveDir * movement.ValueRO.Speed;
            movement.ValueRW.Velocity = velocity;

            float3 newPos = transform.ValueRO.Position + velocity * deltaTime;
            transform.ValueRW.Position = newPos;
        }
    }
}