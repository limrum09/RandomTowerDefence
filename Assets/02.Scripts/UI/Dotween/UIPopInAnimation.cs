using DG.Tweening;
using UnityEngine;

public class UIPopInAnimation : UIPopAnimationBase
{
    [SerializeField]
    private RectTransform target;
    [SerializeField]
    private float startScale = 0.9f;
    [SerializeField]
    private float overshootScale = 1.05f;
    [SerializeField]
    private float duration = 0.25f;

    private Sequence se;


    private void Awake()
    {
        if (target == null)
            target = transform as RectTransform;
    }

    private void OnDestroy()
    {
        se?.Kill();
    }

    public void Play()
    {
        if (target == null)
            return;

        se?.Kill();

        target.localScale = Vector3.one * startScale;

        se = DOTween.Sequence();
        se.Append(Scale(target, overshootScale, duration));
        se.Append(Scale(target, 1f, duration));
    }

    public void ResetScale()
    {
        se?.Kill();

        if (target != null)
            target.localScale = Vector3.one;
    }
}
