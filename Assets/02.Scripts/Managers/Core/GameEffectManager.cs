using System.Collections.Generic;
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
        if (!effectPrefabs.TryGetValue(name, out GameObject obj))
            return;

        GameObject effect = Managers.Pool.Pop(obj, category);

        if (isFollow)
        {
            // 이펙트에서 GameEffectFollow 컴포넌트 추출
            GameEffectFollow follow = effect.GetComponent<GameEffectFollow>();
            
            // 컴포넌트 없을 시 추가
            if(follow == null)
               follow = effect.AddComponent<GameEffectFollow>();

            // tf로 타겟 설정하며 offset은 일단 Vector3.zero로 설정
            follow.SetTartget(tf, Vector3.zero);
        }            

        // tf위치로 이동
        effect.transform.position = tf.position;
        effect.transform.rotation = Quaternion.identity;
    }
}