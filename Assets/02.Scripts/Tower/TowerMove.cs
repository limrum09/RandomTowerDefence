using UnityEngine;

public class TowerMove : MonoBehaviour
{
    // 타워가 현재 위치한 그리드 정보를 참조하기 위한 변수
    private GridManager grid;

    /// <summary>
    /// 타워 위치 확인 및 이동에 필요한 초기 설정
    /// 처음 생성될 때, TowerController에서 StageManager를 받아 GridManager를 저장
    /// </summary>
    /// <param name="getStage"> 현제 스테이지 정보를 가지고있는 StageManager </param>
    public void SetTowerInit(StageManager getStage)
    {
        grid = getStage.Grid;
    }

    /// <summary>
    /// 전달 받은 그리드 좌표로 타워의 월드 위치를 변경
    /// 그리드의 셀 중심 좌표를 계산하여 타워를 배치
    /// </summary>
    /// <param name="pos"> 이동시킬 목표 셀 좌표 </param>
    public void SetTowerPosition(Vector2Int pos)
    {
        transform.position = grid.CellToWorldCenter(pos.x, pos.y);
    }

    /// <summary>
    /// 현재 타워의 월드 위치를 기준으로 그리드 좌표를 변환
    /// grid가 아직 초기화 되지 않았다면 기본값을 반환
    /// </summary>
    /// <returns>타워가 위치한 셀 좌표</returns>
    public Vector2Int GetTowerPosition()
    {
        if (grid == null)
            return Vector2Int.zero;

        return grid.WorldToCell(transform.position);
    }
}
