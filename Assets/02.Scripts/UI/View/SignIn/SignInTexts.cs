using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SignInTexts : UIClickBase
{
    [Header("DoTween")]
    [SerializeField]
    private float hoverScale = 1.2f;
    [SerializeField]
    private float tweenDuration = 0.2f;

    [Header("Event")]
    [SerializeField]
    private UnityEvent onClick;

    private Tween scaleTween;
    private Vector3 originScale;

    private void Awake()
    {
        originScale = transform.localScale;
    }

    protected override void OnClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    protected override void OnEnter(PointerEventData eventData)
    {
        scaleTween?.Kill();

        scaleTween = transform.DOScale(originScale * hoverScale, tweenDuration).SetEase(Ease.OutBack);
    }

    protected override void OnExit(PointerEventData eventData)
    {
        scaleTween?.Kill();

        scaleTween = transform.DOScale(originScale, tweenDuration).SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }
}
