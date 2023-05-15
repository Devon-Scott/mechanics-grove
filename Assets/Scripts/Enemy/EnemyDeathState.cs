using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;

public class EnemyDeathState : EnemyBaseState
{
    private float enterTime;
    public override void Enter(Enemy owner, ArrayList data)
    {
        owner.animator.SetBool("attack", false);
        owner.animator.SetBool("correctAngle", false);
        owner.animator.SetFloat("speed", 0);
        owner.animator.SetTrigger("Death");
        enterTime = 0f;
    }

    public override void Update(Enemy owner)
    {
        // Safeguard for an enemy simply being frozen in death state without animation being triggered
        enterTime += Time.deltaTime;
        if (enterTime >= 5.0f)
        {
            owner.onDeath();
        }
    }

    public override void Exit(Enemy owner)
    {
        
    }
}