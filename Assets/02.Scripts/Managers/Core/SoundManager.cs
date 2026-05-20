using System;
using UnityEngine;

public enum AudioType
{
    BGM,
    SFX
}

[Serializable]
public class SoundSaveData
{
    public string currentBGMUID;
    public float masterVolume;
    public float bgmVolume;
    public float sfxVolume;
    public float uiVolume;
    public bool isBGMPlaying;
    public bool isSFXPlaying;
}

public class SoundManager
{
    private string currentBGMUID;
    private float masterVolume;
    private float bgmVolume;
    private float sfxVolume;
    private float uiVolume;
    private bool isMasterPlaying;
    private bool isBGMPlaying;
    private bool isSFXPlaying;
    private bool isUIPlaying;

    private SoundDataManager soundDataManager;
    private AudioClip currentBGM;

    private AudioSource bgmAudioSource;
    private AudioSource sfxAudioSource;

    public string CurrentBGMStr => currentBGMUID;
    public float MasterVolume => masterVolume;
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;
    public float UIVolume => uiVolume;
    public bool IsMasterPlaying => isMasterPlaying;
    public bool IsBGMPlaying => isBGMPlaying;
    public bool IsSFXPlaying => isSFXPlaying;
    public bool IsUIPlaying => isUIPlaying;
    public void Init()
    {
        soundDataManager = new SoundDataManager();
        soundDataManager.Init();

        GameObject soundObj = new GameObject("SoundManager");
        bgmAudioSource = soundObj.AddComponent<AudioSource>();
        sfxAudioSource = soundObj.AddComponent<AudioSource>();
        soundObj.transform.SetParent(Managers.Instance.transform);

        ResetSound();
    }

    public void ResetSound()
    {
        masterVolume = 1.0f;
        bgmVolume = 1.0f;
        sfxVolume = 1.0f;
        uiVolume = 1.0f;
        isMasterPlaying = true;
        isBGMPlaying = true;
        isSFXPlaying = true;
        isUIPlaying = true;
        currentBGM = null;
        currentBGMUID = string.Empty;

        PlayBGM();
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        bgmAudioSource.volume = bgmVolume * masterVolume;
        Managers.Save.MarkSoundDirty();
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        bgmAudioSource.volume = bgmVolume * masterVolume;
        Managers.Save.MarkSoundDirty();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        Managers.Save.MarkSoundDirty();
    }
    public void SetUIVolume(float value)
    {
        uiVolume = Mathf.Clamp01(value);
        Managers.Save.MarkSoundDirty();
    }

    public bool StopMasterVolume()
    {
        isMasterPlaying = !isMasterPlaying;

        if (!isMasterPlaying)
            bgmAudioSource.Stop();
        else
            PlayBGM();

        Managers.Save.MarkSoundDirty();
        return isMasterPlaying;
    }

    public bool StopBGM()
    {
        isBGMPlaying = !isBGMPlaying;

        if (!isBGMPlaying || !isMasterPlaying)
            bgmAudioSource.Stop();
        else
            PlayBGM();

        Managers.Save.MarkSoundDirty();
        return isBGMPlaying;
    }

    public bool StopSFX()
    {
        isSFXPlaying = !isSFXPlaying;

        Managers.Save.MarkSoundDirty();
        return isSFXPlaying;
    }

    public bool StopUI()
    {
        isUIPlaying = !isUIPlaying;

        Managers.Save.MarkSoundDirty();
        return isUIPlaying;
    }

    public void NextBGM()
    {
        currentBGMUID = soundDataManager.GetNextBGMUID(currentBGMUID);
        PlayBGM();
    }

    public void PrevBGM()
    {
        currentBGMUID = soundDataManager.GetPrevBGMUID(currentBGMUID);
        PlayBGM();
    }

    public void PlayBGM()
    {
        if (!isBGMPlaying || !isMasterPlaying)
            return;

        if (currentBGM == null)
        {
            if(currentBGMUID == string.Empty)
                currentBGMUID = "BGM01";
        }

        currentBGM = soundDataManager.GetAudioClip(currentBGMUID);
        bgmAudioSource.clip = currentBGM;
        bgmAudioSource.Play();
    }

    public void PlayUISFX(string uid)
    {
        if (!isUIPlaying || !isMasterPlaying)
            return;

        AudioClip clip = soundDataManager.GetAudioClip(uid);

        if (clip == null)
            return;

        sfxAudioSource.volume = uiVolume * masterVolume;
        sfxAudioSource.PlayOneShot(clip);
    }

    public void PlaySFX(string uid)
    {
        if (!isSFXPlaying || !isMasterPlaying)
            return;

        AudioClip clip = soundDataManager.GetAudioClip(uid);

        if (clip == null)
            return;

        sfxAudioSource.volume = sfxVolume * masterVolume;
        sfxAudioSource.PlayOneShot(clip);
    }

    public void SaveSoundData()
    {
        Managers.Save.SaveSoundData();
    }

    public void LoadSoundSaveData(SoundSaveData saveData)
    {
        if (saveData == null)
            return;

        currentBGMUID = saveData.currentBGMUID;
        masterVolume = saveData.masterVolume;
        bgmVolume = saveData.bgmVolume;
        sfxVolume = saveData.sfxVolume;
        uiVolume = saveData.uiVolume;
        isBGMPlaying = saveData.isBGMPlaying;
        isSFXPlaying = saveData.isSFXPlaying;

        if (!isBGMPlaying)
            StopBGM();
        else
            PlayBGM();
    }

    public SoundSaveData GetSaveData()
    {
        return new SoundSaveData
        {
            bgmVolume = bgmVolume,
            sfxVolume = sfxVolume,
            masterVolume = masterVolume,
            uiVolume = uiVolume,
            isBGMPlaying = isBGMPlaying,
            isSFXPlaying = isSFXPlaying,
            currentBGMUID = currentBGMUID
        };
    }
}
