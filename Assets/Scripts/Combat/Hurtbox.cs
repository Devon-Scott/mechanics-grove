using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 // get the reference to the ColliderManager
//colliderManager = FindObjectOfType<ColliderManager>();

// register the hurtbox with the ColliderManager
//colliderManager.RegisterHurtbox(this);
public abstract class Hurtbox : MonoBehaviour
{
    public EntityStats Stats;
    public Collider Self;
    // Collider might belong to a blockade, so can't use characterController 
    // CC inherits from Collider though, so this isn't a big problem

    void Start()
    {
        // Get a reference to the Collider for this Entity
        Self = transform.GetComponent<Collider>();

        // Get a reference to the Stats object for tracking Entity vitals
        Stats = GetComponent<EntityStats>();
    }

    public abstract void HandleHit(float damage, Vector3 knockback, float scalar);
}