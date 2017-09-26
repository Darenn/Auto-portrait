using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BasicMaterialer", menuName = "Materialers/BasicMaterialer", order = 1)]
public class BasicMaterialer : Materialer
{
    public Color rootColor;
    public Color leafColor;
    public Material material;

    public override Material GetMaterial(Fractal parent, int childIndex)
    {
        Material m = new Material(material);
        m.color = Color.Lerp(rootColor, leafColor, (float)parent.Depth / parent.maxDepth);
        return m;
    }
      
    public override Material GetRootMaterial()
    {
        Material m = new Material(material);
        m.color = rootColor;
        return m;
    }
}
