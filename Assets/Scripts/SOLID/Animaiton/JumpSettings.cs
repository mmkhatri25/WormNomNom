using UnityEngine;

[System.Serializable]
public class JumpSettings
{
    public AnimationCurve jumpCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // ease-in/out
    public float jumpDuration = 1f;
}
