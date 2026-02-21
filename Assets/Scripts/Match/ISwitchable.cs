using UnityEngine;
using System.Collections;

public interface ISwitchable
{
    public int xIndex { get; set; }
    public int yIndex { get; set; }

    public bool isMatched { get; set; }
    public bool isMoving { get; set; }

    public Vector2 currentPos { get; set; }
    public Vector2 targetPos { get; set; }

    public SwitchableType type { get; set; }

    public Animator animator { get; set; }

    public void SetIndicacies(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    public void MoveToTarget(Vector2 targetPosition);
    public IEnumerator MoveCoroutine(Vector2 targetPos);
    public void DisableSwitchable();
    public GameObject GetGameObject();
}

public enum SwitchableType
{
    Iris,
    ForgetMeNot,
    Jacynth,
    Petunia,
    Hypericum,
}


