using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicMesher", menuName = "Meshers/Mesher", order = 1)]
public class BasicMesher : Mesher
{
    public Mesh mesh;

    public override Mesh GetMesh(Fractal parent, int childIndex)
    {
        return mesh;
    }

    public override Mesh GetRootMesh()
    {
        return mesh;
    }
}
