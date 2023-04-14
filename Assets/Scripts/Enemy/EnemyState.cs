using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;
using MyUtils.StateMachine;

public class EnemyState : MonoBehaviour
{
    
    public StateStack<EnemyState> stateStack;
    public EnemyBaseState currentState;

    public EnemyMoveState MoveState = new EnemyMoveState();
    public EnemyAttackState AttackState = new EnemyAttackState();
    public EnemyKnockbackState KnockbackState = new EnemyKnockbackState();

    /*
    Reference to the graph data for pathfinding
    All this class needs to know is that LevelGraph will provide
    the information necessary to get from point A to point B 
    */ 
    public static LevelGraph levelGraph;

    // Reference to enemy players on the map;
    public List<Collider> PlayerList;

    public EntityStats stats;
    public LayerMask attackLayers;
    public float attackCooldown;
    public CharacterController controller;
    public Animator animator;
    public bool hasAnimator;
    public SphereCollider environmentCheck;
    public Collider target;

    public bool grounded = true;
    public float verticalVelocity;

    private bool Alive;
    
    void Start()
    {
        stateStack = new StateStack<EnemyState>(this);
        hasAnimator = TryGetComponent(out animator);
        controller = GetComponent<CharacterController>();
        environmentCheck = GetComponent<SphereCollider>();
        target = null;
        stats = GetComponent<EntityStats>();
        if (levelGraph == null)
        {
            levelGraph = ScriptableObject.CreateInstance<LevelGraph>();
        }
        stateStack.Push(MoveState);
        if (PlayerList == null)
        {
            PlayerList = new List<Collider>();
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in Players)
            {
                PlayerList.Add(player.GetComponent<CharacterController>());
            }
        }
    }

    void Update()
    {

        DoGravity();
        CheckHealth();
        currentState = (EnemyBaseState)stateStack.CurrentState;
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
            verticalVelocity = Mathf.Clamp(verticalVelocity + stats.Gravity * Time.deltaTime, stats.Gravity, Mathf.Infinity);
        }
        grounded = controller.isGrounded;
        animator.SetBool("Grounded", grounded);
        animator.SetFloat("VerticalVelocity", verticalVelocity);
    }

    public void OnHit(float damage, Vector3 knockback)
    {
        currentState.OnHit(damage, knockback);
    }

    public void toggleHitbox()
    {
        if (currentState == AttackState)
        {
            AttackState.hitboxActive = !AttackState.hitboxActive;
            //Debug.Log("Hitbox toggled " + AttackState.hitboxActive);
        }
        else 
        {
            //print("Attackstate not active");
        }
    }

    void CheckHealth()
    {
        if (stats.Health <= 0)
        {
            if (currentState is EnemyKnockbackState)
            {

            }
            else 
            {
                // Enter the DeathState
            }
        }
    }
}