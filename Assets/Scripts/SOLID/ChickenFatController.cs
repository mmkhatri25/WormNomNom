using UnityEngine;

public class ChickenFatController
{
    private Transform _chickenTransform;
    private Vector3 _defaultScale;

    public ChickenFatController(Transform chickenTransform)
    {
        _chickenTransform = chickenTransform;
        _defaultScale = chickenTransform.localScale;
    }

    public void GrowFat()
    {
        _chickenTransform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }

    public void ResetFat()
    {
        _chickenTransform.localScale = _defaultScale;
    }
}
