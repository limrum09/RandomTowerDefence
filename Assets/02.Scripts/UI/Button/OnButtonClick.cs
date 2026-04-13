using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnButtonClick : MonoBehaviour
{
    public void OnClickGameStartButton()
    {
        Managers.Game.GameStart();
    }
}
