using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private HashSet<Collider> HitObjects;
    private Dictionary<Collider, HealthScript> History;
    private int damage;
    private CharacterController parentCollider;

    public Collider self;
    public LayerMask TargetableObjects;
    // Start is called before the first frame update
    void Start()
    {
        HitObjects = new HashSet<Collider>();
        History = new Dictionary<Collider, HealthScript>();
        parentCollider = transform.GetComponentInParent<CharacterController>();
        // Damage should be inherited from parent game object
        damage = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;  
        // First check if we should care about this collider at all
        if (TargetableObjects == (TargetableObjects | (1 << layer)))
        {
            // Then check if the object has already been hit this animation
            if (!HitObjects.Contains(other))
            {
                HitObjects.Add(other);
                HealthScript hitTarget;

                // Then check if we recognize the object that we hit
                // so we can quickly deal damage again
                if (History.ContainsKey(other))
                {
                    hitTarget = History[other];
                }
                else 
                {
                    hitTarget = other.gameObject.GetComponent<HealthScript>();
                }
                hitTarget.HandleHit(damage);
            }
        }
    }

    public void toggleHitbox()
    {
        if (self.enabled)
        {
            Invoke("clearObjects", 0.25f);
        }
    }

    void clearObjects()
    {
        HitObjects.Clear();
        print("Objects cache is empty");
    }
}
