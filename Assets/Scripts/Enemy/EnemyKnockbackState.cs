using UnityEngine;
using MyUtils.StateMachine;

public class EnemyKnockbackState : EnemyBaseState
{
    public override void Enter(EnemyState owner)
    {

    }

    public override void Update(EnemyState owner)
    {

    }

    public override void Exit(EnemyState owner)
    {
        
    }

    public override void OnHit(float damage, Vector3 knockback)
    {
        base.OnHit(damage, knockback);
    }

}