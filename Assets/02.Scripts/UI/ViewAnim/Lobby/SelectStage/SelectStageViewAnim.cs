using DG.Tweening;
using System;
using UnityEngine;

public class SelectStageViewAnim : UIPopAnimationBase
{
    [System.Serializable]
    public class StageButtonAnimData
    {
        public SelectStageButton button;
        public RectTransform rect;

        public Vector2 enterDirection;
        // 등장하는 방향
        // 왼쪽에서 등장 : (-1, 0), 오른쪽에서 등작 : (1, 0)
        // 아래에서 등작:  (0, -1), 위에서 등작 : (0, 1)
    }

    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private RectTransform panelRoot;

    [SerializeField]
    private StageButtonAnimData[] buttons;

    [SerializeField]
    private float flyDistance = 900f;
    [SerializeField]
    private float enterDuration = 0.35f;
    [SerializeField]
    private float exitDuration = 0.25f;
    [SerializeField]
    private float delay = 0.06f;

    [SerializeField]
    private float hoverScale = 1.15f;
    [SerializeField]
    private float seletedScale = 2.2f;
    [SerializeField]
    private float selectedMoveDuration;

    private Vector2[] originPos;
    private Tween[] hoverTweens;
    private Sequence sequence;

    private bool isOpen;
    private bool isSelected;

    public event Action ShowEnd;
    public event Action HideEnd;

    private void Awake()
    {
        hoverTweens = new Tween[buttons.Length];
        originPos = new Vector2[buttons.Length];

        for(int i = 0; i < buttons.Length; i++)
        {
            int index = i;

            originPos[index] = buttons[index].rect.anchoredPosition;
            buttons[index].button.OnSelect += () => SelectButton(index);
            buttons[index].button.OnHover += () => HoverEnter(index);
            buttons[index].button.OnExitHover += () => HoverExit(index);
            buttons[index].button.IsInputLocked = true;
        }
    }

    private void OnDestroy()
    {
        ShowEnd = null;
        HideEnd = null;
    }

    private void SelectButton(int selectIndex)
    {
        if (!isOpen || isSelected)
            return;

        isSelected = true;

        sequence?.Kill();
        sequence = DOTween.Sequence();

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        for(int i = 0; i< buttons.Length; i++)
        {
            if (i == selectIndex)
                continue;

            Vector2 outPos = GetOutPosition(originPos[i], buttons[i].enterDirection, flyDistance);
            sequence.Join(UIMoveTo(buttons[i].rect, outPos, exitDuration, Ease.InBack));
        }

        RectTransform selected = buttons[selectIndex].rect;

        sequence.Append(selected.DOAnchorPos(Vector2.zero, selectedMoveDuration)).SetEase(Ease.OutCubic);
        sequence.Join(selected.DOScale(seletedScale, selectedMoveDuration).SetEase(Ease.OutBack));
        sequence.OnComplete(() =>
        {
            buttons[selectIndex].button.OnSelectAnimComplete();
        });
    }

    private void HoverEnter(int index)
    {
        if (!isOpen || isSelected)
            return;

        hoverTweens[index]?.Kill();

        hoverTweens[index] = HoverScale(buttons[index].rect, hoverScale, 0.15f);
    }

    private void HoverExit(int index)
    {
        if (!isOpen || isSelected)
            return;

        hoverTweens[index]?.Kill();

        hoverTweens[index] = HoverScale(buttons[index].rect, 1f, 0.15f);
    }

    private void SetButtonInputLocekd(bool locked)
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].button.IsInputLocked = locked;
        }
    }

    public void Show()
    {
        if (isOpen)
            return;

        SetButtonInputLocekd(true);
        isOpen = true;
        isSelected = false;

        sequence?.Kill();

        canvasGroup.alpha = 0.0f;

        sequence = DOTween.Sequence();

        sequence.Join(Fade(canvasGroup, 1f, 0.2f));

        for(int i = 0; i < buttons.Length; i++)
        {
            RectTransform rect = buttons[i].rect;

            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;

            Vector2 outPos = GetOutPosition(originPos[i], buttons[i].enterDirection, flyDistance);

            rect.anchoredPosition = outPos;

            sequence.Insert(i * delay, UIMoveTo(rect, originPos[i], enterDuration, Ease.OutBack));
        }

        float fillStartTime = enterDuration + ((buttons.Length - 1) * delay);

        for(int i = 0; i < buttons.Length; i++)
        {
            sequence.Insert(fillStartTime, UIStrechEmptyHoriziontal(buttons[i].button.FillRect, 0.15f));

            sequence.Insert(fillStartTime, buttons[i].rect.DOLocalRotate(new Vector3(0f, 360f, 0f), 0.4f, RotateMode.FastBeyond360).SetEase(Ease.OutCubic));
        }

        sequence.OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            SetButtonInputLocekd(false);
            ShowEnd?.Invoke();
        });
    }

    public void Hide()
    {
        if (!isOpen || isSelected)
            return;

        SetButtonInputLocekd(true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        isOpen = false;

        sequence?.Kill();
        sequence = DOTween.Sequence();

        sequence.Join(Fade(canvasGroup, 0f, 1.2f));

        for (int i = 0; i < buttons.Length; i++)
        {
            Vector2 outPos = GetOutPosition(originPos[i], buttons[i].enterDirection, flyDistance);

            sequence.Insert(i * delay, UIMoveTo(buttons[i].rect, outPos, exitDuration, Ease.InBack));
        }

        sequence.OnComplete(() =>
        { 
            HideEnd?.Invoke();
        });
    }
}
