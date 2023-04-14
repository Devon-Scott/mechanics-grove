using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;

public class EnemyAttackState : EnemyBaseState
{
    private Collider target;
    private Vector3 targetDirection;
    public bool hitboxActive;
    EnemyState owner;
    EntityStats stats;

    public override void Enter(EnemyState owner, ArrayList data)
    {
        this.owner = owner;
        hitboxActive = false;
        target = owner.target;
        if (owner.hasAnimator)
        {
            owner.animator.SetBool("attack", true);
        }
        stats = owner.stats;
    }

    public override void Update(EnemyState owner)
    {
        if (distanceTo(target.ClosestPoint(owner.transform.position)) >= 2.5f)
        {
            target = null;
        }
        if (target != null)
        {
            Vector3 positionAxis = owner.transform.position;
            positionAxis.y = 0;
            targetDirection = target.gameObject.transform.position - positionAxis;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime * 2f);
            // Ensure we're grounded
            Vector3 direction = new Vector3(0, owner.verticalVelocity, 0);
            owner.controller.Move(direction * Time.deltaTime);
            // Should probably detect if we're actually facing the player before we attack
            float angleToPlayer = Vector3.Angle(owner.transform.forward, targetDirection);
            if (owner.hasAnimator)
            {
                if (angleToPlayer < 20){
                    owner.animator.SetBool("correctAngle", true);
                } 
                else 
                {
                    owner.animator.SetBool("correctAngle", false);
                }
            }
        }
        else 
        {
            owner.stateStack.Pop();
        }
    }

    // Used for determining distance to any other object the move state needs to be aware of
    protected float distanceTo(Vector3 other)
    {
        return (other - owner.transform.position).magnitude;
    }

    public override void Exit(EnemyState owner)
    {
        if (owner.hasAnimator)
        {
            owner.animator.SetBool("attack", false);
            owner.animator.SetBool("correctAngle", false);
        }
    }

    public override void OnHit(float damage, Vector3 knockback)
    {
        base.OnHit(damage, knockback);
        stats.Health -= damage;
        if (knockback.magnitude > stats.knockbackThreshhold)
        {
            ArrayList data = new ArrayList();
            data.Add(knockback);
            owner.stateStack.Push(owner.KnockbackState, data);
        }
    }

    public void Attack()
    {
        
    }
}