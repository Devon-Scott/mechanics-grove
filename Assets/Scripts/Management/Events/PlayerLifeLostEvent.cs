using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerHealthZeroEvent
{
    public ThirdPersonController player;
    public PlayerStats stats;
    
    public PlayerHealthZeroEvent(ThirdPersonController player)
    {
        this.player = player;
        this.stats = player.GetComponent<PlayerStats>();
    }
}
