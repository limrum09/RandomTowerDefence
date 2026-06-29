using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 설정 UI 전체를 관리하는 컨트롤러
/// 그래픽/사운드/단축키 패널 전환, 설정창 표시/숨김, 설정 전체 저장을 담당
/// </summary>
public class SettingUIController : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvas;

    [Header("Animation")]
    [SerializeField]
    private UIPopInAnimation anim;

    [Header("Toggles")]
    [SerializeField]
    private Toggle graphicToggle;
    [SerializeField]
    private Toggle soundToggle;
    [SerializeField]
    private Toggle inputToggle;

    [Header("GameObject")]
    [SerializeField]
    private GameObject graphicPanel;
    [SerializeField]
    private GameObject soundPanel;
    [SerializeField]
    private GameObject inputPanel;

    [Header("Scripts")]
    [SerializeField]
    private InputKeyOptionPanel inputKeyOption;

    private void Awake()
    {
        BindToggles();
    }
    private void Start()
    {
        inputKeyOption.Init();
    }

    /// <summary>
    /// 토글 선택시 해당하는 패널만 보이도록 이벤트를 바인드함
    /// </summary>
    private void BindToggles()
    {
        graphicToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                ShowPanel(graphicPanel);
            }
        });

        soundToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                ShowPanel(soundPanel);
            }
        });

        inputToggle.onValueChanged.AddListener(isOn =>
        {
            if (isOn)
            {
                ShowPanel(inputPanel);
            }
        });
    }

    /// <summary>
    /// 전당받은 패널만 활성화하고 나머지 패널은 비활설화
    /// </summary>
    /// <param name="target"></param>
    private void ShowPanel(GameObject target)
    {
        graphicPanel.SetActive(target == graphicPanel);
        soundPanel.SetActive(target == soundPanel);
        inputPanel.SetActive(target == inputPanel);
    }

    /// <summary>
    /// 설정찰 열기
    /// 기본으로 그래픽 탭을 선택
    /// </summary>
    public void ShowSettingsPanel()
    {
        graphicToggle.isOn = true;
        ShowPanel(graphicPanel);

        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;

        anim.Play();
    }

    /// <summary>
    /// 설정창 닫기
    /// CanvasGroup을 사용하기에 오브젝트는 활성화 상태를 유지
    /// </summary>
    public void HideSettingsPanel()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
        anim.ResetScale();
    }

    /// <summary>
    /// 설정 전체 저장
    /// </summary>
    public void SaveSettings()
    {
        Managers.Save.SaveGraphicData();
        Managers.Save.SaveSoundData();
        Managers.Save.SaveInputKeyData();
    }
}
