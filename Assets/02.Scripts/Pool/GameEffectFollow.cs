using UnityEngine;

/// <summary>
/// 이펙트가 타겟의 위치 이동에 따라 같이 이동할 필요가 있을 시 사용
/// </summary>
public class GameEffectFollow : MonoBehaviour
{
    private Transform target;   // 타겟
    private Vector3 offset;     // 다겟에서 생성된 위치

    public void SetTartget(Transform target, Vector3 offest)
    {
        this.target = target;
        this.offset = offest;
    }

    private void LateUpdate()
    {
        // 타겟이 없거나, Hierarchy에서 타겟이 active상태가 아니라면 종료
        // 대부분 적들이 이펙트 정보를 들고있다가 타워의 공격이나 도착해서 죽는 경우
        if (target == null || !target.gameObject.activeInHierarchy)
        {
            // 해당 이펙트 Pool에 반납
            Managers.Pool.Push(gameObject);
            return;
        }

        // 이동
        transform.position = target.position + offset;
    }

    // active상태가 꺼진다면 초기화
    private void OnDisable()
    {
        target = null;
        offset = Vector3.zero;
    }
}
