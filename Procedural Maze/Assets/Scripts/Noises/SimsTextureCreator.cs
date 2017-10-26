using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimsTextureCreator : MonoBehaviour {
    [SerializeField] private string FirstName;
    [SerializeField] private string LastName;

    [Range(1, 10)]
    [SerializeField]
    private int Shy_Outgoing;
    [Range(1, 10)]
    [SerializeField]
    private int Sloppy_Neat;
    [Range(1, 10)]
    [SerializeField]
    private int Lazy_Active;
    [Range(1, 10)]
    [SerializeField]
    private int Serious_Playful;
    [Range(1, 10)]
    [SerializeField]
    private int Grouchy_Nice;

    private TextureCreator m_textureCreator;

    public void GenerateTexture(string firstName, string lastName, int shy_Outgoing, int sloppy_Neat, int lazy_Active, int serious_Playful, int grouchy_Nice)
    {
        FirstName = firstName;
        LastName = lastName;
        Shy_Outgoing = shy_Outgoing;
        Sloppy_Neat = sloppy_Neat;
        Lazy_Active = lazy_Active;
        Serious_Playful = serious_Playful;
        Grouchy_Nice = grouchy_Nice;
        GenerateTexture();
    }

    public void GenerateTexture()
    {
        Random.InitState(FirstName.GetHashCode() + LastName.GetHashCode());
        if (m_textureCreator == null)
            m_textureCreator = gameObject.AddComponent<TextureCreator>();

        m_textureCreator.resolution = Mathf.RoundToInt(Map(400, 1000, 1, 10, Shy_Outgoing));
        m_textureCreator.frequency = Map(1, 25, 1, 10, Serious_Playful);
        m_textureCreator.octaves = Mathf.RoundToInt(Map(1, 6, 1, 10, Grouchy_Nice));
        m_textureCreator.lacunarity = Map(1, 4, 1, 10, Lazy_Active);
        m_textureCreator.persistence = Map(0, 1, 1, 10, Sloppy_Neat);

        if (Serious_Playful <= 3) m_textureCreator.dimensions = 1;
        else if (Serious_Playful <= 7) m_textureCreator.dimensions = 2;
        else m_textureCreator.dimensions = 3;

        if (Lazy_Active < 5) m_textureCreator.type = NoiseMethodType.Value;
        else m_textureCreator.type = NoiseMethodType.Perlin;

        // Color
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
        m_textureCreator.coloring.SetKeys(gradientColorKeys, gradientAlphaKeys);
        m_textureCreator.FillTexture();
    }

    public float Map(float from, float to, float from2, float to2, float value)
    {
        if (value <= from2)
            return from;
        else if (value >= to2)
            return to;
        return (to - from) * ((value - from2) / (to2 - from2)) + from;
    }
}
