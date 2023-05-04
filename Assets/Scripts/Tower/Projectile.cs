using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float radius;
    public Hitbox hitbox;
    public Rigidbody self;
    private bool _destroyOnNextUpdate;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = gameObject.GetComponent<Hitbox>();
        self = gameObject.GetComponent<Rigidbody>();

        hitbox.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyForce(Vector3 direction)
    {
        self.AddForce(direction);
    }

    void OnCollisionEnter()
    {
        hitbox.enabled = true;
        hitbox.EnableDamage(20);
    }    
}
