using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    public event Action OnGameStart;
    public event Action OnAfterSettingsInit;

    
    public void GameStart()
    {
        OnGameStart?.Invoke();
    }

    public void AfterSettingsInit()
    {
        OnAfterSettingsInit?.Invoke();
    }
}
