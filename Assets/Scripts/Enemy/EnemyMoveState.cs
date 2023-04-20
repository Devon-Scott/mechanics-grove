using System;
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
    private bool knockedBack;
    private bool nearPath;

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
            knockedBack = false;
            nearPath = true;
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
            if (distanceBetween(owner.transform.position, other.transform.position) < 2.5f)
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
        if (knockedBack)
        {
            targetDirection = findClosestPointOnPath(path, owner.transform.position);
            nextNode = findClosestChild(
                    BFSFindClosestNode(path, path.StartNode, owner.transform.position),
                    owner.transform.position
                );
            // Set knockedBack to false, because nearPath won't be true until we're
            // close eough to the path, and we don't need to run this check every frame
            knockedBack = false;
        }
        else if (nearPath)
        {
            targetDirection = nextNode.Location - positionAxis;
        }

        if (targetDirection.magnitude <= 1f){
            // We're already on the path so we find the next node
            if (nearPath)
            {
                // Get the next node
                List<GraphNode> nextNodeList = nextNode.getChildren();
                int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, nextNodeList.Count));
                previousNode = nextNode;
                nextNode = nextNodeList[index];
                // update the Target Direction
                targetDirection = nextNode.Location - positionAxis;
            }
            // Otherwise, we're close enough to the path that we can start moving to the next node
            else 
            {
                nearPath = true;
            }
        }
        // Rotate towards the next node
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
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
        // Check knockback before death (personal preference)
        if (knockback.magnitude > stats.knockbackThreshhold)
        {
            ArrayList data = new ArrayList();
            data.Add(knockback);
            // We need to know if we've been knocked back so we can find our way back onto the path
            knockedBack = true;
            nearPath = false;
            owner.stateStack.Push(owner.KnockbackState, data);
        }
        else if (stats.Health <= 0)
        {
            owner.stateStack.ChangeState(owner.DeathState);
        }
        else
        {
            // Flash to indicate damage
            // Play sound to indicate damage
        }
    }

    Vector3 findClosestPointOnPath(LevelGraph path, Vector3 position)
    {
        GraphNode closestParent = BFSFindClosestNode(path, path.StartNode, position);
        GraphNode closestChild = findClosestChild(closestParent, position);
        Vector3 closestPoint = findClosestPointBetween(closestParent, closestChild, position);
        return closestPoint;
    }

    // Basic utility function for finding distance between points
    protected float distanceBetween(Vector3 start, Vector3 end)
    {
        return (end - start).magnitude;
    }

    // Get the closest node to a point in the scene. This can probably be reused elsewhere
    GraphNode BFSFindClosestNode(LevelGraph G, GraphNode source, Vector3 position)
    {
        if (G == null || source == null)
        {
            throw new ArgumentNullException("Graph or Start Node were null, can't traverse");
        }
        HashSet<GraphNode> Visited = new HashSet<GraphNode>();
        Queue<GraphNode> Queue = new Queue<GraphNode>();
        Visited.Add(source);
        Queue.Enqueue(source);

        GraphNode Closest = source;
        float closestDistance = distanceBetween(position, Closest.Location);
        while (Queue.Count > 0)
        {
            GraphNode Node = Queue.Dequeue();
            for (int i = 0; i < Node.childNodes.Count; i++)
            {
                GraphNode SampleNode = Node.childNodes[i];
                if (Visited.Add(SampleNode)){
                    Queue.Enqueue(SampleNode);
                    float newDistance = distanceBetween(position, SampleNode.Location);
                    if (newDistance < closestDistance)
                    {
                        Closest = SampleNode;
                        closestDistance = newDistance;
                    }
                }   
            }
        }
        return Closest;
    }

    GraphNode findClosestChild(GraphNode Parent, Vector3 position)
    {
        GraphNode closestChild = Parent.childNodes[0];
        float closestDistance = distanceBetween(position, closestChild.Location);
        foreach (GraphNode child in Parent.childNodes)
        {
            float newDistance = distanceBetween(position, child.Location);
            if (newDistance < closestDistance)
            {
                closestChild = child;
                closestDistance = newDistance;
            }
        }
        return closestChild;
    }

    Vector3 findClosestPointBetween(GraphNode parent, GraphNode child, Vector3 position)
    {
        // From my understanding, this creates an "origin" at the parent node, and then the projection
        // is the vector from the parent to the object onto the vector from the parent to the child
        Vector3 point = Vector3.Project(position - parent.Location, child.Location - parent.Location);

        // If point does not lie on the line between parent and child, then presumably there exists
        // a set of nodes closer that should have been found by BFSFindClosestNode and FindClosestChild
        return point;
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the next location this is moving to
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetDirection, 2);
    }
}
