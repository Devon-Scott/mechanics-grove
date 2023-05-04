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
    
    private bool _firing;
    private Collider _target;
    
    // Start is called before the first frame update
    void Start()
    {
        _target = null;
        _firing = false;
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
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection) * Quaternion.Euler(0, _rotationalOffset, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4);
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
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);

        Gizmos.color = Color.red;
        foreach(Vector3 point in _projectileSpawnPoints)
        {
            if (_projectile != null)
            {
                Gizmos.DrawWireSphere(point, _projectile.radius);
            }
            else
            {
                Gizmos.DrawWireSphere(point, 0.25f);
            }
        }
    }

    IEnumerator Fire()
    {
        while (_target != null)
        {
            Projectile projectile = GameObject.Instantiate(_projectile, _projectileSpawnPoints[0], Quaternion.identity);
            projectile.ApplyForce(_target.transform.position);
            print("Firing!");
            // Give projectile target position to travel towards
            yield return new WaitForSeconds(_cooldown);
        }
        StopCoroutine(Fire());
    }
}
