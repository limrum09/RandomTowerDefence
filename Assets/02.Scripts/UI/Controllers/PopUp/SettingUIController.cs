using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
    [Header("Canvas Group")]
    [SerializeField]
    private CanvasGroup canvas;

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

    private void ShowPanel(GameObject target)
    {
        graphicPanel.SetActive(target == graphicPanel);
        soundPanel.SetActive(target == soundPanel);
        inputPanel.SetActive(target == inputPanel);
    }

    public void ShowSettinsPanel()
    {
        graphicToggle.isOn = true;
        ShowPanel(graphicPanel);

        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void HideSettingsPanel()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }



    private void Start()
    {
        inputKeyOption.Init();

        HideSettingsPanel();
    }
}
