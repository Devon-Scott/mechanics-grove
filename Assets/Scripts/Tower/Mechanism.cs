using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyUtils.Graph;

public abstract class Mechanism : MonoBehaviour
{
    public float VerticalAngleOffset;
    public float RotationalOffset;
    public float launchExponent;

    void Start()
    {
        
    }
    
    protected virtual float GetHorizontalDistance(Vector3 start, Vector3 end)
    {
        Vector3 flatStart = new Vector3(start.x, 0, start.z);
        Vector3 flatEnd = new Vector3(end.x, 0, end.z);
        return Graph.distanceBetween(flatStart, flatEnd);
    }

    public virtual Quaternion GetAimRotation(Collider target){return Quaternion.identity;}
    public virtual Vector3 GetFiringVector(Collider target, Vector3 firingPosition){return new Vector3(1, 1, 1);}
}