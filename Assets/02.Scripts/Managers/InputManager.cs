using UnityEngine;

public class InputManager
{
    private readonly KeyData keyData = new KeyData();

    public void Init()
    {
        keyData.ResetKeyCodes();
    }

    public KeyCode GetKeyCode(InputAction key)
    {
        return keyData.GetKeyCode(key);
    }

    public bool GetKeyDown(InputAction key)
    {
        KeyCode code = keyData.GetKeyCode(key);
        return code != KeyCode.None && Input.GetKeyDown(code);
    }

    public bool KeyChange(InputAction key, KeyCode newCode)
    {
        if (keyData.ContainsValue(key, newCode))
            return false;

        keyData.SetKeyCode(key, newCode);
        return true;
    }

    public void ResetKeyCode()
    {
        keyData.ResetKeyCodes();
    }
}
