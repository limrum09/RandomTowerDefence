using System;
using UnityEngine;

/// <summary>
/// 사운드 종류
/// BGM : 배경음악, SFX : 효과음
/// </summary>
public enum AudioType
{
    BGM,
    SFX
}

/// <summary>
/// 사운드 옵션 저장 데이터
/// 현제 BGM, 볼륨 값, BGM/SFX 재생 여부 저잘
/// </summary>
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
    public bool isMasterPlaying;
    public bool isUIPlaying;
}

/// <summary>
/// 게임 전체 사운드를 관리
/// BGM/SFX.UI 사운드 재생, 볼륨 조절, 음소거, BGM 변경, 저장 데이터 적용을 담당
/// </summary>
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
    
    /// <summary>
    /// 현재 마스터 볼륨과 BGM 볼륨을 AudioSource에 적용
    /// </summary>
    private void ApplyBGMVolume()
    {
        bgmAudioSource.volume = bgmVolume * masterVolume;
    }

    /// <summary>
    /// 데이터와 AudioSource를 초기화
    /// 생성한 사운드 오브젝트는 Managers의 자식으로배치되어 씬 이동 시 유지
    /// </summary>
    public void Init()
    {
        soundDataManager = new SoundDataManager();
        soundDataManager.Init();

        GameObject soundObj = new GameObject("SoundManager");
        bgmAudioSource = soundObj.AddComponent<AudioSource>();
        sfxAudioSource = soundObj.AddComponent<AudioSource>();
        soundObj.transform.SetParent(Managers.Instance.transform);

        ResetSound(false);
    }

    /// <summary>
    /// 사운드 옵션을 기본 값으로 초기화, 기본 BGM을 재생
    /// </summary>
    public void ResetSound(bool markDirty = true)
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

        if(markDirty)
            Managers.Save.MarkSoundDirty();
    }

    /// <summary>
    /// 마스터 볼륨을 설정
    /// 모든 사운드 볼륨의 최종 재율로 사용됨
    /// </summary>
    /// <param name="value">넘겨 받은 값</param>
    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyBGMVolume();
        Managers.Save.MarkSoundDirty();
    }

    /// <summary>
    /// BGM 볼륨을 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetBGMVolume(float value)
    {
        bgmVolume = Mathf.Clamp01(value);
        ApplyBGMVolume();
        Managers.Save.MarkSoundDirty();
    }

    /// <summary>
    /// SFX 볼륨을 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        Managers.Save.MarkSoundDirty();
    }

    /// <summary>
    /// UI 효과음 볼륨을 설정
    /// </summary>
    /// <param name="value"></param>
    public void SetUIVolume(float value)
    {
        uiVolume = Mathf.Clamp01(value);
        Managers.Save.MarkSoundDirty();
    }

    /// <summary>
    /// 전체 사운드 재생 상태를 토글
    /// 상태에 따라 전체 볼륨들이 꺼지가나 켜진다.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// BGM 재생여부를 토글
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// SFX 재생여부를 토글
    /// </summary>
    /// <returns></returns>
    public bool StopSFX()
    {
        isSFXPlaying = !isSFXPlaying;

        Managers.Save.MarkSoundDirty();
        return isSFXPlaying;
    }

    /// <summary>
    /// UI 효과음 재생여부를 토글
    /// </summary>
    /// <returns></returns>
    public bool StopUI()
    {
        isUIPlaying = !isUIPlaying;

        Managers.Save.MarkSoundDirty();
        return isUIPlaying;
    }

    /// <summary>
    /// 다음 BGM으로 변경하고 재생
    /// </summary>
    public void NextBGM()
    {
        currentBGMUID = soundDataManager.GetNextBGMUID(currentBGMUID);
        PlayBGM();
        Managers.Save.MarkSoundDirty();
    }

    /// <summary>
    /// 이전 BGM으로 변경하고 재생
    /// </summary>
    public void PrevBGM()
    {
        currentBGMUID = soundDataManager.GetPrevBGMUID(currentBGMUID);
        PlayBGM();
        Managers.Save.MarkSoundDirty();
    }

    /// <summary>
    /// 현재 BGM UID에 해당하는 음악을 재생
    /// UID가 비어있으면 기본 BGM01을 사용
    /// </summary>
    public void PlayBGM()
    {
        if (!isBGMPlaying || !isMasterPlaying)
            return;

        if (string.IsNullOrEmpty(currentBGMUID))
            currentBGMUID = "BGM01";

        currentBGM = soundDataManager.GetAudioClip(currentBGMUID);

        if (currentBGM == null)
            return;
        
        bgmAudioSource.clip = currentBGM;
        ApplyBGMVolume();
        bgmAudioSource.Play();
    }

    /// <summary>
    /// UI 효과음을 재생
    /// UI 사운드 또는 마스터 사운드가 꺼져 있으면, 재생하지 않음
    /// </summary>
    /// <param name="uid"></param>
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

    /// <summary>
    /// SFX 재생
    /// SFX 또는 마스터 사운드가 꺼져 있으면, 재생하지 않음
    /// </summary>
    /// <param name="uid"></param>
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

    /// <summary>
    /// 현재 사운드 옵션 데이터 저장
    /// </summary>
    public void SaveSoundData()
    {
        Managers.Save.SaveSoundData();
    }

    /// <summary>
    /// 저장된 사운드 옵션을 적용
    /// 토글 함수는 상태를 뒤지기 때문에 로드 과정에서는 직접 호출하지 않음
    /// </summary>
    /// <param name="saveData"></param>
    public void LoadSoundSaveData(SoundSaveData saveData)
    {
        if (saveData == null)
            return;

        currentBGMUID = saveData.currentBGMUID;

        masterVolume = Mathf.Clamp01(saveData.masterVolume);
        bgmVolume = Mathf.Clamp01(saveData.bgmVolume);
        sfxVolume = Mathf.Clamp01(saveData.sfxVolume);
        uiVolume = Mathf.Clamp01(saveData.uiVolume);

        isBGMPlaying = saveData.isBGMPlaying;
        isSFXPlaying = saveData.isSFXPlaying;
        isMasterPlaying = saveData.isMasterPlaying;
        isUIPlaying = saveData.isUIPlaying;

        ApplyBGMVolume();

        if (isBGMPlaying && IsMasterPlaying)
            PlayBGM();
        else
            bgmAudioSource.Stop();
    }

    /// <summary>
    /// 저장용 사운드 데이터 반환
    /// </summary>
    /// <returns></returns>
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
