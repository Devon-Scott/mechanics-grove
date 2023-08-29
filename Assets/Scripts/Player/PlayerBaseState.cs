using UnityEngine;
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
    }

    public override void Update(ThirdPersonController owner)
    {
        player.JumpAndGravity();
        player.GroundedCheck();
        player.Move();
    }

    public void LateUpdate()
    {
        player.CameraRotation();
    }

    public void HandleEndOfCast()
    {
        
    }
}
