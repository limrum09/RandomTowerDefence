using System.Collections.Generic;
using UnityEngine;

public class GameEffectManager
{
    private Dictionary<string, GameObject> effectPrefabs = new Dictionary<string, GameObject>();
    public void Init()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Effects");

        foreach (GameObject pref in prefabs)
        {
            effectPrefabs[pref.name] = pref;
        }
    }

    public void Play(string name, Transform tf, PoolCategory category = PoolCategory.Common, bool isParent = false)
    {
        if (!effectPrefabs.TryGetValue(name, out GameObject obj))
            return;

        GameObject effect = Managers.Pool.Pop(obj, category);

        if (isParent)
        {
            GameEffectFollow follow = effect.GetComponent<GameEffectFollow>();
            
            if(follow == null)
               follow = effect.AddComponent<GameEffectFollow>();

            follow.SetTartget(tf, Vector3.zero);
        }            

        effect.transform.position = tf.position;
        effect.transform.rotation = Quaternion.identity;
    }
}
