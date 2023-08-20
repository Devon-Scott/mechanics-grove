using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;
using MyUtils.StateMachine;

public class Enemy : MonoBehaviour
{
    
    public StateStack<Enemy> stateStack;
    public EnemyBaseState currentState;
    public string StateName;

    // All states inherit from EnemyBaseState
    public EnemyMoveState MoveState = new EnemyMoveState();
    public EnemyAttackState AttackState = new EnemyAttackState();
    public EnemyKnockbackState KnockbackState = new EnemyKnockbackState();
    public EnemyDeathState DeathState = new EnemyDeathState();

    public GameObject DeathExplosion;
    public GameObject VictoryParticle;
    /*
    Reference to the graph data for pathfinding
    All this class needs to know is that Level will provide
    the information necessary to get from point A to point B 
    */ 
    public static Level level;

    // Reference to players and observers in the scene
    public static List<Collider> PlayerList;
    private List<IEnemyObserver> observers = new List<IEnemyObserver>();

    public EntityStats stats;
    public LayerMask attackLayers;
    public float attackCooldown;
    public CharacterController controller;
    public Animator animator;
    public bool hasAnimator;
    public SphereCollider environmentCheck;
    public Collider target;

    public bool knockedBack;
    public bool grounded = true;
    public float verticalVelocity;

    private bool Alive;
    
    void Start()
    {
        knockedBack = false;
        stateStack = new StateStack<Enemy>(this);
        hasAnimator = TryGetComponent(out animator);
        controller = GetComponent<CharacterController>();
        environmentCheck = GetComponent<SphereCollider>();
        target = null;
        stats = GetComponent<EntityStats>();
        stateStack.Push(MoveState);
        if (PlayerList == null)
        {
            PlayerList = new List<Collider>();
        }
        if (PlayerList.Count == 0)
        {
            GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in Players)
            {
                PlayerList.Add(player.GetComponent<CharacterController>());
            }
        }
        foreach (IEnemyObserver observer in observers)
        {
            observer.OnEnemySpawn(this);
        }
        
    }

    void Update()
    {
        DoGravity();
        if (stateStack.Stack.Count == 0)
        {
            stateStack.Push(MoveState);
        }
        currentState = (EnemyBaseState)stateStack.CurrentState;
        StateName = currentState.GetType().Name;
        stateStack.Update();
    }

    public void AddObserver(IEnemyObserver observer)
    {
        observers.Add(observer);
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

    public void OnHit(float damage, Vector3 knockback, float scalar)
    {
        print("State Machine OnHit Method Called");
        if (currentState != null)
        {
            currentState.OnHit(damage, knockback, scalar);
        }
        else
        {
            Debug.Log("Current state was somehow null");
        }
    }

    // When an enemy is selected in the inspector, draws a red line on all the
    // paths of the level, and a green circle around the point the enemy is moving to
    void OnDrawGizmosSelected()
    {
        if (level != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(MoveState.TargetPoint, 1);

            Gizmos.color = Color.red;
            foreach(Edge edge in level.Edges)
            {
                Gizmos.DrawLine(edge.start + Vector3.up, edge.end + Vector3.up);
            }
        }
    }

    public void onDeath()
    {
        foreach (IEnemyObserver observer in observers)
        {
            observer.OnEnemyDeath(this);
        }
        ParticleManager.ParticleManagerInit(transform.position, DeathExplosion);
        Destroy(gameObject);
    }

    public void onVictory(){
        foreach (IEnemyObserver observer in observers)
        {
            observer.OnEnemyVictory(this);
        }
        ParticleManager.ParticleManagerInit(transform.position, VictoryParticle);

        Destroy(gameObject);
    }
}