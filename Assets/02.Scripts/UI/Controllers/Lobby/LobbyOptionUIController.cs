using UnityEngine;

/// <summary>
/// 로비 옵션 UI를 관리하는 컨트롤러
/// 옵션 패널 표시/숨김, 설정 UI 표시, 게임 종료 처리를 담당
/// </summary>
public class LobbyOptionUIController : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private SettingUIController settingUI;
    [SerializeField]
    private SettingUIController settingUIPrefab;

    /// <summary>
    /// 게임 종료 버튼에 호출
    /// 현재 데이터들을 저장한 뒤, 종료
    /// </summary>
    public void OnGameEnd()
    {
        Managers.Save.SaveAllData();
        Application.Quit();
    }
    /// <summary>
    /// 로비 옵션 패널을 화면에 표시하고 입력 받도록 상태 전환
    /// </summary>
    public void ShowOptionPanel()
    {
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    /// <summary>
    /// 로비 옵션 패널을 숨기고 입력을 막음
    /// </summary>
    public void HideOptionPanel()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }

    /// <summary>
    /// 설정 UI를 표시
    /// 설정 UI가 아직 생성되지 않았다면 프리팹을 생성하여 현재 오브젝트의 자식으로 배치
    /// </summary>
    public void ShowSettingUI()
    {
        if(settingUI == null)
        {
            settingUI = Instantiate(settingUIPrefab);
            settingUI.transform.SetParent(this.transform);
            settingUI.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        }

        settingUI.ShowSettingsPanel();
    }
}
