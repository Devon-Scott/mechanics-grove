using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hurtbox : MonoBehaviour
{
    public EntityStats Stats;
    public Collider Self;
    // Collider might belong to a blockade, so can't use characterController 

    void Start()
    {
        // Get a reference to the Collider for this Entity
        Self = transform.GetComponent<Collider>();

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

    public abstract void HandleHit(float damage, Vector3 knockback);
    public abstract void EnterKnockback(Vector3 impact);
}