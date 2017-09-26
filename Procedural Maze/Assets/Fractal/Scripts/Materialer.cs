using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Materialer : ScriptableObject
{
    /// <summary>
    /// Returns the material for the given child.
    /// </summary>
    /// <param name="parent">The parent of the child.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns></returns>
    public abstract Material GetMaterial(Fractal parent, int childIndex);

    /// <summary>
    /// Returns the material for the root. Should be called for the root's material.
    /// </summary>
    /// <returns>The material for the root.</returns>
    public abstract Material GetRootMaterial();
}
