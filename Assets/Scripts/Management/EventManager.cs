using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour 
{
    public CustomEvent NextWave;
    public CustomEvent PlayerDeath;
    public CustomEvent GameOver;    

    void Awake()
    {
        NextWave = ScriptableObject.CreateInstance<CustomEvent>();
        PlayerDeath = ScriptableObject.CreateInstance<CustomEvent>();
        GameOver = ScriptableObject.CreateInstance<CustomEvent>();
    }
}

