using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewWaveEvent
{
    public int currentWave;
    public int maxWaves;

    public NewWaveEvent(int current, int max)
    {
        this.currentWave = current;
        this.maxWaves = max;
    }
}
