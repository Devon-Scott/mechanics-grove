using System.Collections;
using System.Collections.Generic;using UnityEngine;
using StarterAssets;
using MyUtils.StateMachine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

public class PlayerBaseState : State<ThirdPersonController>
{
    public ThirdPersonController player;
    public CharacterController _controller;
    public StarterAssetsInputs _input;
    public PlayerStats _stats;
    
 
    protected Vector3 gravityVelocity;
    protected Vector3 velocity;
    protected Vector2 input;
 
    // public InputAction Move;
    // public InputAction look;
    // public InputAction jump;
    // public InputAction sprint;
    // public InputAction drawWeapon;
    // public InputAction attack;

    public PlayerBaseState(ThirdPersonController owner){
        player = owner;
        _input = owner._input;
        _controller = owner._controller;
        _stats = owner.GetComponent<PlayerStats>();
    }

    public override void Update(ThirdPersonController owner)
    {
        player.JumpAndGravity();
        player.GroundedCheck();
        player.Move();
        player.Attack();
    }

    public void LateUpdate()
    {
        player.CameraRotation();
    }

    public virtual void OnHit(float damage, Vector3 knockback, float scalar)
    {
        _stats.Health -= damage;
        ArrayList data = new ArrayList();
        data.Add(knockback);
        data.Add(scalar);
        if ((knockback * scalar).magnitude > _stats.knockbackThreshold)
        {
            player.stateStack.ChangeState(player.KnockbackState, data);
        }
        else
        {
            player.stateStack.ChangeState(player.HitState, data);
        }
    }

    public void HandleEndOfCast()
    {

    }

    public virtual void Transition(){}
}
