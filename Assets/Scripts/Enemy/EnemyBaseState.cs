using UnityEngine;
using MyUtils.StateMachine;

public class EnemyBaseState : State<Enemy>
{
    public virtual void OnHit(float damage, Vector3 knockback, float scalar)
    { 
        Debug.Log("Enemy has been hit for " + damage);
        //Debug.Log("Enemy has been knocked back with vector " + (scalar * knockback).ToString());
    }
}

