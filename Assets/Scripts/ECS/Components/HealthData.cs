using Unity.Entities;

public struct HealthData : IComponentData
{
    public int maxLives;
    public int currentLives;
    public int poisonCount; // 1 to 4
}
