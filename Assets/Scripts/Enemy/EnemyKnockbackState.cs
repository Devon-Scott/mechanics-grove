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
    private Enemy _owner;
    private EntityStats _stats;

    public override void Enter(Enemy owner, ArrayList data)
    {
        this._owner = owner;
        // Assert that the first item in Data is a Vector3 with knockback info
        // Assert the second item is a float with scalar information
        if (data[0] is Vector3 && data[1] is float)
        {
            _knockbackDirection = (Vector3)data[0] * (float)data[1];
        }
        else 
        {
            throw new ArgumentException("First argument of Data must be Vector3, second must be float");
        }
        if (owner.hasAnimator){
            owner.animator.SetTrigger("Knockback");
        }
        this._speed = 5f;
        this._friction = 1f;
        this._stats = owner.stats;
        this._gravity = _stats.Gravity;
        this._alive = _stats.Health > 0;
        _knockbackDirection.y = Math.Abs(_knockbackDirection.y);
        _movement = _knockbackDirection * _speed;
        _movement.y += _verticalStart;
        owner.verticalVelocity = _verticalStart;
        owner.controller.Move(_movement * Time.deltaTime);
        owner.grounded = false;
        // Enemy.print(_movement.ToString());
    }

    public override void Update(Enemy owner)
    {
        // Need to test how this looks and feels
        _speed = Math.Max(0, _speed - (_friction * Time.deltaTime));
        _movement = _knockbackDirection * _speed;
        _movement.y = owner.verticalVelocity;
        owner.controller.Move(_movement * Time.deltaTime);

        if (owner.grounded)
        {
            // Assume this state has been pushed to the state stack
            if (!_alive)
            {
                owner.stateStack.ChangeState(owner.DeathState);
            }
            else
            {
                owner.stateStack.Pop();
            }
        }
    }

    public override void Exit(Enemy owner)
    {

    }

    public override void OnHit(float damage, Vector3 knockback, float scalar)
    {
        // We can still take damage but not be knocked back again during knockback
        // Except for very strong attacks
        // Might change this depending on gameplay experience
        if (_alive)
        {
            _stats.Health -= damage;
            if (knockback.magnitude > _stats.knockbackThreshhold * 3)
            {
                ArrayList data = new ArrayList();
                data.Add(_movement + knockback);
                data.Add(scalar);
                _owner.stateStack.Push(_owner.KnockbackState, data);
            }
            else if (_stats.Health <= 0)
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