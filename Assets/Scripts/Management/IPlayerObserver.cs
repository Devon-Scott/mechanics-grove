using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public interface IPlayerObserver
{
    public void OnPlayerSpawn(ThirdPersonController player);
    public void OnPlayerDeath(ThirdPersonController player);
    public void OnPlayerVictory(ThirdPersonController player);
}
