using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;

public class EnemyKnockbackState : EnemyBaseState
{
    private Vector3 _knockbackDirection;
    private Vector3 _movement;
    // give a guaranteed vertical kick during knockback
    private float _verticalStart = 8;
    private float _speed;
    private float _friction;
    private float _gravity;
    private float _timer;
    private float _animationLength;
    private bool _alive;
    private EnemyState _owner;
    private EntityStats _stats;

    public override void Enter(EnemyState owner, ArrayList data)
    {
        this._owner = owner;
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
        this._friction = 1f;
        this._stats = owner.stats;
        this._gravity = _stats.Gravity;
        this._alive = _stats.Health > 0;
        _movement = _knockbackDirection * _speed;
        _movement.y += _verticalStart;
        owner.verticalVelocity = _verticalStart;
        owner.controller.Move(_movement * Time.deltaTime);
    }

    public override void Update(EnemyState owner)
    {
        // Need to test how this looks and feels
        _speed = Math.Max(0, _speed - (_friction * Time.deltaTime));
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
        if (!_alive)
        {
            owner.stateStack.ChangeState(owner.DeathState);
        }
    }

    public override void OnHit(float damage, Vector3 knockback)
    {
        // We can still take damage but not be knocked back again during knockback
        // Except for very strong attacks
        // Might change this depending on gameplay experience
        base.OnHit(damage, _movement + knockback);
        if (_alive)
        {
            _stats.Health -= damage;
            if (knockback.magnitude > _stats.knockbackThreshhold * 3)
            {
                ArrayList data = new ArrayList();
                data.Add(_movement + knockback);
                _owner.stateStack.Push(_owner.KnockbackState, data);
            }
            else if (_stats.Health >= 0)
            {
                _alive = false;
            }
            else
            {
                // Flash to indicate damage
                // Play sound to indicate damage
            }
        }
    }

}