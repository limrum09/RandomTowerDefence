using DG.Tweening;
using System;
using UnityEngine;

public enum StageInfoPanelType
{
    None,
    Cover,
    Item,
    Enemy,
    TowerStatUpgrade,
    TowerGradeUpgrade,
}

[RequireComponent(typeof(RectTransform))]
[RequireComponent (typeof(CanvasGroup))]
public class InfoPanelController : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private string nextPageTrigger = "NextPage";
    [SerializeField]
    private string noNextPageTrigger = "NoNextPage";
    [SerializeField]
    private string prevPageTrigger = "PrevPage";


    [Header("DOTween Options")]
    [SerializeField]
    private float duration = 0.65f;
    [SerializeField]
    private Vector2 moveOffeset = new Vector2(250f, 180f);
    [SerializeField]
    private float startRotationZ = -8f;
    [SerializeField]
    private float exitRotationZ = 8f;
    [SerializeField]
    private Ease ease = Ease.OutCubic;

    [Header("Componenets")]
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private CanvasGroup canvas;
    
    private DG.Tweening.Sequence se;
    private Vector2 originPos;
    private Quaternion originRot;

    private event Action onNextPageCompleted;
    private event Action onPrevPageCompleted;

    private void Awake()
    {
        originPos = rect.anchoredPosition;
        originRot = rect.localRotation;

        Hide();
    }

    private void Kill()
    {
        if(se != null && se.IsActive())
        {
            se.Kill();
            se = null;
        }
    }

    public void Show()
    {
        Kill();

        rect.SetAsFirstSibling();

        rect.anchoredPosition = originPos;
        rect.localRotation = originRot;

        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void Hide()
    {
        Kill();

        rect.anchoredPosition = originPos;
        rect.localRotation = originRot;

        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    public void PlayCoverOpen(Action onArrived = null, Action onCompleted = null)
    {
        Kill();

        rect.anchoredPosition = originPos + moveOffeset;
        rect.localRotation = Quaternion.Euler(0f, 0f, startRotationZ);

        canvas.alpha = 1.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = true;

        se = DOTween.Sequence();

        se.Join(rect.DOAnchorPos(originPos, duration).SetEase(ease));
        se.Join(rect.DOLocalRotate(Vector3.zero, duration).SetEase(ease));

        se.OnComplete(() =>
        {
            onArrived?.Invoke();

            anim.ResetTrigger(nextPageTrigger);
            anim.SetTrigger(nextPageTrigger);

            onNextPageCompleted = onCompleted;
        });
    }

    public void PlayCoverClose(Action onCompleted = null)
    {
        Kill();

        rect.SetAsLastSibling();

        rect.anchoredPosition = originPos;
        rect.localRotation = originRot;

        canvas.alpha = 1.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = true;

        rect.anchoredPosition = originPos;
        rect.localRotation = Quaternion.identity;

        onPrevPageCompleted = () =>
        {
            onCompleted?.Invoke();

            se = DOTween.Sequence();

            se.Join(rect.DOAnchorPos(originPos + moveOffeset, duration).SetEase(Ease.InCubic));
            se.Join(rect.DOLocalRotate(new Vector3(0f, 0f, startRotationZ), duration).SetEase(Ease.InCubic));

            se.OnComplete(() =>
            {
                Hide();
            });
        };

        anim.ResetTrigger(prevPageTrigger);
        anim.SetTrigger(prevPageTrigger);
    }

    public void PlayNextPage(Action onCompleted = null)
    {
        Kill();

        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        onNextPageCompleted = onCompleted;

        anim.ResetTrigger(nextPageTrigger);
        anim.SetTrigger(nextPageTrigger);
    }

    public void PlayNextPageNoFillout(Action onCompleted = null)
    {
        Kill();

        canvas.interactable = false;
        canvas.blocksRaycasts = false;

        onNextPageCompleted = onCompleted;

        anim.ResetTrigger(noNextPageTrigger);
        anim.SetTrigger(noNextPageTrigger);
    }

    public void NextPageAnimationEnd()
    {
        Hide();

        Action completed = onNextPageCompleted;
        onNextPageCompleted = null;

        completed?.Invoke();
    }

    public void PrevPageAnimtionEnd()
    {
        Action completed = onPrevPageCompleted;
        onPrevPageCompleted = null;

        completed?.Invoke();
    }
}
