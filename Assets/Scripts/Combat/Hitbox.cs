using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

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

    public Vector3 parentPosition;

    public bool Active;
    public float SphereCastRadius;
    public float SphereCastLength;
    public LayerMask TargetableObjects;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.parent != null)
        {
            parentPosition = transform.parent.position;
        }
        
        SphereCastLength = (transform.position - parentPosition).magnitude;
        Active = false;
        HitObjects = new HashSet<Collider>();
        // Get static data reference to the collider dictionary used for fast lookup of hitboxes
        if (ColliderDictionary == null)
        {
            ColliderDictionary = ColliderManager.ColliderDictionary;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent == null)
        {
            parentPosition = transform.position;
        }
        else
        {
            parentPosition = transform.parent.position;
        }
        if (Active)
        {
            
            Collider[] ContactedObjects = Physics.OverlapCapsule(parentPosition, transform.position, SphereCastRadius, TargetableObjects);
            foreach(Collider other in ContactedObjects)
            {
                if (!HitObjects.Contains(other))
                {
                    HitObjects.Add(other);
                    Hurtbox hitTarget;
                    if (ColliderDictionary.ContainsKey(other))
                    {
                        hitTarget = ColliderDictionary[other];
                    }
                    else 
                    {
                        hitTarget = other.gameObject.GetComponent<Hurtbox>();
                        ColliderDictionary.Add(other, hitTarget);
                        Debug.LogError("Entity was not in Dictionary", hitTarget);
                    }
                    // If the collider is completely overlapping the centre of the cast, distance should be 0 to avoid negative values
                    float DistanceFromCentre = Mathf.Max(0, Graph.DistanceToLine(parentPosition, transform.position, other.transform.position));
                    float HitScalar = Mathf.Abs(1 - (DistanceFromCentre / SphereCastRadius));
                    print(HitScalar);
                    hitTarget.HandleHit(damage, (other.transform.position - parentPosition).normalized * KnockbackScaler, HitScalar);
                }
            }
            // Check if anything was hit in the layermask we care about and along the size of this hitbox
            // if(Physics.SphereCast(parentPosition, SphereCastRadius, transform.up, out hitInfo, SphereCastLength, TargetableObjects))
            // {
            //     Collider hitObject = hitInfo.collider;

            //     // Then check if the object has already been hit this animation
            //     if (!HitObjects.Contains(hitObject))
            //     {
            //         // Add it to the list so we don't hit it again during this attack
            //         HitObjects.Add(hitObject);

            //         // Instantiate a hurtbox to store the reference of the collider we hit
            //         Hurtbox hitTarget;

            //         // Then check if we recognize the object that we hit from a static dictionary
            //         // so we can quickly deal damage again
            //         if (ColliderDictionary.ContainsKey(hitObject))
            //         {
            //             hitTarget = ColliderDictionary[hitObject];
            //         }
            //         else 
            //         {
            //             // Keep the game flowing but log an error to be investigated
            //             hitTarget = hitObject.gameObject.GetComponent<Hurtbox>();
            //             ColliderDictionary.Add(hitObject, hitTarget);
            //             Debug.LogError("Entity was not in Dictionary", hitTarget);
            //         }
            //         // Easier to see these distance calculations applied to damage but they need to be applied to knockback as well
            //         float DistanceFromCentre = Graph.DistanceToLine(parentPosition, transform.position, hitObject.transform.position);
            //         float ScaledDamageForDistance = Mathf.Abs(1 - (DistanceFromCentre / SphereCastRadius));
            //         hitTarget.HandleHit(damage * ScaledDamageForDistance, (hitObject.transform.position - parentPosition).normalized * KnockbackScaler);
            //     }
            // }
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
        Gizmos.DrawWireSphere(transform.position, SphereCastRadius);
        Gizmos.DrawWireSphere(parentPosition, SphereCastRadius);
    }
}
