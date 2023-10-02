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
    
    // firingPosition is the spawn point of the cannonball 
    public override Vector3 GetFiringVector(Collider target, Vector3 firingPosition)
    {
        float gravity = Physics.gravity.magnitude;
        
        // transform.position + firingPosition gives the point of the cannon's launch
        // This gives the horizontal distance between the muzzle and the enemy
        float horDistance = GetHorizontalDistance(transform.position + firingPosition, target.transform.position);
        
        // The vector from the muzzle to the target
        Vector3 targetVector = target.transform.position - (transform.position + firingPosition);

        // Normalize the launch vector as a direction, later we calculate a scalar to multiply this by
        //Vector3 launchVector = firingPosition.normalized;
        Vector3 launchVector = ((firingPosition + transform.position) - transform.position).normalized;

        // Calculate a a vector along a horizontal plane, against which we measure an angle of launch
        Vector3 horizontalBasis = launchVector;
        horizontalBasis.y = 0;

        // Convert the angle to Radians because Mathf.Tan expects argument in radians
        float angle = Vector3.Angle(launchVector, horizontalBasis) * Mathf.Deg2Rad;
    
        // get the components of the vector for our calculations
        float xDiff = targetVector.x;
        float yDiff = targetVector.y;
        float zDiff = targetVector.z;

        float tOne = 2 * horDistance / gravity;
        float tTwo = Mathf.Tan(angle);
        float tThree = (yDiff / horDistance);
        
        float timeOfFlight = Mathf.Sqrt(Mathf.Abs(tOne * (tTwo - tThree)));
        float Scalar = Mathf.Abs(horDistance / (timeOfFlight * Mathf.Cos(angle)));

        return Scalar * launchVector;
    }
}