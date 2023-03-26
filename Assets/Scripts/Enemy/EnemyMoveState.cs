using UnityEngine;
using MyUtils.StateMachine;
using MyUtils.Graph;


public class EnemyMoveState : State<EnemyState>
{
    public static LevelGraph path;
    private EnemyState owner;

    public override void Enter(EnemyState owner)
    {
        this.owner = owner;
        path = EnemyState.levelGraph;
    }

    public override void Update(EnemyState owner)
    {

    }

    public override void Exit(EnemyState owner)
    {
        
    }


}
