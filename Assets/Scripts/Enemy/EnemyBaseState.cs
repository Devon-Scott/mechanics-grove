using UnityEngine;
using MyUtils.StateMachine;

public abstract class EnemyBaseState : State<EnemyState>
{
    public abstract void OnHit(int damage, Vector3 impact);

    
}

