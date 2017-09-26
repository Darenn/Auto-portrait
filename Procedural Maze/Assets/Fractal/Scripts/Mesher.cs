using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Mesher : ScriptableObject
{
    public abstract Mesh GetRootMesh();
    public abstract Mesh GetMesh(Fractal parent, int childIndex);
}
