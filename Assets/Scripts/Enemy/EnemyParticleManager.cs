using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticleManager : MonoBehaviour
{
    public GameObject child;
    public ParticleSystem childExplosion;
    // Start is called before the first frame update
    void Start()
    {
        childExplosion.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!childExplosion.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
