using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour 
{
    public CustomEvent NextWave;
    public CustomEvent PlayerDeath;
    public CustomEvent FirstPlayerSpawn;
    public CustomEvent GameOver;    
    public CustomEvent Victory;
    public CustomEvent FadeOut;
    public CustomEvent FadeIn;

    void Awake()
    {
        NextWave = ScriptableObject.CreateInstance<CustomEvent>();
        FirstPlayerSpawn = ScriptableObject.CreateInstance<CustomEvent>();
        PlayerDeath = ScriptableObject.CreateInstance<CustomEvent>();
        GameOver = ScriptableObject.CreateInstance<CustomEvent>();
        FadeOut = ScriptableObject.CreateInstance<CustomEvent>();
        FadeIn = ScriptableObject.CreateInstance<CustomEvent>();
        Victory = ScriptableObject.CreateInstance<CustomEvent>();
    }
}

