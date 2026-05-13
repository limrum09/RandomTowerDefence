using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField]
    private Transform moveObject;
    [SerializeField]
    private Vector3 targetPos;
    [SerializeField]
    private float duration;
    [SerializeField]
    private Ease easeType = Ease.OutQuad;

    private Vector3 originWordlPos;
    private Vector3 originLocalPos;

    private void Awake()
    {
        if (moveObject == null)
            moveObject = transform;

        originWordlPos = moveObject.position;
        originLocalPos = moveObject.localPosition;
    }

    public void MoveToWorldTarget()
    {
        moveObject.DOKill();
        moveObject.DOMove(targetPos, duration).SetEase(easeType);
    }

    public void MoveToWorldOrigin()
    {
        moveObject.DOKill();
        moveObject.DOMove(originWordlPos, duration).SetEase(easeType);
    }

    public void MoveToLocalTarget()
    {
        moveObject.DOKill();
        moveObject.DOLocalMove(originLocalPos + targetPos, duration).SetEase(easeType);
    }

    public void MoveToLocalOrigin()
    {
        moveObject.DOKill();
        moveObject.DOLocalMove(originLocalPos, duration).SetEase(easeType);
    }
}
