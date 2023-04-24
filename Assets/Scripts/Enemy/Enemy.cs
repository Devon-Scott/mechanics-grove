using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;
using MyUtils.StateMachine;

public class Enemy : MonoBehaviour
{
    
    public StateStack<Enemy> stateStack;
    public EnemyBaseState currentState;

    // All states inherit from EnemyBaseState
    public EnemyMoveState MoveState = new EnemyMoveState();
    public EnemyAttackState AttackState = new EnemyAttackState();
    public EnemyKnockbackState KnockbackState = new EnemyKnockbackState();
    public EnemyDeathState DeathState = new EnemyDeathState();

    public GameObject DeathExplosion;
    /*
    Reference to the graph data for pathfinding
    All this class needs to know is that LevelGraph will provide
    the information necessary to get from point A to point B 
    */ 
    public static LevelGraph levelGraph;

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
        if (levelGraph == null)
        {
            print("Instantiating level");
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
        foreach (IEnemyObserver observer in observers)
        {
            observer.OnEnemySpawn(this);
        }
    }

    void Update()
    {
        DoGravity();
        currentState = (EnemyBaseState)stateStack.CurrentState;
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

    public void OnHit(float damage, Vector3 knockback)
    {
        currentState.OnHit(damage, knockback);
    }

    void OnDrawGizmosSelected()
    {
        if (levelGraph != null)
        {
            // Draw a green sphere at the next location this is moving to
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(MoveState.TargetPoint, 1);

            // Draw a red line along the whole levelgraph if it's not null
            Gizmos.color = Color.red;
            HashSet<GraphNode> Visited = new HashSet<GraphNode>();
            Queue<GraphNode> Queue = new Queue<GraphNode>();
            Queue.Enqueue(levelGraph.StartNode);

            GraphNode Closest = levelGraph.StartNode;
            while (Queue.Count > 0)
            {
                GraphNode Node = Queue.Dequeue();
                for (int i = 0; i < Node.childNodes.Count; i++)
                {
                    GraphNode SampleNode = Node.childNodes[i];
                    if (Visited.Add(SampleNode)){
                        Queue.Enqueue(SampleNode);
                        Gizmos.DrawLine(Node.Location + Vector3.up, SampleNode.Location + Vector3.up);
                    }   
                }
            }
        }
    }

    public void onDeath()
    {
        foreach (IEnemyObserver observer in observers)
        {
            observer.OnEnemyDeath(this);
        }
        Instantiate(DeathExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}