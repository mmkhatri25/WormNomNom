using Unity.Entities;

public struct PlayerInputData : IComponentData
{
    public float Horizontal; // -1 (left), 1 (right)
}