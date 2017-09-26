using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class MusicCreator : MonoBehaviour
{
    public System.Random RandomGenerator { private get; set; }

    public int BeatPerTab;

    public Vector2 TabPerIntro;
    public Vector2 TabPerChorus;
    public Vector2 TabPerVerse;
    public Vector2 TabPerBridge;
    public Vector2 TabPerOutro;

    private void Awake()
    {
        if (RandomGenerator == null)
        {
            RandomGenerator = new System.Random();
        }
    }

    /// <summary>
    /// Generate a new music based on the given parameters.
    /// Divide the music in
    /// </summary>
    /// <param name="notesRange">The range of notes that will be used (notesRange included). 
    /// Must be superior to three (0 and 1 are reserved for silence and continue)
    /// </param>
    /// <param name="numberBeats"></param>
    /// <returns></returns>
    public List<int> GenerateMusic(int notesRange)
    {
        Assert.IsTrue(notesRange > 3);
        
        List<int> music = new List<int>();
        int numberTabPerIntro = RandomGenerator.Next((int)TabPerIntro.x, (int)TabPerIntro.y);
        List<int> intro = GeneratePart(notesRange, numberTabPerIntro);
        int numberTabPerChorus = RandomGenerator.Next((int)TabPerChorus.x, (int)TabPerChorus.y);
        List<int> chorus = GeneratePart(notesRange, numberTabPerChorus);
        int numberTabPerVerse = RandomGenerator.Next((int)TabPerVerse.x, (int)TabPerVerse.y);
        List<int> verse = GeneratePart(notesRange, numberTabPerVerse);
        int numberTabPerBridge = RandomGenerator.Next((int)TabPerBridge.x, (int)TabPerBridge.y);
        List<int> bridge = GeneratePart(notesRange, numberTabPerBridge);
        int numberTabPerOutro = RandomGenerator.Next((int)TabPerOutro.x, (int)TabPerOutro.y);
        List<int> outro = GeneratePart(notesRange, numberTabPerOutro);

        music.AddRange(intro);
        music.AddRange(chorus);
        music.AddRange(verse);
        music.AddRange(chorus);
        music.AddRange(verse);
        music.AddRange(bridge);
        music.AddRange(chorus);
        music.AddRange(outro);

        return music;
    }

    public Vector2 NumberNotesChangedInMelody;

    private List<int> GeneratePart(int notesRange, int numberTabs)
    {
        List<int> part = new List<int>();
        part.Add(GenerateNote(notesRange));
        int numberCouple = numberTabs / 2;
        int numberAloneCouple = numberCouple % 2;
        int aloneTab = numberTabs % 2;

        List<int> baseMelody = GenerateMelody(notesRange, 2);

        for (int i = 0; i < numberCouple; i += 2)
        {
            part.AddRange(baseMelody);
            part.AddRange(GenerateModifiedMelody(notesRange, baseMelody));
        }

        for (int i = 0; i < numberAloneCouple; i++)
        {
            List<int> melody = GenerateMelody(notesRange, 1);
            part.AddRange(melody);
            part.AddRange(GenerateModifiedMelody(notesRange, melody));
        }

        for (int i = 0; i < aloneTab; i++)
        {
            List<int> melody = GenerateMelody(notesRange, 1);
            part.AddRange(melody);
        }

        return part;
    }

    private List<int> GenerateMelody(int notesRange, int numberTab)
    {
        List<int> melody = new List<int>();
        melody.Add(GenerateNote(notesRange));
        for (int j = 1; j < 2 * BeatPerTab; j++)
        {
            melody.Add(GenerateNote(notesRange, melody[melody.Count - 1]));
        }
        return melody;
    }

    private List<int> GenerateModifiedMelody(int notesRange, List<int> melody)
    {
        List<int> melodyModified = new List<int>(melody);
        for (int j = 0; j < RandomGenerator.Next((int)NumberNotesChangedInMelody.x, (int)NumberNotesChangedInMelody.y); j++)
        {
            int index = RandomGenerator.Next(0, melodyModified.Count - 1);
            melodyModified[index] = GenerateNote(notesRange, melodyModified[Mathf.Max(0, index - 1)]);
        }
        return melodyModified;
    }


    [Range(0f, 1f)]
    public float ChoosingSilenceOrContinueProbability;
    [Range(0f, 1f)]
    public float ChoosingDisjointNoteProbability;
    [Range(0f, 1f)]
    public float ChoosingNoteInOctaveProbability;
    [Range(0f, 1f)]
    public float ChoosingTwiceSameNoteProbability;

    private int GenerateNote(int notesRange, int previousNote)
    {
        if (RandomGenerator.NextDouble() < ChoosingSilenceOrContinueProbability)
        {
            // Choose a silence or a link
            return RandomGenerator.Next(0, 2);
        }
        else
        {
            if (RandomGenerator.NextDouble() < ChoosingDisjointNoteProbability)
            {
                // Choose a disjoint note
                if (RandomGenerator.NextDouble() < ChoosingNoteInOctaveProbability)
                {
                    // Choose a note within the octave
                    return RandomGenerator.Next(Mathf.Max(2, previousNote - 6), Mathf.Min(notesRange, previousNote + 6));
                }
                else
                {
                    // Choose a note out of the octave
                    int[] lowerRange = { 2, Mathf.Max(2, previousNote - 6) };
                    int[] upperRange = { Mathf.Min(notesRange, previousNote + 6), notesRange };
                    int[] chosenRange = (RandomGenerator.NextDouble() < 0.5) ? lowerRange : upperRange;
                    return RandomGenerator.Next(chosenRange[0], chosenRange[1]);
                }
            }
            else
            {
                // Choose a joint note
                if (RandomGenerator.NextDouble() < ChoosingTwiceSameNoteProbability)
                {
                    return previousNote;
                }
                else
                {
                    // TODO temp fix for if it's a silence or continue
                    if (previousNote < 2) previousNote = RandomGenerator.Next(2, notesRange);
                    return (RandomGenerator.NextDouble() < 0.5f) ? Mathf.Max(2, previousNote - 1) : Mathf.Min(notesRange, previousNote + 1);
                }
            }
        }
    }

    private int GenerateNote(int notesRange)
    {
        int note = RandomGenerator.Next(0, notesRange + 1);
        return GenerateNote(notesRange, note);
    }
}
