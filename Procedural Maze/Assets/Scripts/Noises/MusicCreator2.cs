using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MusicCreator2 : MonoBehaviour
{
    public System.Random RandomGenerator { private get; set; }

    private void OnEnable()
    {
        if (RandomGenerator == null)
        {
            RandomGenerator = new System.Random();
        }
        VerseCreator.RandomGenerator = RandomGenerator;
        ChorusCreator.RandomGenerator = RandomGenerator;
        Debug.Assert(VerseCreator.MelodyNotes.Max - VerseCreator.MelodyNotes.Min == 7, "should be one octave above the highest.");
        Debug.Assert(VerseCreator.JointNoteProb + VerseCreator.NoteOutsideTierceProb + VerseCreator.NoteWithinTierceProb + VerseCreator.RepeatNoteProb == 100,
            "The sum of the probabilities for choosing a note in a verse should be equal to 100.");
    }

    [Tooltip("All notes that can be used. Notes should be in the good order of frequency, and all belong to the same scale.")]
    public AudioClip[] NoteSounds;
    public double BPM;
    public double BeatUnit;



    public int BeatsPerTab;

    public int NumberVoices;


    public MusicPartCreator VerseCreator;
    public MusicPartCreator ChorusCreator;
    

    public Music GenerateMusic()
    {
        Music music = new Music();
        music.BPM = BPM;
        music.BeatUnit = BeatUnit;
        music.NotesSound = new List<AudioClip>(NoteSounds);
        List<List<int>> verse = VerseCreator.GeneratePart(BeatsPerTab);
        List<List<int>> chorus = ChorusCreator.GeneratePart(BeatsPerTab);
        music.Partition = new List<List<int>>();
        music.Partition.Add(new List<int>());
        music.Partition.Add(new List<int>());
        AddMusicPartToAnother(music.Partition, verse);
        AddMusicPartToAnother(music.Partition, chorus);
        AddMusicPartToAnother(music.Partition, verse);
        AddMusicPartToAnother(music.Partition, chorus);
        music.VoiceVolumes = new float[] { 0.3f, 0.15f };
        return music;
    }

    

    #region Helpers
    public static List<int> GetJointNotes(int note, int minNote, int maxNote)
    {
        List<int> jointNotes = new List<int>();
        if (note - 1 >= minNote) jointNotes.Add(note - 1);
        if (note + 1 <= maxNote) jointNotes.Add(note + 1);
        return jointNotes;
    }

    public static List<int> GetNotesWithinInterval(int note, int interval, int minNote, int maxNote)
    {
        List<int> NotesWithinTierce = new List<int>();
        for (int i = note - interval; i < note + interval; i++)
            if (i >= minNote && i <= maxNote) NotesWithinTierce.Add(i);
        return NotesWithinTierce;
    }

    public static List<int> GetChord(int note, int maxNote)
    {
        Debug.Assert(note <= maxNote);
        List<int> chord = new List<int>();
        chord.Add(note);
        if (note + 2 <= maxNote) chord.Add(note + 2);
        if (note + 4 <= maxNote) chord.Add(note + 4);
        return chord;
    }

    public void AddMusicPartToAnother(List<List<int>> another, List<List<int>> toAdd)
    {
        for (int i = 0; i < toAdd.Count; i++)
        {
            for (int j = 0; j < toAdd[i].Count; j++)
            {
                another[i].Add(toAdd[i][j]);
            }
        }
    }
    #endregion Helpers
}
