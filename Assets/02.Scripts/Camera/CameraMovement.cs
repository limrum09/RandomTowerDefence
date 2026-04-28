using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraMovement : MonoBehaviour
{
    [Header("Border")]
    [SerializeField]
    private Vector2 min;
    [SerializeField]
    private Vector2 max;

    [Header("Zoom")]
    [SerializeField]
    private float zoomMin;
    [SerializeField]
    private float zoomMax;
    [SerializeField]
    private float zoomSpeed;

    [Header("Drag")]
    [SerializeField]
    private Camera mainCamera;

    private Vector3 dragStartWorldPosition;
    private void LateUpdate()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
            return;

        CameraZoomAndOut();

        if (Input.GetMouseButton(0))
            CameraDrag();

        ClampCamera();
    }
    private void CameraZoomAndOut()
    {
        float value = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

        mainCamera.orthographicSize -= value;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, zoomMin, zoomMax);
    }

    private void CameraDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            return;
        }

        Vector3 currentWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 drag = dragStartWorldPosition - currentWorldPos;

        transform.position += drag;
    }

    private void ClampCamera()
    {
        Vector3 pos = transform.position;

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        float minX = min.x + halfWidth;
        float maxX = max.x - halfWidth;
        float minY = min.y + halfHeight;
        float maxY = max.y - halfHeight;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
