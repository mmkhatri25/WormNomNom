using Unity.Entities;

public struct AnimationData : IComponentData
{
    public Entity AnimatorEntity;
    public float LastHorizontal;
}