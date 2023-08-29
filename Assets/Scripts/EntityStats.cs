using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [SerializeField]
    private float _health;
    public float Health
    {
        get { return _health;}
        set 
        {
            _health = value;
            if (_health <= 0)
            {
                _health = 0;
            }
            // Send a message to the health bar to change the health value
            BroadcastMessage("setHealth", Health);
        }
    }

    public int Damage;

    public float Speed;

    public float TurnSpeed;

    public float Gravity;

    public float knockbackThreshhold;
}