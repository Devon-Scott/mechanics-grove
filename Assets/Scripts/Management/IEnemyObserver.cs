using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyObserver
{
    public void OnEnemySpawn(Enemy enemy);
    public void OnEnemyDeath(Enemy enemy);
    public void OnEnemyVictory(Enemy enemy);
}