using UnityEngine;

/// <summary>
/// Pool에서 생성된 오브젝트가 어떤 Prefab에서 만들어졌고
/// 어떤 카테고리에 속하는지 기얻하는 컴포넌트
/// </summary>
public class PooledObject : MonoBehaviour
{
    public GameObject Originprefab { get; set; }

    public PoolCategory Category { get; set; }
}
