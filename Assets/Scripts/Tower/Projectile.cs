using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float radius;
    public Hitbox hitbox;
    public Rigidbody self;
    private float _destroyTimer;
    [SerializeField] private GameObject _impactEffect;
    [SerializeField] private LayerMask _collidableLayers;
    private MeshRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        hitbox = gameObject.GetComponent<Hitbox>();
        self = gameObject.GetComponent<Rigidbody>();
        _destroyTimer = 0.25f;

        hitbox.enabled = false;
        hitbox.parentPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        hitbox.parentPosition = transform.position;
    }

    public void ApplyForce(Vector3 direction)
    {
        self.AddForce(direction, ForceMode.VelocityChange);
    }

    void OnDrawGizmos()
    {
        if (hitbox.enabled)
        {
            Gizmos.color = Color.red;
           // Gizmos.DrawSphere(transform.position, 3f);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if ((_collidableLayers.value & 1 << other.gameObject.layer) > 0)
        {
            _renderer.enabled = false;
            hitbox.enabled = true;
            hitbox.EnableDamage(20);
            ParticleManager.ParticleManagerInit(transform.position, _impactEffect);
            self.isKinematic = true;
            StartCoroutine(DestroyObject());
        }
    }   

    IEnumerator DestroyObject()
    {
        while (_destroyTimer >= 0)
        {
            _destroyTimer -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
