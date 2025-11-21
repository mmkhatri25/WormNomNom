using UnityEngine;

public interface IWorm
{
    void Eat(GameObject warm = null);
    void Blast();
    bool IsPoisonous { get; }
    Transform GetTransform();
}
