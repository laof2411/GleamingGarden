using UnityEngine;

public class Cell
{
    public bool isUsable;

    public ISwitchable switchable;

    public Cell(bool _isUsable, ISwitchable _switchable)
    {
        isUsable = _isUsable;
        switchable = _switchable;
    }
}
