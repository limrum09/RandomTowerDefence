using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 입력 관련 기능을 관리
/// 키 설정 조회, 변경 초기화
/// 마우스 월드좌표 계산
/// 마우스 위치를 Grid Cell 좌표로 변환
/// UI위 클릭 여부 검사
/// 마우스로 클릭한 2D 오브젝트/컴포넌트 탐색
/// </summary>
public class InputManager
{
    // 키설정 데이터
    private readonly KeyData keyData = new KeyData();
    //UI Raycast결과를 재사용하기 위한 리스트, 매번 새로 만들지 않기위해 static으로 선언
    private static readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

    /// <summary>
    /// 초기화
    /// </summary>
    public void Init()
    {
        keyData.ResetKeyCodes();
    }

    /// <summary>
    /// 지정한 입력 액션에 연결된 keycode 반환
    /// </summary>
    /// <param name="key">조회할 입력 액션</param>
    /// <returns></returns>
    public KeyCode GetKeyCode(InputAction key)
    {
        return keyData.GetKeyCode(key);
    }

    /// <summary>
    /// 지정한 키가 현재 프레임에 눌렸는지 확인
    /// </summary>
    /// <param name="key">확인할 입력 액션</param>
    /// <returns>키가 눌렸으면 true 반환</returns>
    public bool GetKeyDown(InputAction key)
    {
        KeyCode code = keyData.GetKeyCode(key);
        return code != KeyCode.None && Input.GetKeyDown(code);
    }

    /// <summary>
    /// 특정 입력 액션의 키 설정을 변경
    /// 이미 다른 액션에서 사용중인 키 값이라면 변경하지 않는다
    /// </summary>
    /// <param name="key">변경하고 싶은 액션</param>
    /// <param name="newCode">새로운 KeyCode</param>
    /// <returns>변경 성공 여부 </returns>
    public bool KeyChange(InputAction key, KeyCode newCode)
    {
        // 이미 사용중인 키라면 중복 방지를 위해 실패
        if (keyData.ContainsValue(key, newCode))
            return false;

        keyData.SetKeyCode(key, newCode);
        return true;
    }

    /// <summary>
    /// 키코드를 기본값으로 리셋
    /// </summary>
    public void ResetKeyCode()
    {
        keyData.ResetKeyCodes();
    }

    /// <summary>
    /// 현재 마우스 위치를 월드 좌표로 변환
    /// </summary>
    /// <param name="camera">마우스 클릭을 감시할 카메라</param>
    /// <returns>클릭한 마우스의 월드 좌표를 반환</returns>
    public Vector3 GetMouseWorldPosition(Camera camera)
    {
        Vector3 mouseWorld = camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        return mouseWorld;
    }

    /// <summary>
    /// 현재 마우스 위치를 Grid Cell 좌표로 변환
    /// </summary>
    /// <param name="camera">좌표 변환에 사용할 카메라</param>
    /// <param name="grid">월드 좌표를 셀 좌표로 변환할 GridManager</param>
    /// <returns>마우스가 위치한 Grid Cell 좌표</returns>
    public Vector2Int GetMouseCellPosition(Camera camera, GridManager grid)
    {
        Vector3 mouseWorld = GetMouseWorldPosition(camera);
        return grid.WorldToCell(mouseWorld);
    }

    /// <summary>
    /// 현제 마우스 포인터가 특정 UI 컴포넌트 위에 있는지 확인
    /// </summary>
    /// <typeparam name="T">검사할 UI 컴포넌트의 타입</typeparam>
    /// <returns>해당 UI위에 있으면 true</returns>
    public bool IsPointerOverUI<T>() where T : Component
    {
        // EventSyetem이 없다면 UI Raycast 불가
        if (EventSystem.current == null)
            return false;

        // 현재 마우스 위치를 기준으로 PointerEventData 생성
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        // 이전 결과 제거
        raycastResults.Clear();
        // 현재 마우스 위치 아래의 모든 UI 요소 탐색
        EventSystem.current.RaycastAll(pointer, raycastResults);

        foreach (var re in raycastResults)
        {
            // 결과 오브젝트가 없으면 무시
            if (re.gameObject == null)
                continue;

            // 클릭된 UI오브젝트 중, 부모중에 T 컴포넌트가 있으면 true
            if (re.gameObject.GetComponentInParent<T>() != null)
                return true;

        }

        return false;
    }

    /// <summary>
    /// 현재 마우스 위치에 있는 2D Collider 반환
    /// </summary>
    /// <param name="camera">좌표 변환에 사용할 카메라</param>
    /// <returns>마우스 위치에 있는 Collider2D, 없으면 null 반환</returns>
    public Collider2D GetMouseOvelap2D(Camera camera)
    {
        Vector3 mouseWorld = GetMouseWorldPosition(camera);
        Vector2 point = new Vector2(mouseWorld.x, mouseWorld.y);

        return Physics2D.OverlapPoint(point);
    }

    /// <summary>
    /// 현재 마우스 위치에 있는 2D 오브젝트에서 특정 컴포넌트를 가져온다.
    /// </summary>
    /// <typeparam name="T">가져올 컴포넌트 타입</typeparam>
    /// <param name="camera">좌표변환에 사용할 카메라</param>
    /// <param name="component">탐색된 컴포넌트</param>
    /// <returns></returns>
    public bool TryGetMouseComponent<T> (Camera camera, out T component) where T : Component
    {
        component = null;

        Collider2D hit = GetMouseOvelap2D(camera);

        if (hit == null)
            return false;

        component = hit.GetComponent<T>();
        return component != null;
    }

    public bool IsMouseInsideCameraView(Camera camera)
    {
        if(camera == null) 
            return false;

        Vector3 viewPos = camera.ScreenToViewportPoint(Input.mousePosition);

        return viewPos.x >= 0f && viewPos.x <= 1f && viewPos.y >= 0f && viewPos.y <= 1f && viewPos.z >= 0f && viewPos.z <= 1f;
    }
}
