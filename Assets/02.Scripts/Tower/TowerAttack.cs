using System.Runtime.CompilerServices;
using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [SerializeField]
    private Tower tower;
    [SerializeField]
    private LayerMask enemyLayer;

    private Enemy currentTarget;
    private float attackTimer;

    void Update()
    {
        if (tower == null)
            return;

        UpdateTarget();
        TryAttack();
    }

    /// <summary>
    /// 현재 타겟을 유지, 필요 시 가장 가짜운 적으로 다시 탐색
    /// </summary>
    private void UpdateTarget()
    {
        if(!IsTargetValid(currentTarget))
        {
            currentTarget = null;
        }    

        if(currentTarget != null && !IsInRange(currentTarget.transform.position))
        {
            currentTarget = null;
        }

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
