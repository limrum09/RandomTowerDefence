using UnityEngine;

public class GameEffectFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;

    public void SetTartget(Transform target, Vector3 offest)
    {
        this.target = target;
        this.offset = offest;
    }

    private void LateUpdate()
    {
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            Managers.Pool.Push(gameObject);
            return;
        }

        transform.position = target.position + offset;
    }

    private void OnDisable()
    {
        target = null;
        offset = Vector3.zero;
    }
}
