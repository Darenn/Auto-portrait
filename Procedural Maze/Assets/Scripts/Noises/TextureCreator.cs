using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TextureCreator : MonoBehaviour {

	[Range(30, 1920)]
	public int resolution = 256;

	public float frequency = 1f;

	[Range(1, 8)]
	public int octaves = 1;

	[Range(1f, 4f)]
	public float lacunarity = 2f;

	[Range(0f, 1f)]
	public float persistence = 0.5f;

	[Range(1, 3)]
	public int dimensions = 3;

	public NoiseMethodType type;

	public Gradient coloring;

    public bool ApplyRandomParametersOnEnable = false;

	private Texture2D texture;
	
	private void Awake () {
        
        if (ApplyRandomParametersOnEnable)
        {
            ApplyRandomParameters();
        }
		if (texture == null) {
            texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
			texture.name = "Procedural Texture";
			texture.wrapMode = TextureWrapMode.Clamp;
			texture.filterMode = FilterMode.Trilinear;
			texture.anisoLevel = 9;
			GetComponent<MeshRenderer>().material.mainTexture = texture;
            coloring = new Gradient();
		}
		FillTexture();
	}
	
	public void FillTexture () {
		if (texture.width != resolution) {
			texture.Resize(resolution, resolution);
		}
		
		Vector3 point00 = transform.TransformPoint(new Vector3(-0.5f,-0.5f));
		Vector3 point10 = transform.TransformPoint(new Vector3( 0.5f,-0.5f));
		Vector3 point01 = transform.TransformPoint(new Vector3(-0.5f, 0.5f));
		Vector3 point11 = transform.TransformPoint(new Vector3( 0.5f, 0.5f));

		NoiseMethod method = Noise.methods[(int)type][dimensions - 1];
		float stepSize = 1f / resolution;
		for (int y = 0; y < resolution; y++) {
			Vector3 point0 = Vector3.Lerp(point00, point01, (y + 0.5f) * stepSize);
			Vector3 point1 = Vector3.Lerp(point10, point11, (y + 0.5f) * stepSize);
			for (int x = 0; x < resolution; x++) {
				Vector3 point = Vector3.Lerp(point0, point1, (x + 0.5f) * stepSize);
				float sample = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
				if (type != NoiseMethodType.Value) {
					sample = sample * 0.5f + 0.5f;
				}
				texture.SetPixel(x, y, coloring.Evaluate(sample));
			}
		}
		texture.Apply();
	}

    private void ApplyRandomParameters()
    {
        resolution = 400;
        frequency = Random.Range(1, 50);
        octaves = Random.Range(1, 8);
        lacunarity = Random.Range(1f, 4f);
        persistence = Random.Range(0f, 1f);
        dimensions = Random.Range(1, 3);
        type = (NoiseMethodType)Random.Range((int)NoiseMethodType.Perlin, (int)NoiseMethodType.Perlin);
        GradientColorKey[] gradientColorKeys = new GradientColorKey[Random.Range(3, 6)];
        GradientAlphaKey[] gradientAlphaKeys = new GradientAlphaKey[2];
        gradientAlphaKeys[0] = new GradientAlphaKey();
        gradientAlphaKeys[1] = new GradientAlphaKey();
        gradientAlphaKeys[0].time = 0;
        gradientAlphaKeys[0].alpha = 255;
        gradientAlphaKeys[1].time = 1;
        gradientAlphaKeys[1].alpha = 255;
        for (int i = 0; i < gradientColorKeys.Length; i++)
        {
            GradientColorKey gck = new GradientColorKey();
            gck.color = Random.ColorHSV();
            gck.time = Random.Range(0f, 1f);
            gradientColorKeys[i] = gck;
        }
        coloring.SetKeys(gradientColorKeys, gradientAlphaKeys);
    }
}