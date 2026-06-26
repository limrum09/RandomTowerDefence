using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

/// <summary>
/// 그래픽 옵션 UI패널
/// 해상고, FPS 제한, 화면 모드, 데미지 텍스트 표시 여부를 UI에서 변경
/// 실제 옵션 값 적용은 GraphicManager가 담당
/// 저장은 SaveDataManager가 담당한다.
/// </summary>
public class GraphicOptionPanel : MonoBehaviour
{
    [Header("Option")]
    [SerializeField]
    private TMP_Dropdown resolutionDropdown;
    [SerializeField]
    private TMP_Dropdown fpsLimitDropdown;
    [SerializeField]
    private Toggle screenModeToggle;
    [SerializeField]
    private Toggle damageViewToggle;
    [SerializeField]
    private TMP_Dropdown languageDropDown;

    private readonly List<Locale> availableLocales = new List<Locale>();
    private bool isSettingLanguageDropdown;

    private void Start()
    {
        SetResolutionDropDown();
        SetFPSDropdown();
        SetIsScreenToggle();
        SetIsDamageViewToggle();
        StartCoroutine(InitLanguageDropdown());

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fpsLimitDropdown.onValueChanged.AddListener(SetFPS);
        screenModeToggle.onValueChanged.AddListener(OnClickScreenModeToggle);
        damageViewToggle.onValueChanged.AddListener(OnClickIsDamageViewToggle);
    }

    /// <summary>
    /// 현재 사용 가능한 해상도 목록을 드롭다운에 표시
    /// 현재 적용된 해상도를 건택 상태로 맞춤
    /// </summary>
    private void SetResolutionDropDown()
    {
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < Managers.Graphic.ResolutionSizes.Count; i++)
        {
            ResolutionSize re = Managers.Graphic.ResolutionSizes[i];
            string option = $"{re.size.x} X {re.size.y}";
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(Managers.Graphic.CurrentSize.uid);
        resolutionDropdown.RefreshShownValue();
    }

    /// <summary>
    /// 현재 사용 가능한 FPS 제한 목록을 드롭다운에 표시
    /// 현재 적용된 FPS index를 선택 상태로 맞춤
    /// </summary>
    private void SetFPSDropdown()
    {
        fpsLimitDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < Managers.Graphic.FPS.Count; i++)
        {
            int fps = Managers.Graphic.FPS[i];
            string option = $"{fps}";
            options.Add(option);
        }

        fpsLimitDropdown.AddOptions(options);
        fpsLimitDropdown.SetValueWithoutNotify(Managers.Graphic.CurrentFPS);
        fpsLimitDropdown.RefreshShownValue();
    }

    /// <summary>
    /// 현재 화면 모드 상태를 토글 UI에 반영
    /// </summary>
    private void SetIsScreenToggle()
    {
        screenModeToggle.SetIsOnWithoutNotify(Managers.Graphic.IsWindowMode);
    }

    /// <summary>
    /// 현재 데미지 텍스트 표시 상태를 토글 UI에 반영
    /// </summary>
    private void SetIsDamageViewToggle()
    {
        damageViewToggle.SetIsOnWithoutNotify(Managers.Graphic.IsShowDamageText);
    }

    private void ApplyLanguage(Locale locale, bool save)
    {
        if (locale == null)
            return;

        Managers.Local.ApplyLocale(locale);
        Managers.Graphic.SetLanguageLocaleCode(locale.Identifier.Code);

        if (save)
            Managers.Save.MarkGraphicDirty();

    }

    private void OnChangedLanguageDropdow(int index)
    {
        if (isSettingLanguageDropdown)
            return;

        if (index < 0 || index >= availableLocales.Count)
            return;

        ApplyLanguage(availableLocales[index], true);
    }

    private void SetLanguageDropdownValue()
    {
        Locale selectedLocale = LocalizationSettings.SelectedLocale;
        int index = availableLocales.IndexOf(selectedLocale);

        if (index < 0)
            index = 0;

        isSettingLanguageDropdown = true;
        languageDropDown.SetValueWithoutNotify(index);
        languageDropDown.RefreshShownValue();
        isSettingLanguageDropdown = false;
    }

    private string GetLocalDisplayName(Locale locale)
    {
        if (locale == null)
            return string.Empty;

        if (!string.IsNullOrEmpty(locale.LocaleName))
            return locale.LocaleName;

        return locale.Identifier.Code;
    }

    private IEnumerator InitLanguageDropdown()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (languageDropDown == null)
            yield break;

        availableLocales.Clear();
        availableLocales.AddRange(LocalizationSettings.AvailableLocales.Locales);

        languageDropDown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < availableLocales.Count; i++)
        {
            options.Add(GetLocalDisplayName(availableLocales[i]));
        }

        languageDropDown.AddOptions(options);

        SetLanguageDropdownValue();

        languageDropDown.onValueChanged.RemoveListener(OnChangedLanguageDropdow);
        languageDropDown.onValueChanged.AddListener(OnChangedLanguageDropdow);
    }

    /// <summary>
    /// 드롭 다운에서 선택한 해상도를 적용
    /// </summary>
    /// <param name="index"></param>
    public void SetResolution(int index)
    {
        Managers.Graphic.SetResolutionByUID(index);
        Managers.Save.MarkGraphicDirty();
    }

    /// <summary>
    /// 드롭 다운에서 선택한 FPS 제한을 적용
    /// </summary>
    /// <param name="index"></param>
    public void SetFPS(int index)
    {
        Managers.Graphic.SetFPS(index);
        Managers.Save.MarkGraphicDirty();
    }

    /// <summary>
    /// 전체화면 / 창모드 설정 적용
    /// </summary>
    /// <param name="value"></param>
    public void OnClickScreenModeToggle(bool value)
    {
        Managers.Graphic.SetScreenMode(value);
        Managers.Save.MarkGraphicDirty();
    }

    /// <summary>
    /// 데이지 텍스트 표시 여부 적용
    /// </summary>
    /// <param name="value"></param>
    public void OnClickIsDamageViewToggle(bool value)
    {
        Managers.Graphic.SetShowDamageText(value);
        Managers.Save.MarkGraphicDirty();
    }

    /// <summary>
    /// 그래픽 옵션을 기본값으로 되돌리고 UI를 다시 갱신
    /// 외부 Button에서 호출
    /// </summary>
    public void ResetOptions()
    {
        Managers.Graphic.ResetOptions();
        SetResolutionDropDown();
        SetFPSDropdown();
        SetIsScreenToggle();
        SetIsDamageViewToggle();
    }

    /// <summary>
    /// 데이터 저장, 외부 확인 Button에서 호출
    /// </summary>
    public void SaveGraphicOption()
    {
        Managers.Save.SaveGraphicData();
    }
}
