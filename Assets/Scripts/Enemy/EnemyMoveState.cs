using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;
using MyUtils.Graph;


public class EnemyMoveState : EnemyBaseState
{
    public static LevelGraph path;
    private EnemyState owner;
    private GraphNode nextNode;
    private GraphNode previousNode;
    private CharacterController controller;
    private EntityStats stats;

    private bool init = true;
    private Vector3 targetDirection;

    List<Collider> PlayerList;

    public override void Enter(EnemyState owner, ArrayList data)
    {
        if (init)
        {
            this.owner = owner;
            path = EnemyState.levelGraph;
            init = false;
            previousNode = null;
            nextNode = path.StartNode;
            targetDirection = nextNode.Location - owner.transform.position;
            controller = owner.controller;
            stats = owner.stats;
            PlayerList = owner.PlayerList;
        }
        if (owner.hasAnimator){
            owner.animator.SetFloat("speed", stats.Speed);
        }
    }

    

    public override void Update(EnemyState owner)
    {
        // Check if we need to target a player
        foreach (Collider other in PlayerList)
        {
            if (distanceTo(other.transform.position) < 2.5f)
            {
                owner.target = other;
                owner.stateStack.Push(owner.AttackState);
                break;
            }
        }
        
        // Position axis is meant to provide a purely vertical frame of reference
        // for the enemy to know when to turn on a path, without taking
        // the vertical (y) distance into account
        // This may need refining with levels that have vertical components
        Vector3 positionAxis = owner.transform.position;
        positionAxis.y = 0;
        targetDirection = nextNode.Location - positionAxis;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        if (targetDirection.magnitude <= 1f){
            // Get the next node
            List<GraphNode> nextNodeList = nextNode.getChildren();
            int index = (int)Mathf.Floor(Random.Range(0, nextNodeList.Count));
            previousNode = nextNode;
            nextNode = nextNodeList[index];
            // update the Target Direction
            targetDirection = nextNode.Location - positionAxis;
            targetRotation = Quaternion.LookRotation(targetDirection);
        }
        // Rotate towards the next node
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime * stats.TurnSpeed);

        // Move towards the next node
        Vector3 direction = new Vector3(0, owner.verticalVelocity, 0) + (owner.transform.forward * stats.Speed);
        controller.Move(direction * Time.deltaTime);

        if (owner.hasAnimator){
            owner.animator.SetFloat("speed", stats.Speed);
        }
    }

    public override void Exit(EnemyState owner)
    {
        if (owner.hasAnimator){
            owner.animator.SetFloat("speed", stats.Speed);
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


    // Used for determining distance to any other object the move state needs to be aware of
    protected float distanceTo(Vector3 other)
    {
        return (other - owner.transform.position).magnitude;
    }

    // Used for finding the shortest distance from this object to any point along a vector
    float distanceToVector(Vector3 start, Vector3 end)
    {
        Vector3 targetLine = end - start;
        Vector3 pointOnLine = Vector3.Normalize(targetLine);
        Vector3 vectorToPoint = owner.transform.position - pointOnLine;
        float numerator = Vector3.Magnitude(Vector3.Cross(vectorToPoint, targetLine));

        return numerator / Vector3.Magnitude(targetLine);
    }
}
