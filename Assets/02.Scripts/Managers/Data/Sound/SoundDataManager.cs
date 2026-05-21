using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UID, 사운드 타입, 실제 AudioClip을 보유
/// </summary>
[Serializable]
public class SoundData
{
    public string uid;
    public AudioType type;
    public AudioClip clip;
}

/// <summary>
/// 사운드 데이터를 관리
/// SoundDatabaseOs에서 목록을 읽어와 UID를 기준으로 AudioClip을 반환
/// BGM 목록은 별도로 보관하여 다음/이전 BGM선택에 사용
/// </summary>
public class SoundDataManager
{
    private Dictionary<string, SoundData> sounds = new Dictionary<string, SoundData>();
    private List<string> bgmUIDs = new List<string>();

    /// <summary>
    /// SoundDatabaseSO를 Resoueces에서 불러와 Dictionary와 bgm목록을 초기화
    /// </summary>
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

    /// <summary>
    /// UID에 해당하는 AudioClip을 반환
    /// 존재하지 않으면 null을 반환
    /// </summary>
    /// <param name="uid"></param>
    /// <returns></returns>
    public AudioClip GetAudioClip(string uid)
    {
        if (!sounds.TryGetValue(uid, out SoundData sound))
            return null;

        return sound.clip;
    }

    /// <summary>
    /// 현재 UID를 바탕으로 다음 BGM UID를 반환
    /// 마지막 BGM이면 첫번째 BGM으로 순환하며 반환
    /// </summary>
    /// <param name="currentUID"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 현재 UID를 기준으로 이전 BGM UID를 반환
    /// 첫번째 BGM이면 마지막 BGM으로 순환하며 반환
    /// </summary>
    /// <param name="currentUID"></param>
    /// <returns></returns>
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
