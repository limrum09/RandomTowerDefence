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
        if (currentTarget == null)
        {
            tower.Attack(false);
            return;
        }

        if (!IsTargetValid(currentTarget))
        {
            currentTarget = null;
            tower.Attack(false);
            return;
        }

        attackTimer += Time.deltaTime;

        float attackCoolTime = 1f / tower.CurrentAtkSpeed;
        if (attackCoolTime > attackTimer)
            return;

        attackTimer = 0f;
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

        float dirX = currentTarget.transform.position.x - transform.position.x;

        if(dirX > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
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
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, tower.AtkRange, enemyLayer);

        Enemy nearEnemy = null;
        float nearDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (!IsTargetValid(enemy))
                continue;

            float distance = (enemy.transform.position - transform.position).sqrMagnitude;
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

        //if(enemy.isdead)

        return true;
    }

    /// <summary>
    /// 지정한 위치가 현재 타워 사거리 안 인지 확인
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    private bool IsInRange(Vector3 targetPosition)
    {
        float sqrDistance = (targetPosition - transform.position).sqrMagnitude;
        float sqrRange = tower.AtkRange * tower.AtkRange;
        return sqrRange >= sqrDistance;
    }
}
