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
    private LayerMask _targetLayers;
    [SerializeField]
    private Vector3[] _projectileSpawnPoints;
    private Vector3[] _spawnPoints;
    private int _numOfSpawnPoints;
    
    private bool _firing;
    private Collider _target;
    
    // Need to have projectile spawner be child of tower weapon
    // Projectile spawner needs to do the physics calculation for sending cannonball to target
    // Start is called before the first frame update
    void Start()
    {
        _target = null;
        _firing = false;
        _numOfSpawnPoints = _projectileSpawnPoints.GetLength(0);
        _spawnPoints = new Vector3[_numOfSpawnPoints];
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
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection) * Quaternion.Euler(-30, 0, 0) * Quaternion.Euler(0, _rotationalOffset, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4);
                for(int i = 0; i < _numOfSpawnPoints; i++)
                {
                    _spawnPoints[i] = transform.rotation * Quaternion.Euler(0, -_rotationalOffset, 45) * _projectileSpawnPoints[i];
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

            Gizmos.color = Color.red;
            foreach(Vector3 point in _spawnPoints)
            {
                Vector3 spawnPoint = transform.position + point;
                Gizmos.DrawSphere(spawnPoint, 0.25f);
            }
        }
        if (_target != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_target.transform.position, 1f);
        }
    }

    IEnumerator Fire()
    {
        while (_target != null)
        {
            Vector3 launchDirection = (_target.transform.position - transform.position);
            Vector3 launchVector = (_spawnPoints[0] - transform.position);
            float launchPower = 24 * (launchDirection).magnitude;
            
            Vector3 verticalComponent = new Vector3(0, (_target.transform.position - transform.position).magnitude, 0) * 4;
            Vector3 forceVector = 3.5f * (_target.transform.position - transform.position).magnitude * (_target.transform.position - transform.position) + verticalComponent;
            
            Instantiate(_projectile, _spawnPoints[0] + transform.position, Quaternion.identity).ApplyForce(launchPower * launchVector);
            print("Firing!");
            // Give projectile target position to travel towards
            yield return new WaitForSeconds(_cooldown);
        }
        _firing = false;
        StopCoroutine(Fire());
    }
}
