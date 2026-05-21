using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스테이지 옵션 UI를 관리하는 컨트롤러
/// 옵션 창 표시/숨김, 설정 패널 표시, 게임 계속하기 로비 이동 이벤트를 담당
/// </summary>
public class StageOptionUIController : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvas;

    [Header("Buttons")]
    [SerializeField]
    private Button continueButton;
    [SerializeField]
    private Button inputOptionButton;
    [SerializeField]
    private Button lobbyButton;
    [SerializeField]
    private Button lobbyOkButton;

    [Header("Game Objects")]
    [SerializeField]
    private GameObject frame;
    [SerializeField]
    private GameObject lobbyCheckPopup;

    [Header("Panels")]
    [SerializeField]
    private SettingUIController settingPanel;

    [Header("Prefab")]
    [SerializeField]
    private SettingUIController settingPanelPrefab;

    public event Action OnStageGameContinue;        // 게임 지속하기를 눌렀을 때 호출되는 이벤트
    public event Action OnMoveToLobby;              // 로비로 가기를 눌렀을 때 호출되는 이벤트

    private void Start()
    {
        HideOption();
    }

    private void OnEnable()
    {   
        // 옵션 UI가 활성화 되면 로비 이동 확인 팝업은 기본으로 숨김
        lobbyCheckPopup.SetActive(false);
    }

    /// <summary>
    /// 옵션 패널 표시
    /// </summary>
    public void ShowOptionPanel()
    {
        ShowOption();
    }

    /// <summary>
    /// 설정 패널 표시
    /// 설정 패널이 아직 생성되지 않았다면 프리팹을 생성
    /// </summary>
    public void ShowSettingPanel()
    {
        if(settingPanel == null)
        {
            settingPanel = Instantiate(settingPanelPrefab);
            settingPanel.transform.SetParent(frame.transform);
            settingPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        settingPanel.ShowSettingsPanel();
    }

    /// <summary>
    /// 옵션창을 닫고, 스테이지 게임을 재개
    /// </summary>
    public void StageGameContinue()
    {
        HideOption();
        OnStageGameContinue?.Invoke();
    }

    /// <summary>
    /// 로이 이동 이벤트 호출
    /// 씬 전환은 구독한 외부 객체에서 담당
    /// </summary>
    public void MoveToLobby()
    {
        OnMoveToLobby?.Invoke();
    }

    /// <summary>
    /// 옵션 UI를 표시하고 입력을 받을 수 있게 전환
    /// </summary>
    public void ShowOption()
    {
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    /// <summary>
    /// 옵션 UI를 숨기고 입력을 막음
    /// </summary>
    public void HideOption()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;

    }
}
