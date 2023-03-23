using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    EnemyHitbox hitbox;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = FindObjectOfType<EnemyHitbox>();
        hitbox.hit += HandleHit;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandleHit(int damage)
    {
        print("Player hit for " + damage + " damage");
    }
}
