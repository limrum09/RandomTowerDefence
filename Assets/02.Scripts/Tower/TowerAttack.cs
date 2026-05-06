using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField]
    private Tower tower;                    // 공격 정보를 가진 참조를 할 Tower
    [SerializeField]
    private LayerMask enemyLayer;           // 타워가 공격할 Enemy의 Layer
    [SerializeField]
    private SpriteRenderer spriteRenderer;  // 타워 좌, 우 반전용 sprite renderer

    private Enemy currentTarget;            // 현제 타워가 공격랑 타겟
    private float attackTimer;              // 공격 쿨타임 계산용 타이머

    void Update()
    {
        // 타워가 없으면 공격하지 않음
        if (tower == null)
            return;

        // 현재 타겟 유지 또는 새로운 타겟 탐색
        UpdateTarget();
        // 타겟 위치에 따라 타워 좌, 우 방향 변경
        UpdateFacing();
        // 공격 가능한 타겟이면 공격
        TryAttack();
    }

    /// <summary>
    /// 현재 타겟을 유지, 필요 시 가장 가까운 적으로 다시 탐색
    /// </summary>
    private void UpdateTarget()
    {
        // 현재 타겟이 죽었거나 비활성화 시 제거
        if(!IsTargetValid(currentTarget))
        {
            currentTarget = null;
        }    
        // 현재 타겟이 사거리 밖으로 나간경우 제거
        if(currentTarget != null && !IsInRange(currentTarget.transform.position))
        {
            currentTarget = null;
        }
        // 타겟이 엇으면 사거리안에서 가장 가까운 적 탐색
        if (currentTarget == null)
        {
            currentTarget = FindNearEnemyInRange();
        }
    }

    /// <summary>
    /// 공격 속도에 맞추어서 적 타겟 공격
    /// </summary>
    private void TryAttack()
    {
        // 타겟이 없을 경우 공격 상태 해체
        if (currentTarget == null)
        {
            tower.Attack(false);
            return;
        }

        // 타겟이 죽었거나 비활성화기 초기화
        if (!IsTargetValid(currentTarget))
        {
            currentTarget = null;
            tower.Attack(false);
            return;
        }

        attackTimer += Time.deltaTime;

        // 공격 속도를 기준으로 공격 간격 계산
        float attackCoolTime = 1f / tower.CurrentAtkSpeed;

        // 공격 쿨타임 끝나지 않을 시 대기
        if (attackCoolTime > attackTimer)
            return;

        // 공격 타이머 초기화
        attackTimer = 0f;

        // 실제 공격
        AttackEnemy(currentTarget);
    }

    /// <summary>
    /// 현재 타겟 위치에 따라 타워의 좌, 우 방향 변경
    /// </summary>
    private void UpdateFacing()
    {
        // Sprite Renderer가 없을 시 표시 불가
        if (spriteRenderer == null)
            return;

        // 타겟이 없으면 마지막 방향 유지
        if (currentTarget == null)
            return;

        // 타워 기준 적의 x좌표 위치 차이 계산
        float dirX = currentTarget.transform.position.x - transform.position.x;

        // 오른쪽
        if(dirX > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        // 왼쪽
        else if (dirX < 0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// 타겟에게 데미지 입힘
    /// </summary>
    /// <param name="target"></param>
    private void AttackEnemy(Enemy target)
    {
        if (target == null)
            return;

        tower.Attack(true);
        target.EnemyGeTakeDamage(tower.CurrentDamage);
    }

    /// <summary>
    /// 범위 내 가장 가까운 적 탐색
    /// </summary>
    /// <returns></returns>
    private Enemy FindNearEnemyInRange()
    {
        // 현제 타워 위치를 기준으로 사거리 안의 Enemy Layer 탐색
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tower.AtkRange, enemyLayer);

        Enemy nearEnemy = null;
        float nearDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            // 탐색된 Collider에서 Enemy 컴포넌트 가져오기
            Enemy enemy = hit.GetComponent<Enemy>();

            // 공격 가능한 적 아니면 제외
            if (!IsTargetValid(enemy))
                continue;

            // 거리 비교
            float distance = (enemy.transform.position - transform.position).sqrMagnitude;

            // 가까운 적이 생기면 갱신
            if(distance < nearDistance)
            {
                nearDistance = distance;
                nearEnemy = enemy;
            }
        }

        return nearEnemy;
    }

    /// <summary>
    /// 적이 살아 있는지, 공격가능한 상태인지 확인
    /// </summary>
    /// <param name="enemy"></param>
    /// <returns></returns>
    private bool IsTargetValid(Enemy enemy)
    {
        if (enemy == null)
            return false;

        if (!enemy.gameObject.activeInHierarchy)
            return false;

        // dead가 생겨도 필요한지는 모르겠지만 일단 주석으로 추가
        //if(enemy.isdead)

        return true;
    }

    /// <summary>
    /// 지정한 위치가 현재 타워 사거리 안 인지 확인
    /// </summary>
    /// <param name="targetPosition">거리를 확인할 타겟의 위치</param>
    /// <returns></returns>
    private bool IsInRange(Vector3 targetPosition)
    {
        // 실제 거리의 제곱
        float sqrDistance = (targetPosition - transform.position).sqrMagnitude;
        // 사거리 제곱
        float sqrRange = tower.AtkRange * tower.AtkRange;
        // 실제 거리가 가서리보다 작다면 True
        return sqrRange >= sqrDistance;
    }
}
