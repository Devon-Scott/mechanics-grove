using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

public class EnemyController : MonoBehaviour
{
    /*
    Reference to the graph data for pathfinding
    All this class needs to know is that LevelGraph will provide
    the information necessary to get from point A to point B 
    */ 
    public LevelGraph levelGraph;

    // Variables to control state
    public bool attack;

    // Variables to control properties of enemy
    public float speed = 0.25f;
    public int health = 5;
    public float turnSpeed = 2.5f;
    public int gravity = -15;
    public LayerMask attackLayers;

    private CharacterController controller;
    private Animator animator;
    private SphereCollider attackCollider;
    private Collider target;
    
    private bool hasAnimator;
    private GraphNode nextNode;
    private GraphNode previousNode;
    private Vector3 targetDirection;

    private bool grounded = true;
    private float verticalVelocity;
    
    // Start is called before the first frame update
    void Start()
    {
        levelGraph = ScriptableObject.CreateInstance<LevelGraph>();
        controller = GetComponent<CharacterController>();
        attackCollider = GetComponent<SphereCollider>();
        hasAnimator = TryGetComponent(out animator);
        nextNode = levelGraph.StartNode;
        previousNode = null;
        targetDirection = nextNode.Location - transform.position;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        // Enemies will either move along the path or attack a player that comes too close
        // Enemy prefab has a child collider that will tell Enemy if a target is in range
        // using GetTarget and RemoveTarget
        if (!attack)
        {
            DoGravity();
            Move();
        } else {
            AttackState();
        }
    }

    void Move(){
        // Position axis is meant to provide a purely vertical frame of reference
        // for the enemy to know when to turn on a path, without taking
        // the vertical (y) distance into account
        // This may need refining with levels that have vertical components
        Vector3 positionAxis = transform.position;
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        // Move towards the next node
        Vector3 direction = new Vector3(0, verticalVelocity, 0) + (transform.forward * speed);
        controller.Move(direction * Time.deltaTime);
        grounded = controller.isGrounded;

        if (hasAnimator){
            animator.SetFloat("speed", speed);
        }
    }

    void GetTarget(Collider other)
    {
        target = other;
        attack = true;
        if (hasAnimator)
        {
            animator.SetBool("attack", attack);
        }
    }

    void RemoveTarget()
    {
        target = null;
        attack = false;
        if (hasAnimator)
        {
            animator.SetBool("attack", attack);
        }
    }

    void AttackState(){
        // Rotate to the target
        if (target != null)
        {
            Vector3 positionAxis = transform.position;
            positionAxis.y = 0;
            targetDirection = target.gameObject.transform.position - positionAxis;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            // Should probably detect if we're actually facing the player before we attack
            float angleToPlayer = Vector3.Angle(transform.forward, targetDirection);
            if (hasAnimator)
            {
                if (angleToPlayer < 20){
                    animator.SetBool("correctAngle", true);
                } 
                else 
                {
                    animator.SetBool("correctAngle", false);
                }
            }
        }
        // Create hitbox at peak of attack animation
    }

    void DoGravity()
    {
        if (grounded)
        {
            verticalVelocity = Mathf.Clamp(verticalVelocity, -0.1f, Mathf.Infinity);
        } 
        else 
        {
            verticalVelocity = Mathf.Clamp(verticalVelocity + gravity * Time.deltaTime, gravity, Mathf.Infinity);
        }
    }

    void RefreshAttack()
    {
        gameObject.BroadcastMessage("clearObjectCache");
    }
}
