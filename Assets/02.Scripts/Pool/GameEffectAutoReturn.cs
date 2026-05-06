using UnityEngine;

/// <summary>
/// Pool에서 꺼낸 Paricel Effect를 재생하고, 재생 시간이 끝나면 Pool로 반납
/// 해당 스크립트는 보통 ParticleSystem Prefab의 최상위 오브젝트에 붙인다.
/// </summary>
public class GameEffectAutoReturn : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        // 현재 오브젝트에 ParticleSystem이 있다면 가져온다.
        ps = GetComponent<ParticleSystem>();

        // 현제 오브젝트의 자식으로 ParticleSystem이 있다면 가져온다.
        // 이펙트가 나오는 ParticleSystem을 자식으로 두고있으면 사용
        if (ps == null)
            ps = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Pool에서 SetActive(true)가 되었을 때 자동으로 호출
    /// </summary>
    private void OnEnable()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        // 이전에 남아 있는 파티클 제거
        ps.Clear(true);
        // 파티클 재생
        ps.Play(true);

        // ParticleSystem의 대략적인 재생 시간 계산
        float lifeTime = ps.main.duration + ps.main.startLifetime.constantMax;

        // lifeTime위에 Pool 반납 함수 호출
        Invoke(nameof(ReturnPool), lifeTime);
    }

    /// <summary>
    /// Pool이 반납되면 SetActive(false)가 되면 호출
    /// 예약된 invoke를 제거하여 중복 반납을 방지
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke();
    }

    /// <summary>
    /// 재생이 종료된 Effect를 Pool에 반납
    /// </summary>
    private void ReturnPool()
    {
        Managers.Pool.Push(gameObject);
    }
}
