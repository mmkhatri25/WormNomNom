using Unity.Entities;

public struct PowerSlideData : IComponentData
{
    public float currentPower;
    public float maxPower;
    public bool isActivated;
}
