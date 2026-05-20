using System;
using TMPro;
using UnityEngine;

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

    private void TryApplyInput(string s)
    {
        if (string.IsNullOrEmpty(s))
            return;

        if (!Enum.TryParse(s, true, out KeyCode key))
            return;

        if (!Managers.InputData.KeyChange(action, key))
        {
            inputField.SetTextWithoutNotify(currentChar);
            return;
        }            

        SetInputActionText();
        Managers.Save.MarkInputDirty();
    }

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

    public void SetInputActionText()
    {
        currentChar = Managers.InputData.GetKeyCode(action).ToString();
        inputNameText.text = Managers.Local.GetString("INPUT_ACTION_" + action.ToString().ToUpper());
        inputField.SetTextWithoutNotify(currentChar);
    }
}
