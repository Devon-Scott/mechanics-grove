using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    private float _health;
    private float _maxHealth;
    private GameObject _player;
    private float _distanceToPlayer;
    [SerializeField]
    private Material _bar;
    [SerializeField]
    private EntityStats _stats;
    private int _partialRenderID;

    public float RenderDistance;

    
    void Start()
    {
        _health = gameObject.GetComponentInParent<EntityStats>().Health;
        _maxHealth = _health;
        _player = GameObject.Find("PlayerArmature");
        _bar = GetComponent<MeshRenderer>().material;
        _partialRenderID = Shader.PropertyToID("_PartialRender");
    }

    void Update()
    {
        Vector3 toPlayer = (transform.position - _player.transform.position);
        _distanceToPlayer = toPlayer.magnitude;
        transform.rotation = Quaternion.LookRotation(toPlayer, Vector3.up) * Quaternion.Euler(0, 0, -90);

        if (_distanceToPlayer <= RenderDistance && _health < _maxHealth)
        {
            // Render the health bar
        }
    }

    void setHealth(float health)
    {
        _bar.SetFloat(_partialRenderID, health / _maxHealth);
    }
}