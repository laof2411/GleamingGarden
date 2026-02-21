using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Flower : MonoBehaviour, ISwitchable
{
    [SerializeField] private SwitchableType _type;

    private int _xIndex;
    private int _yIndex;
    private bool _isMatched;
    private Vector2 _currentPos;
    private Vector2 _targetPos;
    private bool _isMoving;
    [SerializeField] private Animator _animator;

    #region Properties
    public int xIndex
    {
        get {  return _xIndex; }
        set { _xIndex = value; }
    }
    public int yIndex
    {
        get { return _yIndex; }
        set { _yIndex = value; }
    }
    public bool isMatched
    {
        get { return _isMatched; }
        set { _isMatched = value; }
    }
    public Vector2 currentPos
    {
        get { return _currentPos; }
        set { _currentPos = value; }
    }
    public Vector2 targetPos
    {
        get { return _targetPos; }
        set { _targetPos = value; }
    }
    public bool isMoving
    {
        get { return _isMoving; }
        set { _isMoving = value; }
    }
    public SwitchableType type
    {
        get { return _type; }
        set { _type = value; }
    }

    public Animator animator
    {
        get { return _animator; }
        set { _animator = value; }
    }
    #endregion Properties

    public Flower(int _x, int _y)
    {
        xIndex = _x;
        yIndex = _y;
    }

    private void Start()
    {
        float random = Random.Range(0.0f, 1.0f);

        StartCoroutine(IdleAnimationCoroutine(random));
    }

    private IEnumerator IdleAnimationCoroutine(float _seconds)
    {
        yield return new WaitForSeconds(_seconds);

        int random = Random.Range(0, 1);
        if(random == 0)
        {
            animator.SetBool("left",true);
        }
        else
        {
            animator.SetBool("right", true);
        }      
    }

    public void DisableSwitchable()
    {
        AudioManager.instance.PlayAudio("Poof");
        this.gameObject.SetActive(false);
    }

    public void MoveToTarget(Vector2 targetPosition)
    {
        StartCoroutine(MoveCoroutine(targetPosition));
    }

    public IEnumerator MoveCoroutine(Vector2 targetPosition)
    {
        isMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(startPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}


