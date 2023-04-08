using UnityEngine;
using MyUtils.StateMachine;

public class EnemyAttackState : EnemyBaseState
{
    private Collider target;
    private Vector3 targetDirection;
    public bool hitboxActive;
    EnemyState owner;

    public override void Enter(EnemyState owner)
    {
        this.owner = owner;
        hitboxActive = false;
        target = owner.target;
        if (owner.hasAnimator)
        {
            owner.animator.SetBool("attack", true);
        }
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

    protected override void OnHit(int damage, Vector3 impact)
    {
        
    }

    public void Attack()
    {
        
    }
}