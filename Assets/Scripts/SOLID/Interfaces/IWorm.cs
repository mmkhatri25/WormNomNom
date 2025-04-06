using UnityEngine;

public interface IWorm
{
    void Eat();
    bool IsPoisonous { get; }
    Transform GetTransform();
}
