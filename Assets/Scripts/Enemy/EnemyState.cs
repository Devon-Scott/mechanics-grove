using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;
using MyUtils.StateMachine;

public class EnemyState : MonoBehaviour
{
    
    public StateStack<EnemyState> stateStack;
    public State<EnemyState> currentState;

    public EnemyMoveState MoveState = new EnemyMoveState();
    public EnemyAttackState AttackState = new EnemyAttackState();
    public EnemyKnockbackState KnockbackState = new EnemyKnockbackState();

    /*
    Reference to the graph data for pathfinding
    All this class needs to know is that LevelGraph will provide
    the information necessary to get from point A to point B 
    */ 
    public static LevelGraph levelGraph;
    public EnemyStats stats;
    public LayerMask attackLayers;

    // Reference to enemy players on the map;
    public List<Collider> players;

    public CharacterController controller;
    public Animator animator;
    public bool hasAnimator;
    public SphereCollider environmentCheck;
    public Collider target;

    public bool grounded = true;
    public float verticalVelocity;
    
    void Start()
    {
        stateStack = new StateStack<EnemyState>(this);
        hasAnimator = TryGetComponent(out animator);
        controller = GetComponent<CharacterController>();
        environmentCheck = GetComponent<SphereCollider>();
        target = null;
        stats = GetComponent<EnemyStats>();
        levelGraph = ScriptableObject.CreateInstance<LevelGraph>();
        stateStack.Push(MoveState);
        players = new List<Collider>();
    }

    void Update()
    {
        DoGravity();
        stateStack.Update();
    }

    void DoGravity()
    {
        if (grounded)
        {
            verticalVelocity = Mathf.Clamp(verticalVelocity, -0.1f, Mathf.Infinity);
        } 
        else 
        {
            verticalVelocity = Mathf.Clamp(verticalVelocity + stats.gravity * Time.deltaTime, stats.gravity, Mathf.Infinity);
        }
        grounded = controller.isGrounded;

    }
}