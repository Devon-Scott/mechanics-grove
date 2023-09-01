using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerStats : MonoBehaviour
{
    private ThirdPersonController _player;

    public float MaxHealth;
    [SerializeField]
    private float _health;
    public float Health
    {
        get {return _health;}
        set
        {
            _health = value;
            if (_health <= 0)
            {
                _health = 0;
                BroadcastMessage("TriggerDeath");
            }
            // Send a message to the health bar to change the health value
            BroadcastMessage("OnHealth", Health);
        }
    }

    public int InitialLives;
    private int _lives;
    public int Lives
    {
        get {return _lives;}
        set 
        {
            _lives = value;
            if (_lives <= 0)
            {
                BroadcastMessage("TriggerGameOver");
            }
            BroadcastMessage("OnLifeLost", _lives);
        }
    }

    public int InitialMoney;
    private int _money;
    public int Money
    {
        get {return _money;}
        set 
        {
            _money = value;
            BroadcastMessage("OnMoney", _money);
        }
    }
    public int knockbackThreshold;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Health = MaxHealth;
        Lives = InitialLives;
        Money = InitialMoney;
        _player = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleHit(int damage, Vector3 impact)
    {
        print("Player hit for " + damage + " damage");
        Health -= damage;
        if (damage >= knockbackThreshold){
            EnterKnockback(impact);
        }
        if (Health <= 0)
        {
            print("Object has died");
        }
    }

    void EnterKnockback(Vector3 impact)
    {
        //print(impact.ToString());
    }
}
