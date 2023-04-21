using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyObserver
{
    public void OnEnemyDeath(Enemy enemy);
}