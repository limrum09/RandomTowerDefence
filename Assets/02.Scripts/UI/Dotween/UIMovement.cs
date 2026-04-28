using UnityEngine;
using DG.Tweening;
using System.Net.NetworkInformation;

public class UIMovement : MonoBehaviour
{
    [SerializeField]
    private RectTransform rect;
    [SerializeField]
    private Vector2 targetPosition;
    [SerializeField]
    private float duration;
    [SerializeField]
    private Ease easeType = Ease.OutQuad;

    private Vector2 originPosition;

    private void Awake()
    {
        if(rect == null)
            rect = GetComponent<RectTransform>();

        originPosition = rect.anchoredPosition;
    }

    public void MoveToTarget()
    {
        rect.DOKill();

        rect.DOAnchorPos(targetPosition, duration).SetEase(easeType);
    }

    public void MoveToOrigin()
    {
        rect.DOKill();

        rect.DOAnchorPos(originPosition, duration).SetEase(easeType);
    }
}
