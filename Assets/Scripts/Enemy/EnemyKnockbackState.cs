using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;

public class EnemyKnockbackState : EnemyBaseState
{
    private Vector3 _knockbackDirection;
    private Vector3 _movement;
    private float _verticalStart = 8;
    private float _speed;
    private float _gravity;
    private float _timer;
    private float _animationLength;
    private EntityStats _stats;

    public override void Enter(EnemyState owner, ArrayList data)
    {
        // Assert that the first item in Data is a Vector3 with knockback info
        if (data[0] is Vector3)
        {
            _knockbackDirection = (Vector3)data[0];
        }
        else 
        {
            throw new ArgumentException("First argument of Data must be Vector3");
        }
        if (owner.hasAnimator){
            owner.animator.SetTrigger("Knockback");
        }
        this._speed = 5f;
        this._stats = owner.stats;
        this._gravity = _stats.Gravity;
        _movement = _knockbackDirection * _speed;
        _movement.y = _verticalStart;
        owner.verticalVelocity = _verticalStart;
        owner.controller.Move(_movement * Time.deltaTime);
    }

    public override void Update(EnemyState owner)
    {
        _movement = _knockbackDirection * _speed;
        _movement.y = owner.verticalVelocity;
        owner.controller.Move(_movement * Time.deltaTime);

        if (owner.grounded)
        {
            // Assume this state has been pushed to the state stack
            owner.stateStack.Pop();
        }


    }

    public override void Exit(EnemyState owner)
    {
        
    }

    public override void OnHit(float damage, Vector3 knockback)
    {
        // We can still take damage but not be knocked back again during knockback
        // Might change this depending on gameplay experience
        base.OnHit(damage, knockback);
        _stats.Health -= damage;
    }

}