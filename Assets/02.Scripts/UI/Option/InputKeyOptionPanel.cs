using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 단축키 설정 옵션 UI
/// InputAction enum에 정의된 모든 입력 액션에 대해 변경 UI를 생성
/// 키 저장/초기화/변경을 처리
/// </summary>
public class InputKeyOptionPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private ChangedInputPanel prefab;

    [Header("Panels")]
    [SerializeField]
    private GameObject resetCheckPanel;

    private List<ChangedInputPanel> panels = new List<ChangedInputPanel>();

    /// <summary>
    /// 모든 InputAction에 대한 키 변경 패넝을 생성하고 초기화
    /// </summary>
    public void Init()
    {
        foreach(InputAction e in Enum.GetValues(typeof(InputAction)))
        {
            ChangedInputPanel newPanel = Instantiate(prefab, content);
            
            newPanel.SetInputPanel(e);
            panels.Add(newPanel);
        }

        content.sizeDelta = new Vector2(content.anchoredPosition.x, 80.0f + (panels.Count * 80.0f));

        resetCheckPanel.SetActive(false);
    }

    /// <summary>
    /// 모든 단축키를 기본값으로 되돌리고 UI 표시 갱신
    /// </summary>
    public void ResetInputAction()
    {
        Managers.InputData.ResetKeyCode();

        foreach(var panel in panels)
        {
            panel.SetInputActionText();
        }
    }

    /// <summary>
    /// 현재 단축키 저장
    /// </summary>
    public void SaveInputKeyOption()
    {
        Managers.Save.SaveInputKeyData();
    }
}
