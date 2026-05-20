using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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

    [Header("Local Texts")]
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI resetButtonText;
    [SerializeField]
    private TextMeshProUGUI applyButtonText;
    [SerializeField]
    private TextMeshProUGUI OKButtonText;
    [SerializeField]
    private TextMeshProUGUI resetCheckText;
    [SerializeField]
    private TextMeshProUGUI resetCancelText;
    [SerializeField]
    private TextMeshProUGUI resetOKText;
    [SerializeField]
    private TextMeshProUGUI masterVolumeText;
    [SerializeField]
    private TextMeshProUGUI bgmVolumeText;
    [SerializeField]
    private TextMeshProUGUI sfxVolumeText;
    [SerializeField]
    private TextMeshProUGUI uiVolumetext;
    [SerializeField]
    private TextMeshProUGUI selectBgmText;

    private SoundManager sound;
    private LocalizationDataManager local;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        local = Managers.Local;
        title.text = local.GetString("TEXT_SETTING_SOUND");
        resetButtonText.text = local.GetString("BUTTON_RESET");
        applyButtonText.text = local.GetString("BUTTON_APPLY");
        OKButtonText.text = local.GetString("BUTTON_OK");
        resetCheckText.text = local.GetString("TEXT_RESET_CHECK");
        resetCancelText.text = local.GetString("BUTTON_CANCEL");
        resetOKText.text = local.GetString("BUTTON_OK");
        masterVolumeText.text = local.GetString("SETTING_MASTER_VOLUME");
        bgmVolumeText.text = local.GetString("SETTING_BGM_VOLUME");
        sfxVolumeText.text = local.GetString("SETTING_SFX_VOLUME");
        uiVolumetext.text = local.GetString("SETTING_UI_VOLUME");
        selectBgmText.text = local.GetString("SETTING_BGM_SELECT");

        sound = Managers.Sound;

        SetInitToSoundManager();

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

        resetCheckPanel.SetActive(false);
    }


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

    public void ResetSoundOption()
    {
        sound.ResetSound();
        SetInitToSoundManager();
    }
}
