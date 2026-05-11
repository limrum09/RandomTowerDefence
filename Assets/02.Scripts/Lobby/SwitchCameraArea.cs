using UnityEngine;

/// <summary>
/// 카메라 이동을 하기위해 플레이어를 감지하는 오브젝트
/// 카메라가 이동하는 방향을 정한다
/// </summary>
public class SwitchCameraArea : MonoBehaviour
{
    [SerializeField]
    private CameraSwitcher switcher;
    [SerializeField]
    private MoveCameraDir dir;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switcher.MoveCamera(dir);
        }
    }
}
