using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Fractal : MonoBehaviour {

    public int maxDepth;

    public float minTimeBetweenChildCreation;
    public float maxTimeBetweenChildCreation;

    public int Depth
    {
        get
        {
            return depth;
        }
    }

    public bool IsRoot
    {
        get
        {
            return depth == 0;
        }
    }

    public GameObject fractalModel;

    public Transformer transformer;
    public Materialer materialer;
    public Probabiliter probabiliter;
    public Mesher mesher;

    private int depth = 0;
    private Material material;
    private float spawnProbability = 1;
    private Mesh mesh;
    private Dictionary<int, Transform> childrenTransforms;

    void Start () {
        if (IsRoot)
        {
            material = materialer.GetRootMaterial();
            mesh = mesher.GetRootMesh();
            childrenTransforms = transformer.GetChildrenTransforms(fractalModel);
            fractalModel.SetActive(false); // We don't want to see the model
        }
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = material;
        if (depth < maxDepth)
        {
            StartCoroutine(CreateChildren());
        }
	}

    private void Initialize(Fractal parent, int childIndex)
    {
        transformer = parent.transformer;
        materialer = parent.materialer;
        probabiliter = parent.probabiliter;
        mesher = parent.mesher;

        // Init fractal attributes
        mesh = mesher.GetMesh(parent, childIndex);
        material = parent.material;
        maxDepth = parent.maxDepth;
        depth = parent.depth + 1;
        minTimeBetweenChildCreation = parent.minTimeBetweenChildCreation;
        maxTimeBetweenChildCreation = parent.maxTimeBetweenChildCreation;        
        spawnProbability = probabiliter.GetSpawningProbability(parent, childIndex);
        childrenTransforms = parent.childrenTransforms;

        // Init game object
        transform.parent = parent.transform;
        transform.localScale = transformer.GetLocalScale(parent, childIndex, childrenTransforms);
        transform.localPosition = transformer.GetLocalPosition(parent, childIndex, childrenTransforms);
        transform.localRotation = transformer.GetLocalRotation(parent, childIndex, childrenTransforms);
        material = materialer.GetMaterial(parent, childIndex);
    }

    private IEnumerator CreateChildren()
    {
        for (int i = 0; i < childrenTransforms.Count; i++)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenChildCreation, maxTimeBetweenChildCreation));
            if (Random.value < spawnProbability)
            {
                new GameObject("FractalChild").AddComponent<Fractal>().Initialize(this, i);
            }
        }
    }
}
