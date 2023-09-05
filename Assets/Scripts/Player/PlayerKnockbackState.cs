using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using MyUtils.StateMachine;

public class PlayerKnockbackState : PlayerBaseState
{
    Vector3 _knockbackDirection;
    Vector3 _flattenedKnockback;
    
    public PlayerKnockbackState(ThirdPersonController owner) : base(owner)
    {

    }

    public override void Enter(ThirdPersonController owner, ArrayList data)
    {
        if (data[0] is Vector3 && data[1] is float)
        {
            _knockbackDirection = (Vector3)data[0] * (float)data[1];
        }
        else 
        {
            throw new ArgumentException("First argument of Data must be Vector3, second must be float");
        }
        if (owner._hasAnimator)
        {
            owner._animator.SetBool(owner._animIDKnockback, true);
        }
        // Rotate character to face knockback direction
        _flattenedKnockback = _knockbackDirection;
        _flattenedKnockback.y = 0;
        Quaternion rotation = Quaternion.LookRotation(-_flattenedKnockback);
        owner.transform.rotation = rotation;
    }

    public override void Update(ThirdPersonController owner)
    {
        owner._animator.SetBool(owner._animIDKnockback, false);
        owner.JumpAndGravity();
        owner.GroundedCheck();
        owner._controller.Move(_flattenedKnockback * Time.deltaTime * 5);

        // Flash the model for invulnerability?
    }

    public override void Transition()
    {
        player.stateStack.ChangeState(player.GetUpState);
    }

    public override void OnHit(float damage, Vector3 knockback, float scalar)
    {
        // Intended to be invulnerable during this state, no additional knockback or damage
        // _stats.Health -= damage;
        // ArrayList data = new ArrayList();
        // data.Add(knockback);
        // data.Add(scalar);
        // if ((knockback * scalar).magnitude > _stats.knockbackThreshold)
        // {
        //     player.stateStack.ChangeState(player.KnockbackState, data);
        // }
        // else
        // {
        //     player.stateStack.ChangeState(player.HitState);
        // }
    }
}
