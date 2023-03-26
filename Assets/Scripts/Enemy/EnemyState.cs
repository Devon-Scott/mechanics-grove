using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;
using MyUtils.StateMachine;

public class EnemyState : MonoBehaviour
{
    
    StateStack<EnemyState> stateStack;
    State<EnemyState> currentState;

    EnemyMoveState MoveState = new EnemyMoveState();
    EnemyAttackState EnemyAttackState = new EnemyAttackState();
    EnemnyKnockbackState EnemnyKnockbackState = new EnemnyKnockbackState();

    /*
    Reference to the graph data for pathfinding
    All this class needs to know is that LevelGraph will provide
    the information necessary to get from point A to point B 
    */ 
    public static LevelGraph levelGraph;
    public EnemyStats stats;
    void Start()
    {
        stateStack = new StateStack<EnemyState>(this);
        stateStack.Push(MoveState);
        stats = new EnemyStats();
    }

    void Update()
    {
        stateStack.Update();
    }
}