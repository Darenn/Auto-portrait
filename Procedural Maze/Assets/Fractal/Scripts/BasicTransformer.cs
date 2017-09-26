using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "BasicTransformer", menuName = "Transformers/BasicTransformer", order = 1)]
public class BasicTransformer : Transformer
{
    /*public Vector3[] childDirections = {
        Vector3.up,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    public Vector3[] childOrientations = {
        Vector3.zero,
        new Vector3 (0f, 0f, -90f),
        new Vector3 (0f, 0f, 90f),
        new Vector3 (90f, 0f, 0f),
        new Vector3 (-90f, 0f, 0f)
    };

    // Assert.AreEqual(childDirections.Length, childOrientations.Length, "Should have as much directions as orientations.");

    public float childScale;

    public override Vector3 GetLocalPosition(Fractal parent, int childIndex)
    {
        return childDirections[childIndex] * (0.5f + 0.5f * childScale);
    }

    public override Quaternion GetLocalRotation(Fractal parent, int childIndex)
    {
        return Quaternion.Euler(childOrientations[childIndex]);
    }

    public override Vector3 GetLocalScale(Fractal parent, int childIndex)
    {
        return Vector3.one * childScale;
    }*/
    public override Vector3 GetLocalPosition(Fractal parent, int childIndex, Dictionary<int, Transform> childrenTransforms)
    {
        return childrenTransforms[childIndex].localPosition;
    }

    public override Quaternion GetLocalRotation(Fractal parent, int childIndex, Dictionary<int, Transform> childrenTransforms)
    {
        return childrenTransforms[childIndex].localRotation;
    }

    public override Vector3 GetLocalScale(Fractal parent, int childIndex, Dictionary<int, Transform> childrenTransforms)
    {
        return childrenTransforms[childIndex].localScale;
    }
}
