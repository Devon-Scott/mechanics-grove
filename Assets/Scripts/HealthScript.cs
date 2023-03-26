using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public int health;
    public int knockbackThreshold;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleHit(int damage, Vector3 impact)
    {
        print("Player hit for " + damage + " damage");
        health -= damage;
        if (damage >= knockbackThreshold){
            EnterKnockback(impact);
        }
        if (health <= 0)
        {
            print("Object has died");
        }
    }

    void EnterKnockback(Vector3 impact)
    {
        print(impact.ToString());
    }
}
