using UnityEngine;
using MyUtils.StateMachine;

public class EnemyBaseState : State<EnemyState>
{
    public virtual void OnHit(float damage, Vector3 knockback)
    { 
        Debug.Log("Enemy has been hit for " + damage);
    }
}

