using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using System;

public class PlayerGetUpState : PlayerBaseState
{
    public PlayerGetUpState(ThirdPersonController owner) : base(owner)
    {

    }

    public override void Update(ThirdPersonController owner)
    {
        owner.JumpAndGravity();
        owner.GroundedCheck();

        // Flash the model for invulnerability?
    }

    public override void Transition()
    {
        player.stateStack.ChangeState(player.CombatState);
    }
}
