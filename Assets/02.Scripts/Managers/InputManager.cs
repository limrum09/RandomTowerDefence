using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    private readonly KeyData keyData = new KeyData();
    private static readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

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

    public Vector3 GetMouseWorldPosition(Camera camera)
    {
        Vector3 mouseWorld = camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        Debug.Log("마우스 이벤트 위치 : " + mouseWorld);
        return mouseWorld;
    }

    public Vector2Int GetMouseCellPosition(Camera camera, GridManager grid)
    {
        Vector3 mouseWorld = GetMouseWorldPosition(camera);
        return grid.WorldToCell(mouseWorld);
    }

    public bool IsPointerOverUI<T>() where T : Component
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        raycastResults.Clear();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        foreach (var re in raycastResults)
        {
            if (re.gameObject == null)
                continue;
            if (re.gameObject.GetComponentInParent<T>() != null)
                return true;
        }

        return false;
    }

    public Collider2D GetMouseOvelap2D(Camera camera)
    {
        Vector3 mouseWorld = GetMouseWorldPosition(camera);
        Vector2 point = new Vector2(mouseWorld.x, mouseWorld.y);

        return Physics2D.OverlapPoint(point);
    }

    public bool TryGetMouseComponent<T> (Camera camera, out T component) where T : Component
    {
        component = null;

        Collider2D hit = GetMouseOvelap2D(camera);

        if (hit == null)
            return false;

        component = hit.GetComponent<T>();
        return component != null;
    }
}
