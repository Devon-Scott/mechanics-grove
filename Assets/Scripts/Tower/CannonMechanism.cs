using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

public class CannonMechanism : Mechanism
{
    

    public override Quaternion GetAimRotation(Collider target)
    {
        Vector3 targetDirection = target.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection) * Quaternion.Euler(VerticalAngleOffset, 0, 0) * Quaternion.Euler(0, RotationalOffset, 0);
        return Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 4);
    }
    
    public override Vector3 GetFiringVector(Collider target, Vector3 firingPosition)
    {
        
        //float mass = _projectile.self.mass;
        float gravity = Physics.gravity.magnitude;
        float distance = Graph.distanceBetween(transform.position + firingPosition, target.transform.position);
        float horDistance = GetHorizontalDistance(transform.position, target.transform.position);
        
        Vector3 targetVector = target.transform.position - (transform.position + firingPosition);
        Vector3 launchVector = firingPosition.normalized;

        float xDiff = targetVector.x;
        float yDiff = targetVector.y;
        float zDiff = targetVector.z;

        float dot = Vector3.Dot(targetVector, launchVector);

        float Scalar = Mathf.Sqrt(Vector3.Dot(targetVector, targetVector) / dot);

        return Mathf.Pow(Scalar, launchExponent) * launchVector;
    }
}