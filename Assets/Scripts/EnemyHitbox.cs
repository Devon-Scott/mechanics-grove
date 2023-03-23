using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private HashSet<Collider> HitObjects;
    public event Action<int> hit;
    public int damage;

    public LayerMask TargetableObjects;
    // Start is called before the first frame update
    void Start()
    {
        HitObjects = new HashSet<Collider>();
        damage = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        int layer = other.gameObject.layer;
        if (!HitObjects.Contains(other))
        {
            if (TargetableObjects == (TargetableObjects | (1 << layer)))
            {
                HitObjects.Add(other);
                print("Invoking hit with " + damage + " damage");
                hit.Invoke(damage);
            }
        } 
    }

    void clearObjectCache()
    {
        HitObjects.Clear();
    }
}
