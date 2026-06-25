using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 단일 입력 액션의 키 변경 UI
/// InputAction의 이름을 표시하고 InputField에 입력된 한 글자를 KeyCode로 변환하여 저장
/// </summary>
public class ChangedInputPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI inputNameText;
    [SerializeField]
    private TMP_InputField inputField;

    private InputAction action;
    private bool isChanging;
    private string currentChar;
    private string lastChar;

    /// <summary>
    /// InputField 값이 변경될 때 호출
    /// 여러 글자가 입력되어도 마지막 한 글자만 남김
    /// </summary>
    /// <param name="s"></param>
    private void OnChangeInputAction(string s)
    {
        if (isChanging)
            return;

        if (string.IsNullOrEmpty(s))
            return;

        lastChar = s[^1].ToString().ToUpper();

        isChanging = true;
        inputField.text = lastChar;
        inputField.caretPosition = 1;
        isChanging = false;
    }

    /// <summary>
    /// 입력이 끝나면 현재 값을 KeyCode로 변환하고 저장
    /// 이미 다른 액션에서 사용 중인 키라면 기존 키로 되돌림
    /// </summary>
    /// <param name="s"></param>
    private void TryApplyInput(string s)
    {
        if (string.IsNullOrEmpty(s))
        {
            inputField.SetTextWithoutNotify(currentChar);
            return;
        }

        if (!Enum.TryParse(s, true, out KeyCode key))
        {
            inputField.SetTextWithoutNotify(currentChar);
            return;
        }

        if (!Managers.InputData.KeyChange(action, key))
        {
            inputField.SetTextWithoutNotify(currentChar);
            return;
        }            

        SetInputActionText();
        Managers.Save.MarkInputDirty();
        Managers.Save.SaveInputKeyData();
    }

    /// <summary>
    /// 이 패널이 담당할 입력 ㅎ액션을 설정하고 이벤트 연결
    /// </summary>
    /// <param name="input"></param>
    public void SetInputPanel(InputAction input)
    {
        isChanging = false;
        action = input;
        lastChar = string.Empty;

        inputField.onValueChanged.RemoveListener(OnChangeInputAction);
        inputField.onEndEdit.RemoveListener(TryApplyInput);

        inputField.onValueChanged.AddListener(OnChangeInputAction);
        inputField.onEndEdit.AddListener(TryApplyInput);

        SetInputActionText();
    }

    /// <summary>
    /// 현재 액션 이름과 등록된 KeyCode를 UId에 표시
    /// </summary>
    public void SetInputActionText()
    {
        currentChar = Managers.InputData.GetKeyCode(action).ToString();
        inputNameText.text = Managers.Local.GetString("OptionUI", "INPUT_ACTION_" + action.ToString().ToUpper());
        inputField.SetTextWithoutNotify(currentChar);
    }
}
