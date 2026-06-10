using UnityEngine;

public class OnButtonClick : MonoBehaviour
{
    [SerializeField]
    private StageManager stage;

    public void OnClickGameStartButton()
    {
        stage.WaveStart();
    }
}
