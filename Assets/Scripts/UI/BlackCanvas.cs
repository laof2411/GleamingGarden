using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class BlackCanvas : MonoBehaviour
{
    [SerializeField] private float time;
    [SerializeField] private RectTransform mask;

    public void MakeMaskSmall()
    {
        mask.DOScale(Vector3.zero, time);
    }

    public void MakeMaskBig()
    {
        mask.DOScale(Vector3.one, time);
    }
}
