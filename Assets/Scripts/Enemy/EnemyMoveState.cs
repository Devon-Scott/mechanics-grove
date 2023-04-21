using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;
using MyUtils.Graph;

public class EnemyMoveState : EnemyBaseState
{
    public static LevelGraph path;
    private Enemy owner;
    private GraphNode nextNode;
    private GraphNode previousNode;
    private CharacterController controller;
    private EntityStats stats;
    private bool _nearPath;
    private bool _needDirectionUpdate;

    private bool init = true;
    private Vector3 targetDirection;
    private Vector3 _targetPoint;
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
            path = Enemy.levelGraph;
            init = false;
            previousNode = null;
            nextNode = path.StartNode;
            if (nextNode == null)
            {
                Debug.Log("what the hell");
            }
            targetDirection = nextNode.Location - owner.transform.position;
            TargetPoint = nextNode.Location;
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
        if (owner.knockedBack)
        {
            GraphNode closestParent = BFSFindClosestNode(path, path.StartNode, positionAxis);
            GraphNode closestChild = findClosestChild(closestParent, positionAxis);
            Vector3 closestPoint = findClosestPointBetween(closestParent, closestChild, positionAxis);
            TargetPoint = closestPoint;
            nextNode = closestChild;
            // Set _knockedBack to false, because _nearPath won't be true until we're
            // close eough to this TargetPoint, and we don't need to run this check every frame
            owner.knockedBack = false;
        }

        if (distanceBetween(positionAxis, TargetPoint) <= 1f){
            // We're already on the path so we find the next node
            if (_nearPath)
            {
                if (TargetPoint == nextNode.Location)
                {
                    // Get the next node
                    List<GraphNode> nextNodeList = nextNode.getChildren();
                    int index = (int)Mathf.Floor(UnityEngine.Random.Range(0, nextNodeList.Count));
                    previousNode = nextNode;
                    nextNode = nextNodeList[index];
                    // update the Target Direction
                    targetDirection = nextNode.Location - positionAxis;
                    TargetPoint = nextNode.Location;
                } 
                else 
                {
                    TargetPoint = nextNode.Location;
                }

            }
            // Otherwise, we're close enough to the path that we can start moving to the next node
            else 
            {
                _nearPath = true;
            }
        }
        // Rotate towards the next node
        targetDirection = TargetPoint - positionAxis;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
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

    public override void OnHit(float damage, Vector3 knockback)
    {
        base.OnHit(damage, knockback);
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
        // But we check just in case
        if (pointIsInRange(parent.Location, child.Location, parent.Location + point))
        {
            return parent.Location + point;
        }
        else 
        {
            float parentDistance = (position - parent.Location).sqrMagnitude;
            float childDistance = (position - child.Location).sqrMagnitude;
            if (parentDistance <= childDistance)
            {
                return parent.Location;
            }
            else 
            {
                return child.Location;
            }
        }
        
    }

    bool pointIsInRange(Vector3 start, Vector3 end, Vector3 point)
    {
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        float dz = end.z - start.z;
        float innerProduct = (point.x - start.x) * dx + (point.y - start.y) * dy + (point.z - start.z) * dz;
        return 0 <= innerProduct && innerProduct <= dx * dx + dy * dy + dz * dz;
    }
}
