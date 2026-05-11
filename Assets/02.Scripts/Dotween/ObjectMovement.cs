using DG.Tweening;
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

    private Vector3 originPosition;

    private void Awake()
    {
        if (moveObject == null)
            moveObject = transform;

        originPosition = moveObject.position;
    }

    public void MoveToTarget()
    {
        moveObject.DOKill();
        moveObject.DOMove(targetPos, duration).SetEase(easeType);
    }

    public void MoveToOrigin()
    {
        moveObject.DOKill();
        moveObject.DOMove(originPosition, duration).SetEase(easeType);
    }

    public void SaveCurrentOrigin()
    {
        originPosition = moveObject.position;
    }
}
