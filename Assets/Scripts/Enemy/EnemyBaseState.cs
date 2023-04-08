using UnityEngine;
using MyUtils.StateMachine;

public abstract class EnemyBaseState : State<EnemyState>
{
    protected abstract void OnHit(int damage, Vector3 impact);
}

