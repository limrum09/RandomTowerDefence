using UnityEditor;
using UnityEngine;

/// <summary>
/// 로비에서 플레이어 이동 구현
/// 로비는 횡스크롤 이동이기에 좌, 우로만 이동한다
/// 키보드로 이동하는 것이 아닌, 마우스로만 이동
/// 마우스가 클릭한 지점의 x 좌표로 이동한다
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer spRenderer;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private float speed;
    
    private Camera userCamera;
    private bool isMove;
    Vector3 DesPos;
    private void Start()
    {
        transform.position = new Vector3(0f, transform.position.y, transform.position.z);
        userCamera = Camera.main;
        DesPos = transform.position;
        isMove = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SetMousePointDestination();
        }

        if (isMove)
        {
            MovePlayer();
        }
    }

    public void SetButtonDestination(float getPos)
    {
        DesPos = new Vector3(getPos, transform.position.y, transform.position.z);

        SetMoveStat();
    }

    private void SetMousePointDestination()
    {
        if(userCamera == null)
            userCamera = Camera.main;

        Vector3 des = Managers.InputData.GetMouseWorldPosition(userCamera);

        if (!Managers.InputData.IsMouseInsideCameraView(userCamera))
            return;

        if (Managers.InputData.IsPointerOverUI<TowerUIRaycastTarget>())
            return;

        DesPos = new Vector3(des.x, transform.position.y, transform.position.z);

        SetMoveStat();
    }

    private void SetMoveStat()
    {
        isMove = true;

        Vector3 dir = DesPos - transform.position;

        if (dir.x > 0)
            spRenderer.flipX = false;
        else
            spRenderer.flipX = true;

        anim.SetBool("IsMove", true);
    }

    private void MovePlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, DesPos, Time.deltaTime * speed);

        if(Vector3.Distance(transform.position, DesPos) < 0.05f)
        {
            anim.SetBool("IsMove", false);
            isMove = false;
        }
    }
}
