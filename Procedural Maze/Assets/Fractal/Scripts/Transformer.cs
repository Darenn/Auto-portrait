using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Transformer : ScriptableObject
{
    public abstract Vector3 GetLocalPosition(Fractal parent, int childIndex, Dictionary<int, Transform> childrenTransforms);
    public abstract Quaternion GetLocalRotation(Fractal parent, int childIndex, Dictionary<int, Transform> childrenTransforms);
    public abstract Vector3 GetLocalScale(Fractal parent, int childIndex, Dictionary<int, Transform> childrenTransforms);

    public Dictionary<int, Transform> GetChildrenTransforms(GameObject fractalModel)
    {
        // Key is the child index
        Dictionary<int, Transform> childrenTransforms = new Dictionary<int, Transform>();

        for (int i = 0; i < fractalModel.transform.childCount; i++)
        {
            childrenTransforms[i] = fractalModel.transform.GetChild(i);
        }

        return childrenTransforms;
    }
}
