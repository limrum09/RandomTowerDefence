using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonClick : MonoBehaviour
{
    [SerializeField]
    private StageManager stage;
    public void OnClickGameStartButton()
    {
        stage.StageStart();
    }
}
