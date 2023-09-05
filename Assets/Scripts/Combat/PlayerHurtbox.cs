using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerHurtbox : Hurtbox
{
    private ThirdPersonController player;
    // Start is called before the first frame update
    void Start()
    {
        player = transform.GetComponent<ThirdPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void HandleHit(float damage, Vector3 knockback, float scalar)
    {
        player.OnHit(damage, knockback, scalar);
    }
}
