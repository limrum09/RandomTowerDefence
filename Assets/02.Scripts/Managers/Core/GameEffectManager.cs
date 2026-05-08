using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class GameEffectManager
{
    private Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();
    public void Init()
    {
        // Resources의 Effects폴더에 있는 모든 GameObject가져오기
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Effects");

        foreach (GameObject pref in prefabs)
        {
            // 이름으로 저장
            effectPrefabs[pref.name] = pref;
        }
    }

    /// <summary>
    /// 이름으로 검색을 하여 생성, isFollow 유무에 따라 이펙트가 따라갈지 해당위치에만 생성될지 정함
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tf"></param>
    /// <param name="category"></param>
    /// <param name="isParent"></param>
    public void Play(string name, Transform tf, PoolCategory category = PoolCategory.Common, bool isFollow = false)
    {
        if (tf == null)
            return;

        GameObject effect = SpawnEffect(name, tf.position, category, Quaternion.identity);

        if (effect == null)
            return;

        if (isFollow)
            SetFollow(effect, tf, Vector3.zero);
    }

    public void Play2DAnimationEffect(string name, Transform target, Transform tower, PoolCategory category = PoolCategory.Common, float randomPositionRange = 0.15f, float randomRoationRange = 0.15f, bool isFacing = false)
    {
        if (target == null)
            return;

        Vector3 randomOffest = new Vector3(0f, UnityEngine.Random.Range(-randomPositionRange, randomPositionRange), 0f);
        Quaternion randomQuaternion = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-randomRoationRange, randomRoationRange));

        GameObject effect = SpawnEffect(name, target.position + randomOffest, category, randomQuaternion);

        if (effect == null)
            return;

        SetFollow(effect, target, randomOffest);

        if (tower != null)
        {
            SpriteRenderer sp = effect.GetComponentInChildren<SpriteRenderer>();

            if (sp != null)
                sp.flipX = isFacing;
        }
    }

    private GameObject SpawnEffect(string name, Vector3 position, PoolCategory category, Quaternion rotation)
    {
        if (!effectPrefabs.TryGetValue(name, out GameObject obj))
            return null;

        GameObject effect = Managers.Pool.Pop(obj, category);

        // tf위치로 이동
        effect.transform.position = position;
        effect.transform.rotation = rotation;

        return effect;
    }

    private void SetFollow(GameObject effect, Transform tf, Vector3 offset)
    {
        // 이펙트에서 GameEffectFollow 컴포넌트 추출
        GameEffectFollow follow = effect.GetComponent<GameEffectFollow>();

        // 컴포넌트 없을 시 추가
        if (follow == null)
            follow = effect.AddComponent<GameEffectFollow>();

        // tf로 타겟 설정하며 offset은 일단 Vector3.zero로 설정
        follow.SetTartget(tf, Vector3.zero);
    }
}