using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;
using MyUtils.Graph;

public class EnemyMoveState : EnemyBaseState
{
    public static Level level;
    private Enemy owner;
    
    private CharacterController controller;
    private EntityStats stats;
    private bool _nearPath;
    private bool _needDirectionUpdate;
    private bool init = true;

    private Vector3 _targetDirection;
    private Vector3 _targetPoint;
    private Vector3 _nextNode;

    public Vector3 TargetPoint
    {
        get {return _targetPoint;}
        set 
        {
            _targetPoint = value;
        }
    }

    static List<Collider> PlayerList;

    public override void Enter(Enemy owner, ArrayList data)
    {
        if (init)
        {
            this.owner = owner;
            level = Enemy.level;
            init = false;
            _nextNode = level.StartPoint;
            _targetDirection = _nextNode - owner.transform.position;
            TargetPoint = _nextNode;
            controller = owner.controller;
            stats = owner.stats;
            PlayerList = Enemy.PlayerList;
            _nearPath = true;
        }
        if (owner.hasAnimator){
            owner.animator.SetFloat("speed", stats.Speed);
        }
    }

    public override void Update(Enemy owner)
    {
        // Check if we need to target a player
        foreach (Collider other in Enemy.PlayerList)
        {
            Debug.Log("Checking in range");
            if (Vector3.Distance(owner.transform.position, other.transform.position) < 2.5f)
            {
                
                owner.target = other;
                owner.stateStack.Push(owner.AttackState);
                break;
            }
        }
        
        // Position axis is meant to provide a purely vertical frame of reference
        // for the enemy to know when to turn on a path, without taking
        // the vertical (y) distance into account, so that Controller doesn't get confused
        // when close to a node that is "in the ground"
        // This may need refining with levels that have vertical components
        Vector3 positionAxis = owner.transform.position;
        positionAxis.y = 0;

        // Check if we need to find our way back to the path, and if so, walk to it.
        // If we're close enogh to the path, we point ourselves to the next node.
        if (owner.knockedBack)
        {
            Vector3 closestParent = level._graph.FindClosestNode(positionAxis);
            Vector3 closestChild = level._graph.findClosestChild(closestParent, positionAxis);
            Vector3 closestPoint = Graph.findClosestPointBetween(closestParent, closestChild, positionAxis);
            TargetPoint = closestPoint;
            _nextNode = closestChild;
            // Set _knockedBack to false, because _nearPath won't be true until we're
            // close eough to this TargetPoint, and we don't need to run this check every frame
            owner.knockedBack = false;
        }

        if (Graph.distanceBetween(positionAxis, TargetPoint) <= 1f){
            // We're already on the path so we find the next node
            if (_nearPath)
            {
                if (TargetPoint == _nextNode)
                {
                    // Get the next node
                    List<Vector3> nextNodeList = level._graph.getChildren(_nextNode);
                    if (nextNodeList.Count == 0)
                    {
                        owner.onVictory();
                    }
                    else
                    {
                        int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, nextNodeList.Count));
                        _nextNode = nextNodeList[index];
                        // update the Target Direction
                        _targetDirection = _nextNode - positionAxis;  
                    }
                } 
                // If _nearPath was not true on the last call, it will be true now
                // So regardless of if we reach a node or just got back to the path
                // we need to update our target point of direction
                TargetPoint = _nextNode;
            }
            // Otherwise, we're close enough to the path that we can start moving to the next node
            else 
            {
                _nearPath = true;
            }
        }
        // Rotate towards the next node
        _targetDirection = TargetPoint - positionAxis;
        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime * stats.TurnSpeed);

        // Move towards the next node
        Vector3 direction = new Vector3(0, owner.verticalVelocity, 0) + (owner.transform.forward * stats.Speed);
        
        controller.Move(direction * Time.deltaTime);

        if (owner.hasAnimator){
            owner.animator.SetFloat("speed", stats.Speed);
        }
    }

    public override void Exit(Enemy owner)
    {
        if (owner.hasAnimator){
            owner.animator.SetFloat("speed", stats.Speed);
        }
    }

    public override void OnHit(float damage, Vector3 knockback, float scalar)
    {
        stats.Health -= damage;
        // Check knockback before death (personal preference) maybe not
        
        if (stats.Health <= 0)
        {
            owner.stateStack.ChangeState(owner.DeathState);
        }
        else if (knockback.magnitude > stats.knockbackThreshhold)
        {
            ArrayList data = new ArrayList();
            data.Add(knockback);
            data.Add(scalar);
            // We need to know if we've been knocked back so we can find our way back onto the path in EnemyMoveState
            owner.knockedBack = true;
            _nearPath = false;
            owner.stateStack.Push(owner.KnockbackState, data);
        }
        else
        {
            // Flash to indicate damage
            // Play sound to indicate damage
        }
    }
}
