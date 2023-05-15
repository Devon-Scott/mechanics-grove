using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
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

    public static void ParticleManagerInit(Vector3 location, GameObject effect)
    {
        GameObject myEffect = new GameObject("Effect");
        GameObject.Instantiate(effect, location, Quaternion.identity, myEffect.transform);
        ParticleManager reference = myEffect.AddComponent<ParticleManager>();
        reference.child = myEffect;
        reference.childExplosion = myEffect.GetComponentInChildren<ParticleSystem>();
    }
}
