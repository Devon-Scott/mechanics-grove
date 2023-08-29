using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using MyUtils.StateMachine;


public class PlayerCombatState : PlayerBaseState
{

    private float _swapCooldown;
    public PlayerCombatState(ThirdPersonController owner) : base(owner)
    {

    }
    
    public override void Enter(ThirdPersonController owner, ArrayList data = null){
        // Has no effect if the object is null
        ThirdPersonController.Destroy(owner._currentWeapon);
        owner._currentWeapon = ThirdPersonController.Instantiate(owner.weapons[0], owner.rightHandObject.transform);
        owner._currentOffHand = ThirdPersonController.Instantiate(owner.offHands[0], owner.leftHandObject.transform);
        if (owner._hasAnimator)
        {
            owner._animator.SetBool("Combat", true);
            owner._animator.SetBool("Build", false);
        }
        _swapCooldown = 1f;
    }

    public override void Update(ThirdPersonController owner){
        base.Update(owner);
        if (_input.build && _swapCooldown <= 0)
        {
            owner.stateStack.ChangeState(owner.BuildState);
        }
        
        if (_swapCooldown > 0)
        {
            _swapCooldown -= Time.deltaTime;
        }
    }

    public override void Exit(ThirdPersonController owner){

    }
}
