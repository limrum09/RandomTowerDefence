using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Pool을 용도별로 구분하기 위한 카테고리
/// Common : 여러 Scene에서 공용으로 사용하는 오브젝트
/// UI : UI 연출, 버튼 이펙트 등에 사용, 나중에 뽑기를 추가할지 모르겠지만 추가할 경우 사용
/// Stage : 스테이지 전용 이펙트, 적, 투사체(화살) 등에 사용. Stage종료하면 전부 삭제할 듯
/// </summary>
public enum PoolCategory
{
    Common,
    Stage,
    UI
}

public class PoolManager
{
    // 카테고리별 Pool 저장소
    // PoolCategory -> Prefab -> 비활성화 오브젝트 Queue
    private readonly Dictionary<PoolCategory, Dictionary<GameObject, Queue<GameObject>>> pools = new Dictionary<PoolCategory, Dictionary<GameObject, Queue<GameObject>>>();

    // 카테고리별 root transform 저장
    // 반납된 오브젝트를 정리하는 부모 오브젝트, Pool을 삭제할때 사용함
    private readonly Dictionary<PoolCategory, Transform> roots = new Dictionary<PoolCategory, Transform>();

    private Transform poolRoot;

    /// <summary>
    /// Pool Manager초기화, Managers에서 호출
    /// </summary>
    public void Init()
    {
        GameObject rootObj = new GameObject("PoolManager");
        Object.DontDestroyOnLoad(rootObj);
        poolRoot = rootObj.transform;

        for(int i = 0; i < System.Enum.GetValues(typeof(PoolCategory)).Length; i++)
        {
            CreateCategoryPool((PoolCategory)i);
        }
    }

    /// <summary>
    /// 카테고리별로 Root 오브젝트 생성
    /// </summary>
    /// <param name="category"></param>
    private void CreateCategoryPool(PoolCategory category)
    {
        GameObject categoryObj = new GameObject($"Pool_{category}");
        categoryObj.transform.SetParent(poolRoot);

        roots[category] = categoryObj.transform;
        pools[category] = new Dictionary<GameObject, Queue<GameObject>>();
    }

    /// <summary>
    /// Pool에서 오브젝트를 꺼난다.
    /// 남는 오브젝트가 있으면 재사용하고, 없으면 새로 생성한다.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="category"></param>
    /// <returns></returns>
    public GameObject Pop(GameObject prefab, PoolCategory category = PoolCategory.Common)
    {
        if(prefab == null)
        {
            return null;
        }

        // 카테고리가 없을 시 생성
        if (!pools.ContainsKey(category))
        {
            CreateCategoryPool(category);
        }

        Dictionary<GameObject, Queue<GameObject>> categoryPool = pools[category];

        // 해당 Prefab의 Queue가 없으면 새로 생성
        if(!categoryPool.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            categoryPool[prefab] = pool;
        }

        GameObject obj = null;

        // 비활성화 오브젝트가 있다면 재사용
        if(pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
        }
        // 없으면 새로 생성
        else
        {
            obj = Object.Instantiate(prefab, roots[category]);

            // 해당 오브젝트가 어떤 prefab에서 만들어 졌는지 기록
            PooledObject pooledObject = obj.GetComponent<PooledObject>();

            if(pooledObject == null)
                pooledObject = obj.AddComponent<PooledObject>();

            pooledObject.Originprefab = prefab;
            pooledObject.Category = category;
        }

        return obj;
    }

    /// <summary>
    /// 사용이 끝난 오브젝트는 Pool에 반납
    /// </summary>
    /// <param name="obj"></param>
    public void Push(GameObject obj)
    {
        if (obj == null)
            return;

        PooledObject pooledObject = obj.GetComponent<PooledObject> ();

        // Pool에서 생성된 Obj가 아니면 삭제
        if(pooledObject == null || pooledObject.Originprefab == null)
        {
            Object.Destroy(obj);
            return;
        }

        PoolCategory category = pooledObject.Category;

        if (!pools.ContainsKey(category))
        {
            CreateCategoryPool(category);
        }

        // Pool Root 밑으로 다시 정렬
        obj.transform.SetParent(roots[category]);

        // 안보이게 비활성화
        obj.SetActive(false);

        Dictionary<GameObject, Queue<GameObject>> categoryPool = pools[category];

        // 해당 Prefab의 Queue가 없으면 생성
        if(!categoryPool.TryGetValue(pooledObject.Originprefab, out Queue<GameObject> pool))
        {
            pool = new Queue<GameObject>();
            categoryPool[pooledObject.Originprefab] = pool;
        }

        // 가시 재사용 가능하고록 Queue에 저장
        pool.Enqueue(obj);
    }

    /// <summary>
    /// 특정 카테고리의 Pool만 삭제
    /// 스테이지 종료 시, Stage Pool만 삭제
    /// </summary>
    /// <param name="category"></param>
    public void CategoryClear(PoolCategory category)
    {
        if (!pools.TryGetValue(category, out Dictionary<GameObject, Queue<GameObject>> categoryPool))
            return;

        foreach(var pool in categoryPool)
        {
            Queue<GameObject> queue = pool.Value;

            while(queue.Count > 0)
            {
                GameObject obj = queue.Dequeue();

                if(obj != null)
                    Object.Destroy(obj);
            }
        }

        categoryPool.Clear();
    }

    /// <summary>
    /// 모든 Pool 삭제
    /// 게임 종료 또는 전체 리셋
    /// </summary>
    public void ClearAll()
    {
        foreach (PoolCategory category in pools.Keys)
            CategoryClear(category);
    }
}
