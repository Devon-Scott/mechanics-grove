using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.StateMachine;

public class EnemyHurtbox : Hurtbox
{
    public Enemy stateMachine;

    void Start()
    {
        print("EnemyHurtbox Start function");
        // Get a reference to the Collider for this Entity
        Self = transform.GetComponent<CharacterController>();
        stateMachine = transform.GetComponent<Enemy>();
        // get the reference to the ColliderManager
        //colliderManager = FindObjectOfType<ColliderManager>();
        
        // register the hurtbox with the ColliderManager
        //colliderManager.RegisterHurtbox(this);

        // Get a reference to the Stats object for tracking Entity vitals
        Stats = GetComponent<EntityStats>();
        

        /* 
        The idea would be for players, enemies, and obstacles to all have a hurtbox,
        then when a hitbox with damage data comes in, the hurtbox can delegate this information
        to the state machine manager of the object its attached to
        For example
        public void handleKnockback()
        {
            StateMachine.HandleKnockback();
        }
        That way, appropriate knockback behaviour can be controlled in every state
        */
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public override void HandleHit(float damage, Vector3 knockback)
    {
        stateMachine.OnHit(damage, knockback);
    }
}
