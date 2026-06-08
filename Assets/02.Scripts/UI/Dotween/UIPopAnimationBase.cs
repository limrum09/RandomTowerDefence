using DG.Tweening;
using System.Collections.Generic;
using TMPro;
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

    protected Sequence UISwapFromRightToLeft(RectTransform[] targets, TextMeshProUGUI[] texts, Dictionary<RectTransform, Vector2> originPosMap, System.Action changeContent, float distance = 150f, float duration = 0.18f, float delay = 0.03f)
    {
        Sequence se = DOTween.Sequence();

        for(int i = 0; i < texts.Length; i++)
        {
            se.Join(texts[i].DOFade(0f, duration * 0.5f));
        }

        se.AppendCallback(() =>
        {
            for(int i = 0; i < targets.Length; i++)
            {
                Vector2 origin = originPosMap[targets[i]];
                targets[i].anchoredPosition = new Vector2(origin.x + distance, origin.y);
            }

            for(int i = 0; i< texts.Length; i++)
            {
                texts[i].alpha = 0f;
            }
        });

        for (int i = 0; i < targets.Length; i++)
        {
            Vector2 origin = originPosMap[targets[i]];

            se.Join(targets[i].DOAnchorPos(origin, duration).SetEase(Ease.OutCubic));
        }

        for(int i = 0; i < texts.Length; i++)
        {
            se.Join(texts[i].DOFade(1f, duration));
        }

        return se;
    }

    protected Vector2 GetOutPosition(Vector2 origind, Vector2 dir, float distance)
    {
        return origind + dir.normalized * distance;
    }
}
