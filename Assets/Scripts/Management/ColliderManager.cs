using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderManager : MonoBehaviour, IEnemyObserver
{
    // This class will also need to be IColliderObserver
    public static Dictionary<Collider, Hurtbox> ColliderDictionary;

    void Awake()
    {
        if (ColliderDictionary == null)
        {
            ColliderDictionary = new Dictionary<Collider, Hurtbox>();
        }
        else 
        {
            // Singleton collider, enforce only one exists
            Destroy(gameObject);
        }
    }

    void Update()
    {

    }

    public void OnEnemySpawn(Enemy enemy)
    {
        Hurtbox enemyHurtbox = enemy.GetComponent<Hurtbox>();
        Collider enemyCollider = enemy.GetComponent<Collider>();
        ColliderDictionary.Add(enemyCollider, enemyHurtbox);
    }

    public void OnEnemyDeath(Enemy enemy)
    {
        Collider enemyCollider = enemy.GetComponent<Collider>();
        ColliderDictionary.Remove(enemyCollider);
    }

}