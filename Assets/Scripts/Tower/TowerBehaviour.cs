using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

public class TowerBehaviour : MonoBehaviour
{
    [Header("Stats")]
    public int Cost;
    public int Damage;
    [SerializeField] private float _projectileTimer;
    [SerializeField] private Projectile _projectile;
    [SerializeField] private float _cooldown;
    [SerializeField] private float _attackRadius;
    [SerializeField] private float _rotationalOffset;
    [SerializeField] private float _launchAngle;
    [SerializeField] private LayerMask _targetLayers;
    [SerializeField] private Vector3[] _projectileSpawnPoints;
    [SerializeField] private Mechanism _towerMechanism;
    
    private Vector3[] _spawnPoints;
    private int _numOfSpawnPoints;
    
    private bool _firing;
    private Collider _target;

    void Awake()
    {
        _numOfSpawnPoints = _projectileSpawnPoints.GetLength(0);
        _spawnPoints = new Vector3[_numOfSpawnPoints];
        for(int i = 0; i < _numOfSpawnPoints; i++)
        {
            _spawnPoints[i] = _projectileSpawnPoints[i];
        }
        _target = null;
        _firing = false;
    }
    
    // Need to have projectile spawner be child of tower weapon
    // Projectile spawner needs to do the physics calculation for sending cannonball to target
    // Start is called before the first frame update
    void Start()
    {
        _towerMechanism = gameObject.GetComponent<Mechanism>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_target == null)
        {
            _target = FindTarget();
        }
        else if (!_firing)
        {
            StartCoroutine(Fire());
            _firing = true;
        }
        else
        {
            // Check if target is in range, if not, set to null
            if (!TargetIsInRange())
            {
                _target = null;
            }
            else
            {
                transform.rotation = _towerMechanism.GetAimRotation(_target);
                for(int i = 0; i < _numOfSpawnPoints; i++)
                {
                    _spawnPoints[i] = transform.rotation * Quaternion.Euler(0, -_rotationalOffset, 0) * _projectileSpawnPoints[i];
                }
                
            }
        }
    }

    private Collider FindTarget()
    {

        // Spherecast attack radius with Enemy layer mask
        Collider[] targets = Physics.OverlapSphere(transform.position, _attackRadius, _targetLayers);
        int numOfTargets = targets.GetLength(0);
        if (numOfTargets > 0)
        {
            return targets[0];
        }
        // Sort colliders by distance to End Point
        return null;
    }

    private bool TargetIsInRange()
    {
        return Graph.distanceBetween(transform.position, _target.transform.position) <= _attackRadius;
    }

    void OnDrawGizmosSelected()
    {
        if (_spawnPoints != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _attackRadius);
            foreach(Vector3 point in _spawnPoints)
            {
                Gizmos.color = Color.red;
                Vector3 spawnPoint = transform.position + point;
                Gizmos.DrawSphere(spawnPoint, 0.2f);
                Gizmos.DrawLine(transform.position, spawnPoint);

                Gizmos.color = Color.blue;
                Vector3 forward = point;
                forward.y = 0;
                Gizmos.DrawSphere(transform.position + forward, 0.25f);
                Gizmos.DrawLine(transform.position, transform.position + forward);    
            }
        }
        if (_target != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_target.transform.position, 1f);
            Gizmos.DrawLine(transform.position + _spawnPoints[0], _target.transform.position);
        }
    }

    IEnumerator Fire()
    {
        yield return new WaitForSeconds(_cooldown);
        while (_target != null)
        {
            foreach (Vector3 point in _spawnPoints)
            {
                Vector3 direction;
                if (_target != null){
                    direction = _towerMechanism.GetFiringVector(_target, point);
                }
                else {
                    break;
                }
            
                Projectile newProjectile = Instantiate(_projectile, transform.position + point, Quaternion.identity);
                newProjectile.ApplyForce(direction);
                newProjectile.Init(this.Damage, this._projectileTimer);
                yield return new WaitForSeconds(_cooldown);
            }
        }
        _firing = false;
        StopCoroutine(Fire());
    }
}
