using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicProbabiliter", menuName = "Probabiliters/BasicProbabiliter", order = 1)]
public class BasicProbabiliter : Probabiliter {

    public float probability;

    public override float GetSpawningProbability(Fractal parent, int childIndex)
    {
        return probability;
    }
}
