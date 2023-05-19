using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuildState : MonoBehaviour
{
    public GameObject[] Towers;
    public GameObject[] Meshes;
    
    [HideInInspector]
    public bool BuildState;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (BuildState)
        {
            
        }
    }
}
