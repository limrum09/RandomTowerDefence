using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public enum InputAction
{
    MoveTower,
    MakeTower,
    ShowGradeUpgradeTowerView,
    ShowStatUpgradeTowerView,
    TowerGradeNormalUpgrade,
    TowerGradePremiunUpgrade,
    TowerStatDamgeUpgrade,
    TowerStatAttackSpeedUpgrade,
    Buy,
    MakeObstacle,
    RemoveObstacle,
    Options
}

public class KeyData
{
    private Dictionary<InputAction, KeyCode> keys = new Dictionary<InputAction, KeyCode>();

    public void ResetKeyCodes()
    {
        keys[InputAction.MoveTower] = KeyCode.M;
        keys[InputAction.MakeTower] = KeyCode.N;
        keys[InputAction.ShowGradeUpgradeTowerView] = KeyCode.Z;
        keys[InputAction.ShowStatUpgradeTowerView] = KeyCode.X;
        keys[InputAction.TowerGradeNormalUpgrade] = KeyCode.E;
        keys[InputAction.TowerGradePremiunUpgrade] = KeyCode.D;
        keys[InputAction.TowerStatDamgeUpgrade] = KeyCode.W;
        keys[InputAction.TowerStatAttackSpeedUpgrade] = KeyCode.S;
        keys[InputAction.Buy] = KeyCode.B;
        keys[InputAction.MakeObstacle] = KeyCode.R;
        keys[InputAction.RemoveObstacle] = KeyCode.T;
        keys[InputAction.Options] = KeyCode.Escape;
    }

    public bool TryGetKeyCode(InputAction key, out KeyCode keyCode)
    {
        return keys.TryGetValue(key, out keyCode);
    }

    public KeyCode GetKeyCode(InputAction key)
    {
        if (keys.TryGetValue(key, out KeyCode keyCode))
            return keyCode;

        return KeyCode.None;
    }

    public void SetKeyCode(InputAction key, KeyCode keyCode)
    {
        keys[key] = keyCode;
    }

    public bool ContainsValue(InputAction key, KeyCode keyCode)
    {
        foreach (var pair in keys)
        {
            if (pair.Key != key && pair.Value == keyCode)
                return true;
        }

        return false;
    }
}
