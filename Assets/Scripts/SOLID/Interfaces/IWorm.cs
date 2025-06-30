using UnityEngine;

public interface IWorm
{
    void Eat(GameObject warm = null);
    bool IsPoisonous { get; }
    Transform GetTransform();
}
