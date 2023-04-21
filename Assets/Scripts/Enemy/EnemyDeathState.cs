using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;

public class EnemyDeathState : EnemyBaseState
{
    public override void Enter(EnemyState owner, ArrayList data)
    {
        Debug.Log("Entered Death state");
        owner.animator.SetBool("attack", false);
        owner.animator.SetBool("correctAngle", false);
        owner.animator.SetFloat("speed", 0);
        owner.animator.SetTrigger("Death");
    }

    public override void Update(EnemyState owner)
    {

    }

    public override void Exit(EnemyState owner)
    {
        
    }
}