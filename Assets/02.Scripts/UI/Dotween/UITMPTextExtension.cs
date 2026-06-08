using DG.Tweening;
using TMPro;
using UnityEngine;

public static class UITMPTextExtension
{
    public static Tween FadeIn(this TMP_Text text, float duration = 0.12f)
    {
        text.alpha = 0f;
        return text.DOFade(1f, duration);
    }

    public static Tween FadeOut(this TMP_Text text, float duration = 0.12f)
    {
        return text.DOFade(0f, duration);
    }

    public static Sequence SlideFadeIn(this TMP_Text text, float distance = 150f, float duration = 0.12f)
    {
        RectTransform rect = text.rectTransform;

        Vector2 origin = rect.anchoredPosition;

        rect.anchoredPosition = new Vector2(origin.x + distance, origin.y);

        text.alpha = 0f;

        Sequence se = DOTween.Sequence();

        se.Join(rect.DOAnchorPos(origin, duration));
        se.Join(text.DOFade(1f, duration));

        return se;
    }

    public static Sequence SlideFadeOut(this TMP_Text text, float distance = 150f, float duration = 0.12f)
    {
        RectTransform rect = text.rectTransform;

        Vector2 origin = rect.anchoredPosition;

        Vector2 target = new Vector2(origin.x + distance, origin.y);

        text.alpha = 1f;

        Sequence se = DOTween.Sequence();

        se.Join(rect.DOAnchorPos(target, duration));
        se.Join(text.DOFade(0f, duration));

        return se;
    }
}
