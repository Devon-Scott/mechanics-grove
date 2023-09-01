using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public interface IPlayerObserver
{
    public void OnPlayerSpawn(ThirdPersonController player);
    public void OnPlayerHealth(int health);
    public void OnPlayerMoney(int money);
    public void OnPlayerDeath(ThirdPersonController player);
    public void OnPlayerLifeLost(int lives);
    public void OnPlayerVictory(ThirdPersonController player);
}
