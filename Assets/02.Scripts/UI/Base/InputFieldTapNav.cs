using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Input Field에서 Tap, Enter, Esc를 입력받아 동작
/// Tap으로 다음 InputField로 넘어가고,
/// Enter로 등록되어 있는 Button이 동작하고,
/// Esc로 등록되어 있는 Exit Button이 동작한다
/// </summary>
public class InputFieldTapNav : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button enterButton;
    [SerializeField]
    private Button exitButton;

    [Header("Input Fields")]
    [SerializeField]
    private TMP_InputField[] inputFields;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if(enterButton != null)
                enterButton.onClick.Invoke();

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (exitButton != null)
                exitButton.onClick.Invoke();

            return;
        }

        if (!Input.GetKeyDown(KeyCode.Tab))
            return;

        GameObject selected = EventSystem.current.currentSelectedGameObject;

        for(int i = 0; i < inputFields.Length; i++)
        {
            if (inputFields[i] != null && inputFields[i].gameObject == selected)
            {
                int nextIndex = (i + 1) % inputFields.Length;

                inputFields[nextIndex].Select();
                inputFields[nextIndex].ActivateInputField();

                return;
            }
        }
    }
}
