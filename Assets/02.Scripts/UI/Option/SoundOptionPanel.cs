using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 사운드 옵션 UI 패널
/// Master/BGM/SFX/UI 볼륨 조절, 각 사운드 음소거 조절
/// BGM 이전/다음 변경, 사운드 옵션 초기화/저장 담당
/// 실제 사운드 적용은 SoundManager가 담당
/// </summary>
public class SoundOptionPanel : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    private GameObject resetCheckPanel;

    [Header("Sliders")]
    [SerializeField]
    private Slider masterSlider;
    [SerializeField]
    private Slider bgmSlider;
    [SerializeField]
    private Slider sfxSlider;
    [SerializeField]
    private Slider uiSlider;

    [Header("Buttons")]
    [SerializeField]
    private SoundIconButton masterVolumeButton;
    [SerializeField]
    private SoundIconButton bgmVolumeButton;
    [SerializeField]
    private SoundIconButton sfxVolumeButton;
    [SerializeField]
    private SoundIconButton uiVolumeButton;
    [SerializeField]
    private Button nextBGMButton;
    [SerializeField]
    private Button prevBGMButton;

    [Header("Texts")]
    [SerializeField]
    private TextMeshProUGUI currentBgmText;

    private SoundManager sound;

    
    void Start()
    {
        sound = Managers.Sound;

        SetInitToSoundManager();
        BindButtons();
        BindSliders();
        
        resetCheckPanel.SetActive(false);
    }

    /// <summary>
    /// 현재 SoundManager에 저장된 사운드 설정 값을 UI에 반영
    /// </summary>
    private void SetInitToSoundManager()
    {
        masterSlider.value = sound.MasterVolume;
        bgmSlider.value = sound.BGMVolume;
        sfxSlider.value = sound.SFXVolume;
        uiSlider.value = sound.UIVolume;
        currentBgmText.text = sound.CurrentBGMStr;
        masterVolumeButton.SetIconView(sound.IsMasterPlaying);
        bgmVolumeButton.SetIconView(sound.IsBGMPlaying);
        sfxVolumeButton.SetIconView(sound.IsSFXPlaying);
        uiVolumeButton.SetIconView(sound.IsUIPlaying);
    }

    /// <summary>
    /// 버튼들의 이벤트를 연결
    /// 각 버튼은 SoundManager의 음소거 상태를 변경하고, 아이콘 표시 상태를 갱신
    /// </summary>
    private void BindButtons()
    {
        masterVolumeButton.BindFunc(sound.StopMasterVolume);
        bgmVolumeButton.BindFunc(sound.StopBGM);
        sfxVolumeButton.BindFunc(sound.StopSFX);
        uiVolumeButton.BindFunc(sound.StopUI);

        nextBGMButton.onClick.AddListener(() =>
        {
            sound.NextBGM();
            currentBgmText.text = sound.CurrentBGMStr;
        });

        prevBGMButton.onClick.AddListener(() =>
        {
            sound.PrevBGM();
            currentBgmText.text = sound.CurrentBGMStr;
        });
    }

    /// <summary>
    /// 볼륨 슬라이더 리벤트를 연결
    /// 슬라이더 값이 변경되면 SoundManager에 즉시 반영
    /// </summary>
    private void BindSliders()
    {
        masterSlider.onValueChanged.AddListener(value =>
        {
            sound.SetMasterVolume(value);
        });

        bgmSlider.onValueChanged.AddListener(value =>
        {
            sound.SetBGMVolume(value);
        });

        sfxSlider.onValueChanged.AddListener(value =>
        {
            sound.SetSFXVolume(value);
        });

        uiSlider.onValueChanged.AddListener(value =>
        {
            sound.SetUIVolume(value);
        });
    }

    /// <summary>
    /// 사운드 옵션을 기본값으로 되돌리고 UI 갱신
    /// </summary>
    public void ResetSoundOption()
    {
        sound.ResetSound();
        SetInitToSoundManager();
    }

    /// <summary>
    /// 현재 사운드를 저장
    /// </summary>
    public void SaveSoundOption()
    {
        Managers.Save.SaveSoundData();
    }
}
