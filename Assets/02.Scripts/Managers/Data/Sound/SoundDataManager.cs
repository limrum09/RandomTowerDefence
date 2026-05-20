using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class SoundData
{
    public string uid;
    public AudioType type;
    public AudioClip clip;
}

public class SoundDataManager
{
    private Dictionary<string, SoundData> sounds = new Dictionary<string, SoundData>();
    private List<string> bgmUIDs = new List<string>();

    public void Init()
    {
        SoundDatabaseSO soundList = Resources.Load<SoundDatabaseSO>("Sound/SoundDatabase");

        sounds.Clear();

        foreach(var sound in soundList.Sounds)
        {
            if (sound == null || sound.clip == null)
                continue;
            sounds[sound.uid] = sound;

            if(sound.type == AudioType.BGM)
                bgmUIDs.Add(sound.uid);
        }
    }

    public AudioClip GetAudioClip(string uid)
    {
        if (!sounds.TryGetValue(uid, out SoundData sound))
            return null;

        return sound.clip;
    }

    public string GetNextBGMUID(string currentUID)
    {
        if (bgmUIDs.Count <= 0)
            return string.Empty;

        int index = bgmUIDs.IndexOf(currentUID);

        if (index < 0)
            return bgmUIDs[0];

        index++;

        if (index >= bgmUIDs.Count)
            index = 0;

        return bgmUIDs[index];
    }

    public string GetPrevBGMUID(string currentUID)
    {
        if(bgmUIDs.Count <= 0)
            return string.Empty;

        int index = bgmUIDs.IndexOf(currentUID);

        if(index < 0)
            return bgmUIDs[0];
        
        index--;

        if (index < 0)
            index = bgmUIDs.Count - 1;

        return bgmUIDs[index];
    }
}
