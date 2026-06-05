using DG.Tweening;
using UnityEngine;

public class UIPopAnimationBase : MonoBehaviour
{
    protected Tween Fade(CanvasGroup canvas, float alpha, float duration)
    {
        return canvas.DOFade(alpha, duration);
    }

    protected Tween UIMoveTo(RectTransform target, Vector2 pos, float duration, Ease ease = Ease.OutCubic)
    {
        return target.DOAnchorPos(pos, duration).SetEase(ease);
    }

    protected Tween Scale(Transform target, float scale, float duration)
    {
        return target.DOScale(scale, duration).SetEase(Ease.OutBack);
    }

    protected Tween HoverScale(Transform target, float scale, float duration)
    {
        return target.DOScale(scale, duration).SetEase(Ease.OutBack);
    }   

    protected Tween ClickPunch(Transform target)
    {
        return target.DOPunchScale(Vector3.one * 0.08f, 0.15f);
    }

    protected Tween UIFixedFillHorizontal(RectTransform fillRect, float width, float duration)
    {
        float height = fillRect.sizeDelta.y;

        fillRect.sizeDelta = new Vector2(0f, height);

        return fillRect.DOSizeDelta(new Vector2(width, height), duration).SetEase(Ease.Linear);
    }

    protected Tween UIStrechFillHorizontal(RectTransform fillRect, float width, float duration)
    {
        float height = fillRect.sizeDelta.y;

        fillRect.sizeDelta = new Vector2(0f, height);

        return fillRect.DOSizeDelta(new Vector2(width, height), duration).SetEase(Ease.Linear);
    }

    protected Tween UIFiexedEmptyHoriziontal(RectTransform fillRect, float originWidth, float duration)
    {
        float height = fillRect.sizeDelta.y;

        fillRect.sizeDelta = new Vector2(originWidth, height);

        return fillRect.DOSizeDelta(new Vector2(0f, height), duration).SetEase(Ease.Linear);
    }

    protected Tween UIStrechEmptyHoriziontal(RectTransform fillRect, float duration)
    {
        fillRect.localScale = Vector3.one;

        return fillRect.DOScaleX(0f, duration).SetEase(Ease.Linear);
    }

    protected Vector2 GetOutPosition(Vector2 origind, Vector2 dir, float distance)
    {
        return origind + dir.normalized * distance;
    }
}
