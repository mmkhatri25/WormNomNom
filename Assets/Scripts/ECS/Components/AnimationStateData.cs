using Unity.Entities;

public enum DuckAnimationState
{
    Idle,
    Flap,
    Hurt,
    Dead
}

public struct AnimationStateData : IComponentData
{
    public DuckAnimationState currentState;
}
