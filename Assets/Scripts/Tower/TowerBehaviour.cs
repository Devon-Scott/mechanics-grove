using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

public class TowerBehaviour : MonoBehaviour
{
    [SerializeField]    
    private Projectile _projectile;
    [SerializeField]
    private float _cooldown;
    [SerializeField]
    private float _attackRadius;
    [SerializeField]
    private float _rotationalOffset;
    [SerializeField]
    private float _launchAngle;
    [SerializeField]
    private LayerMask _targetLayers;
    [SerializeField]
    private Vector3[] _projectileSpawnPoints;
    
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
                Vector3 targetDirection = _target.transform.position - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection) * Quaternion.Euler(-_launchAngle, 0, 0) * Quaternion.Euler(0, _rotationalOffset, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4);
                for(int i = 0; i < _numOfSpawnPoints; i++)
                {
                    //_spawnPoints[i] = transform.rotation * Quaternion.Euler(0, -_rotationalOffset, 45) * _projectileSpawnPoints[i];
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
                Gizmos.DrawSphere(spawnPoint, 0.25f);
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
        }
    }

    float GetHorizontalDistance(Vector3 start, Vector3 end)
    {
        Vector3 flatStart = new Vector3(start.x, 0, start.z);
        Vector3 flatEnd = new Vector3(end.x, 0, end.z);
        return Graph.distanceBetween(flatStart, flatEnd);
    }

    IEnumerator Fire()
    {
        while (_target != null)
        {
            float mass = _projectile.self.mass;
            float gravity = Physics.gravity.magnitude;
            float horDistance = GetHorizontalDistance(transform.position, _target.transform.position);

            Vector3 forward = _spawnPoints[0];                
            forward.y = 0;
            float angleInRadians = Vector3.Angle(forward, (transform.position - _spawnPoints[0]));
            
            float launchHeight = transform.position.y;
            float targetHeight = _target.transform.position.y;
            float verDistance = launchHeight - targetHeight;

            float timeOfFlight = 1.5f;
            
            float Vx = horDistance / timeOfFlight;
            float Vy = (verDistance + (0.5f * gravity * timeOfFlight * timeOfFlight)) / timeOfFlight;

            float magnitude = new Vector2(Vx, Vy).magnitude;

            float force = mass * magnitude / timeOfFlight;
            print(force);
            Vector3 launchVector = force * (_spawnPoints[0]);
            
            Instantiate(_projectile, transform.position + _spawnPoints[0], Quaternion.identity).ApplyForce(launchVector);
            // Give projectile target position to travel towards
            yield return new WaitForSeconds(_cooldown);
        }
        _firing = false;
        StopCoroutine(Fire());
    }
}
