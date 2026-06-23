using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 스테이지의 적 스폰지점과 목표 지점을 새로 생성하는 버튼
/// StageManager를 참조하며 게임이 시작하거나, 새로고침 횟수가 부족하다면 호출 불가
/// </summary>
public class RerollButtonClick : MonoBehaviour
{
    [SerializeField]
    private Button rollButton;
    [SerializeField]
    private TextMeshProUGUI text;

    private LocalizationDataManager local;
    private int rerollCnt;

    public event Action OnClickReroll;

    private void SetText()
    {
        text.text = $"{Managers.Local.GetString("Stage", "STAGE_TERRAIN_REFRESH")}({rerollCnt})";
    }

    private void OnDestroy()
    {
        if (local != null)
            local.OnLanguageChanged -= SetText;
    }

    public void OnClickedButton()
    {
        OnClickReroll?.Invoke();
    }

    public void SetRerollCnt(int cnt)
    {
        if(local == null)
        {
            local = Managers.Local;
            local.OnLanguageChanged += SetText;
        }   

        rerollCnt = cnt;
        SetText();

        if (cnt <= 0)
        {
            OffRerollButton();
        }
    }

    public void OffRerollButton()
    {
        rollButton.interactable = false;
        rollButton.gameObject.SetActive(false);
    }
}
