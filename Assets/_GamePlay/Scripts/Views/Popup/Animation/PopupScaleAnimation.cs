using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Views
{
    public class PopupScaleAnimation : MonoBehaviour, IPopupAnimation
{
    public Ease ease = Ease.OutBack;
    public float time = 0.2f;
    public void OnShow(GameObject view, Action onComplete)
    {
        view.transform.localScale = Vector3.zero;
        view.transform.DOScale(Vector3.one, time).SetEase(ease).OnComplete(() => onComplete?.Invoke()).SetUpdate(true);
    }

    public void OnHide(GameObject view, Action onComplete)
    {
        view.transform.DOScale(Vector3.zero, time).OnComplete(() => onComplete?.Invoke()).SetUpdate(true);
    }

}
}