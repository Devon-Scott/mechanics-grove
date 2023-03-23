using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is meant to be used in conjunction with an enemy parent object
// It will tell the parent whether or not there is something nearby that can be targeted

public class EnemyAttackDetector : MonoBehaviour
{

    public SphereCollider self;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other){
        if (other.gameObject.layer == LayerMask.NameToLayer("Character")){
            self.SendMessageUpwards("GetTarget", other);
        }
    }

    void OnTriggerExit(Collider other){
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            self.SendMessageUpwards("RemoveTarget");
        } 
    }
}
