using UnityEngine;
using UnityEngine.UI;

public class SettingUIController : MonoBehaviour
{
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

    private void Start()
    {
        
    }

    private void ShowPanel(GameObject target)
    {
        graphicPanel.SetActive(target == graphicPanel);
        soundPanel.SetActive(target == soundPanel);
        inputPanel.SetActive(target == inputPanel);
    }
}
