using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float radius;
    public Hitbox hitbox;
    public Rigidbody self;
    private float _destroyTimer;
    private ParticleSystem _impactEffect;
    [SerializeField]
    private LayerMask _layers;

    // Start is called before the first frame update
    void Start()
    {
        hitbox = gameObject.GetComponent<Hitbox>();
        self = gameObject.GetComponent<Rigidbody>();
        _destroyTimer = 0.25f;

        hitbox.enabled = false;
        hitbox.parentPosition = transform.position;
        _impactEffect = gameObject.GetComponentInChildren<ParticleSystem>();
        _impactEffect.Stop();
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
        if ((_layers.value & 1 << other.gameObject.layer) > 0)
        {
            hitbox.enabled = true;
            hitbox.EnableDamage(20);
            _impactEffect.transform.position = transform.position;
            _impactEffect.Play();
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
