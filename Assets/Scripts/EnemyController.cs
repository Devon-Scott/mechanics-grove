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
    public LayerMask attackLayers;

    private CharacterController controller;
    private Animator animator;
    private SphereCollider attackCollider;
    private Collider target;
    private bool hasAnimator;
    private GraphNode nextNode;
    private Vector3 targetDirection;

    // Start is called before the first frame update
    void Start()
    {
        levelGraph = ScriptableObject.CreateInstance<LevelGraph>();
        controller = GetComponent<CharacterController>();
        attackCollider = GetComponent<SphereCollider>();
        hasAnimator = TryGetComponent(out animator);
        nextNode = levelGraph.StartNode;
        targetDirection = nextNode.Location - transform.position;
        target = null;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!attack)
        {
            Move();
        } else {
            //Attack();
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
            nextNode = nextNodeList[index];
            // update the Target Direction
            targetDirection = nextNode.Location - positionAxis;
            targetRotation = Quaternion.LookRotation(targetDirection);
        }
        // Rotate towards the next node
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

        // Move towards the next node
        controller.Move(transform.forward * speed * Time.deltaTime);

        if (hasAnimator){
            animator.SetFloat("speed", speed);
        }
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.layer == LayerMask.NameToLayer("Character")){
            attack = true;
            target = other;
        } 
        if (hasAnimator){
            animator.SetBool("attack", attack);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.layer == LayerMask.NameToLayer("Character")){
            attack = false;
            target = null;
        } 
        if (hasAnimator){
            animator.SetBool("attack", attack);
        }
    }

    void Attack(){
        // Rotate to the target
        // if (target != null){
        //     Vector3 positionAxis = transform.position;
        //     positionAxis.y = 0;
        //     targetDirection = nextNode.Location - positionAxis;
        //     Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        // }
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        // Create hitbox at peak of attack animation
    }
}
