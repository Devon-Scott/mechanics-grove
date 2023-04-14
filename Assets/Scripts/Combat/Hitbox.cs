using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=uGFzWM1sJjU
// Change hit logic to update, use spherecast or some kinnd of manual collider to check
// That way static AOE damage can just clear hit objects once per second and everything still works
// Now jsut need to figure out how to get information for the spherecast 

public class Hitbox : MonoBehaviour
{
    private HashSet<Collider> HitObjects;
    private static Dictionary<Collider, Hurtbox> ColliderDictionary;
    
    public float damage;
    public float KnockbackScaler;
    
    private RaycastHit hitInfo;

    private Vector3 parentPosition;

    public bool Active;
    public float SphereCastRadius;
    public float SphereCastLength;
    public LayerMask TargetableObjects;
    // Start is called before the first frame update
    void Start()
    {
        parentPosition = transform.parent.position;
        SphereCastLength = (transform.position - parentPosition).magnitude;
        print(SphereCastLength);
        Active = false;
        HitObjects = new HashSet<Collider>();
        // Lazy initialization of a static data structure for speeding up hurtbox references
        if (ColliderDictionary == null)
        {
            ColliderDictionary = new Dictionary<Collider, Hurtbox>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        parentPosition = transform.parent.position;
        if (Active)
        {
            
            // Check if anything was hit in the layermask we care about and along the size of this hitbox
            if(Physics.SphereCast(parentPosition, SphereCastRadius, transform.up, out hitInfo, SphereCastLength, TargetableObjects))
            {
                Collider hitObject = hitInfo.collider;

                // Then check if the object has already been hit this animation
                if (!HitObjects.Contains(hitObject))
                {
                    // Add it to the list so we don't hit it again during this attack
                    HitObjects.Add(hitObject);

                    // Instantiate a hurtbox to store the reference of the collider we hit
                    Hurtbox hitTarget;

                    // Then check if we recognize the object that we hit from a static dictionary
                    // so we can quickly deal damage again
                    if (ColliderDictionary.ContainsKey(hitObject))
                    {
                        hitTarget = ColliderDictionary[hitObject];
                    }
                    else 
                    {
                        hitTarget = hitObject.gameObject.GetComponent<Hurtbox>();
                        ColliderDictionary.Add(hitObject, hitTarget);
                    }
                    hitTarget.HandleHit(damage, (hitObject.transform.position - parentPosition).normalized * KnockbackScaler);
                }
            }
        }
    }

    public void EnableDamage(float damage)
    {
        Active = true;
        this.damage = damage;
        HitObjects.Clear();
    }

    public void DisableDamage()
    {
        Active = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, parentPosition);
    }
}
