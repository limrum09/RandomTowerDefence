using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// 해상도 옵션 구조체
/// uid는 런타임 드롭다운 선택용 임시 UID이며, 저장에는 사룔하지 않음
/// </summary>
public struct ResolutionSize
{
    public int uid;
    public Vector2Int size;

    public ResolutionSize(int getUid, int getWidth, int getHeight)
    {
        uid = getUid;
        size = new Vector2Int(getWidth, getHeight);
    }
}

/// <summary>
/// 그래픽 옵션 저장 데이터
/// 해상도는 환경마다 목록 순서가 달라질 수 있기네 UID가 아니라 width/height로 저장
/// fps는 실제 FPS값이 아닌 fpsList의 index값
/// </summary>
[Serializable]
public class GraphicSaveData
{
    public int width;
    public int height;
    public bool isWindowMode;
    public int fps;
    public bool isShowDamageText;
    public string languageLocaleCode;
}

/// <summary>
/// 그래픽 옵션을 관리
/// 해상도, 화면 모드, FPS 제한, 데미지 텍스트 표시 여부를 담당
/// </summary>
public class GraphicManager
{
    private readonly List<ResolutionSize> resolutionSizes= new List<ResolutionSize>();
    private Dictionary<int, ResolutionSize> resolutionDic = new Dictionary<int, ResolutionSize>();

    private readonly List<int> fpsList = new List<int>();

    private bool isWindowMode;
    private bool isShowDamageText;
    private ResolutionSize currentSize;
    private int currentFPS;
    private string languageLocaleCode = string.Empty;

    public bool IsWindowMode => isWindowMode;
    public bool IsShowDamageText => isShowDamageText;
    public ResolutionSize CurrentSize => currentSize;
    public int CurrentFPS => currentFPS;
    public IReadOnlyList<int> FPS => fpsList;
    public IReadOnlyList<ResolutionSize> ResolutionSizes => resolutionSizes;
    public string LanguageLocaleCode => languageLocaleCode;

    /// <summary>
    /// resolutioList에서 width/height가 일치하는 해상도 찾기
    /// 저장 데이터 로드 시 사용
    /// </summary>
    /// <param name="width">찾을 width값</param>
    /// <param name="height">찾을 height값</param>
    /// <param name="result">찾은 ResolutionSize, 알맞은게 없다면 null을 보낸다</param>
    /// <returns>찾았다면 true, 못찾으면 false를 반환</returns>
    private bool FindResolution(int width, int height, out ResolutionSize result)
    {
        foreach(var r in resolutionSizes)
        {
            if(r.size.x == width && r.size.y == height)
            {
                result = r;
                return true;
            }
        }

        result = default;
        return false;
    }

    /// <summary>
    /// 현재 시스플레이에서 지원하는 해상도와 FPS목록을 수집하고 기본 옵션으로 초기화
    /// </summary>
    public void Init()
    {
        resolutionSizes.Clear();
        resolutionDic.Clear();

        Resolution[] resolutions;
        resolutions = Screen.resolutions;

        int index = 0;
        HashSet<string> checkResolution = new HashSet<string>();
        HashSet<int> checkFPS = new HashSet<int>();
        for(int i = 0; i< resolutions.Length; i++)
        {
            string hashKey = $"{resolutions[i].width}x{resolutions[i].height}";

            if (checkResolution.Contains(hashKey))
                continue;

            checkResolution.Add(hashKey);

            ResolutionSize newSize = new ResolutionSize(index, resolutions[i].width, resolutions[i].height);
            resolutionSizes.Add(newSize);
            resolutionDic[newSize.uid] = newSize;
            index++;

            int fps = Mathf.RoundToInt((float)resolutions[i].refreshRateRatio.value);

            if (fps <= 0 || checkFPS.Contains(fps))
                continue;

            checkFPS.Add(fps);
            fpsList.Add(fps);
        }

        fpsList.Sort();
        fpsList.Add(-1);

        ResetOptions();
    }

    /// <summary>
    /// 그래픽 옵션을 기본값으로 돌린다
    /// 1920x1080을 기본 해상도로 삼고, 없으면 가장 큰 해상도를 사용
    /// </summary>
    public void ResetOptions()
    {
        SetScreenMode(false);
        SetShowDamageText(true);

        int fps = fpsList.Count > 3 ? fpsList.Count / 2 : 0;
        SetFPS(fps);

        if (resolutionSizes.Count == 0)
            return;

        ResolutionSize defaultSize = resolutionSizes.Find(x => x.size.x == 1920 && x.size.y == 1080);

        if (defaultSize.size.x == 1920 && defaultSize.size.y == 1080)
            SetResolutionByUID(defaultSize.uid);
        else
        {
            int index = resolutionSizes.Count - 1;
            SetResolutionByUID(resolutionSizes[index].uid);
        }
    }

    /// <summary>
    /// 드롭 다운 및 초기화에서 찾은 ResolutionSize의 uid를 받아 해상도르 적용
    /// uid는 런타임 내부 식별자
    /// </summary>
    /// <param name="uid"></param>
    public void SetResolutionByUID(int uid)
    {
        if (!resolutionDic.TryGetValue(uid, out ResolutionSize size))
            return;

        currentSize = size;

        FullScreenMode mode = isWindowMode ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

        Screen.SetResolution(currentSize.size.x, currentSize.size.y, mode);
    }

    /// <summary>
    /// FPS 목록 index를 기준으로 FPS 제한을 적용
    /// fpsList의 값이 -1이면 제학이 없음
    /// </summary>
    /// <param name="index"></param>
    public void SetFPS(int index)
    {
        if (index < 0 || index >= fpsList.Count)
            index = fpsList.Count - 1;

        currentFPS = index;
        Application.targetFrameRate = fpsList[currentFPS];
    }

    /// <summary>
    /// 화면 모드 설정
    /// true면 전체화면 false면 창모드
    /// </summary>
    /// <param name="value"></param>
    public void SetScreenMode(bool value)
    {
        isWindowMode = value;

        FullScreenMode mode = isWindowMode ? FullScreenMode.Windowed : FullScreenMode.FullScreenWindow;

        Screen.SetResolution(currentSize.size.x, currentSize.size.y, mode);
    }

    /// <summary>
    /// 데이지 텍스트 표시 여부
    /// </summary>
    /// <param name="value"></param>
    public void SetShowDamageText(bool value) => isShowDamageText = value;

    /// <summary>
    /// 언어 변경
    /// </summary>
    /// <param name="localeCode"></param>
    public void SetLanguageLocaleCode(string localeCode)
    {
        languageLocaleCode = localeCode;
    }

    /// <summary>
    /// 저장된 데이터 로드
    /// </summary>
    /// <param name="saveData"></param>
    public void LoadOptionSaveData(GraphicSaveData saveData)
    {
        if(saveData == null)
        {
            ResetOptions();
            return;
        }

        SetScreenMode(saveData.isWindowMode);
        SetShowDamageText(saveData.isShowDamageText);
        SetFPS(saveData.fps);
        SetLanguageLocaleCode(saveData.languageLocaleCode);

        if (FindResolution(saveData.width, saveData.height, out ResolutionSize result))
            SetResolutionByUID(result.uid);
        else if (resolutionSizes.Count > 0)
            SetResolutionByUID(resolutionSizes[resolutionSizes.Count - 1].uid);
    }

    /// <summary>
    /// 데이터 저장
    /// </summary>
    /// <returns></returns>
    public GraphicSaveData GetSaveData()
    {
        return new GraphicSaveData
        {
            width = currentSize.size.x,
            height = currentSize.size.y,
            isWindowMode = isWindowMode,
            fps = currentFPS,
            isShowDamageText = isShowDamageText,
            languageLocaleCode = languageLocaleCode,
        };
    }
}
