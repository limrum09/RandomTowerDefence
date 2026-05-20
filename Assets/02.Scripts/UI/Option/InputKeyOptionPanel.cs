using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InputKeyOptionPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform content;
    [SerializeField]
    private ChangedInputPanel prefab;

    [Header("Panels")]
    [SerializeField]
    private GameObject resetCheckPanel;

    [Header("Local Texts")]
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI resetButtonText;
    [SerializeField]
    private TextMeshProUGUI applyButtonText;
    [SerializeField]
    private TextMeshProUGUI OKButtonText;
    [SerializeField]
    private TextMeshProUGUI resetCheckText;
    [SerializeField]
    private TextMeshProUGUI resetCancelText;
    [SerializeField]
    private TextMeshProUGUI resetOKText;

    private List<ChangedInputPanel> panels = new List<ChangedInputPanel>();

    public void Init()
    {
        foreach(InputAction e in Enum.GetValues(typeof(InputAction)))
        {
            ChangedInputPanel newPanel = Instantiate(prefab, content);
            
            newPanel.SetInputPanel(e);
            panels.Add(newPanel);
        }

        content.sizeDelta = new Vector2(content.anchoredPosition.x, 80.0f + (panels.Count * 80.0f));

        title.text = Managers.Local.GetString("TEXT_SETTING_SHORTCUTKEY");
        resetButtonText.text = Managers.Local.GetString("BUTTON_RESET");
        applyButtonText.text = Managers.Local.GetString("BUTTON_APPLY");
        OKButtonText.text = Managers.Local.GetString("BUTTON_OK");
        resetCheckText.text = Managers.Local.GetString("TEXT_RESET_CHECK");
        resetCancelText.text = Managers.Local.GetString("BUTTON_CANCEL");
        resetOKText.text = Managers.Local.GetString("BUTTON_OK");

        resetCheckPanel.SetActive(false);
    }

    public void ResetInputAction()
    {
        Managers.InputData.ResetKeyCode();

        foreach(var panel in panels)
        {
            panel.SetInputActionText();
        }
    }
}
