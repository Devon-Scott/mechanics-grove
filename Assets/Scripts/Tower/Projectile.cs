using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float radius;
    public float DestroyTimer;
    [SerializeField] private float _damage;
    public Hitbox hitbox;
    public Rigidbody self;
    [SerializeField] private GameObject _impactEffect;
    [SerializeField] private LayerMask _collidableLayers;
    private MeshRenderer _renderer;
    private bool _hasRenderer;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        hitbox = gameObject.GetComponent<Hitbox>();
        self = gameObject.GetComponent<Rigidbody>();
        _damage = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        hitbox.enabled = false;
        hitbox.parentPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        hitbox.parentPosition = transform.position;
    }

    public void Init(float damage, float timer)
    {
        this._damage = damage;
        this.DestroyTimer = timer;
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
            hitbox.damage = this._damage;
            hitbox.EnableDamage(this._damage);
            ParticleManager.ParticleManagerInit(transform.position, _impactEffect);
            self.isKinematic = true;
            StartCoroutine(DestroyObject());
        }
    }   

    IEnumerator DestroyObject()
    {
        while (DestroyTimer >= 0)
        {
            DestroyTimer -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
