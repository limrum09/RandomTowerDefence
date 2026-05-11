using UnityEngine;

public enum MoveCameraDir
{
    Left,
    Right
}

/// <summary>
/// 로비의 카메라는 좌, 우로만 이동하며 총 2개의 화면만 사용
/// Dotoween을 이용해 입력 받은 값으로 카메라의 좌, 우 움직임을 제어
/// </summary>
public class CameraSwitcher : MonoBehaviour
{
    [SerializeField]
    private ObjectMovement move;

    /// <summary>
    /// 카메라 이동 호출
    /// </summary>
    /// <param name="dir">이동하는 방향</param>
    public void MoveCamera(MoveCameraDir dir)
    {
        if(dir == MoveCameraDir.Left)
        {
            move.MoveToOrigin();
        }
        else if(dir == MoveCameraDir.Right)
        {
            move.MoveToTarget();
        }
    }
}
