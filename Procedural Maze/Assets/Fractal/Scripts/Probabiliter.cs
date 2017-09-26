using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Probabiliter : ScriptableObject
{
    public abstract float GetSpawningProbability(Fractal parent, int childIndex);
}
