using UnityEngine;
using UnityEngine.VFX;

public class Cell
{
    public Vector3 switchablePosition;
    public bool isUsable;

    public ISwitchable switchable;
    public ParticleSystem dissapearEffect;

    public Cell(bool _isUsable, ISwitchable _switchable, ParticleSystem _dissapearEffect, Vector3 _switchablePosition)
    {
        isUsable = _isUsable;
        switchable = _switchable;
        dissapearEffect = _dissapearEffect;
        switchablePosition = _switchablePosition;
    }
}
