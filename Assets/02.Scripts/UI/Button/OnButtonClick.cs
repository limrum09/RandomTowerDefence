using TMPro;
using UnityEngine;

public class OnButtonClick : MonoBehaviour
{
    [SerializeField]
    private StageManager stage;
    [SerializeField]
    private TextMeshProUGUI text;

    private void Start()
    {
        text.text = Managers.Local.GetString("BUTTON_WAVE_START");
    }

    public void OnClickGameStartButton()
    {
        stage.WaveStart();
    }
}
