using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;

public class PlayerHitState : PlayerBaseState
{
    Vector3 _knockbackDirection;
    Vector3 _flattenedKnockback;
    
    public PlayerHitState(ThirdPersonController owner) : base(owner)
    {

    }

    // Same Behaviour as Knockback state, only we just flinch, instead of getting knocked back
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
            owner._animator.SetBool(owner._animIDHit, true);
        }
        // Rotate character to face knockback direction
        _flattenedKnockback = _knockbackDirection;
        _flattenedKnockback.y = 0;
        Quaternion rotation = Quaternion.LookRotation(-_flattenedKnockback);
        owner.transform.rotation = rotation;
    }

    public override void Update(ThirdPersonController owner)
    {
        // No controller input, but also no movement
        owner._animator.SetBool(owner._animIDHit, false);
        owner.JumpAndGravity();
        owner.GroundedCheck();
        // Flash the model for invulnerability?
    }

    public override void Transition()
    {
        player.stateStack.ChangeState(player.CombatState);
    }
}