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
        }
    }

    public int Damage;

    public float Speed;

    public float TurnSpeed;

    public float Gravity;

    public float knockbackThreshhold;
}