using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class MusicPartCreator {

    public System.Random RandomGenerator;
    
    public int NumberTab;
    public int NumberTabMelody;
    [Tooltip("The lowest and highest note used in verses, should be one octave between them. Correspond to a note of Notes.")]
    public Interval SupportNotes;
    public Interval NumberSilenceInMelody;
    public Interval NumberContinueInMelody;
    public Interval NumberNotesModifiedInMelody;
    public Interval NumberSilenceInSupportPattern;
    public Interval NumberContinueInSupportPattern;
    [Tooltip("The lowest and highest note used in verses, should be one octave below the highest. Correspond to a note of Notes.")]
    public Interval MelodyNotes;

    [Header("Note choosing probabilities")]
    [Tooltip("Probability that the next note generated will be a joint note of the last one.")]
    [Range(0, 100)]
    public int JointNoteProb;
    [Tooltip("Probability that the next note generated will be within the tierce interval of the last one," +
        " but not joint (second, or tierce)")]
    [Range(0, 100)]
    public int NoteWithinTierceProb;
    [Tooltip("Probability that the next note generated will be withing the octace, but outside of the tierce.")]
    [Range(0, 100)]
    public int NoteOutsideTierceProb;
    [Tooltip("Probability that the next note generated will be the same as the last one.")]
    [Range(0, 100)]
    public int RepeatNoteProb;

    public List<List<int>> GeneratePart(int beatsPerTab)
    {
        List<List<int>> part = new List<List<int>>();
        List<int> partMelody = GenerateMelody(beatsPerTab);
        List<int> support = GenerateSupport(beatsPerTab, partMelody);
        List<int> modifiedMelody = GetModifiedMelody(partMelody, MelodyNotes);
        partMelody.AddRange(modifiedMelody);
        part.Add(partMelody);
        support.AddRange(support);
        part.Add(support);
        return part;
    }

    private List<int> GenerateMelody(int beatsPerTab)
    {
        // Create a probability array
        const int joint = 1; const int withinTierce = 2; const int outsideTierce = 3; const int repeat = 4;
        Dictionary<int, int> cases = new Dictionary<int, int>();
        cases.Add(joint, JointNoteProb);
        cases.Add(withinTierce, NoteWithinTierceProb);
        cases.Add(outsideTierce, NoteOutsideTierceProb);
        cases.Add(repeat, RepeatNoteProb);
        int[] probArray = Probability.CreateProbabilityArray(cases);

        // Creating the melody by adding a first random note
        int firstNote = RandomGenerator.Next(MelodyNotes.Min, MelodyNotes.Max);
        List<int> melody = new List<int>();
        melody.Add(firstNote);

        // Add notes to all beats using given probabilities
        for (int i = 1; i < NumberTabMelody * beatsPerTab; i++)
        {
            int choice = probArray[RandomGenerator.Next(0, 100)];
            int note = -1;
            int lastNote = melody[i - 1];
            switch (choice)
            {
                case joint:
                    List<int> jointNotes = MusicCreator2.GetJointNotes(lastNote, MelodyNotes.Min, MelodyNotes.Max);
                    note = jointNotes[RandomGenerator.Next(0, jointNotes.Count)];
                    break;
                case withinTierce:
                    List<int> withinTierceNotes = MusicCreator2.GetNotesWithinInterval(lastNote, 3, MelodyNotes.Min, MelodyNotes.Max);
                    List<int> withinOneNote = MusicCreator2.GetNotesWithinInterval(lastNote, 1, MelodyNotes.Min, MelodyNotes.Max);
                    withinTierceNotes.RemoveAll(n => withinOneNote.Contains(n));
                    note = withinTierceNotes[RandomGenerator.Next(0, withinTierceNotes.Count)];
                    break;
                case outsideTierce:
                    List<int> withinTierceN = MusicCreator2.GetNotesWithinInterval(lastNote, 3, MelodyNotes.Min, MelodyNotes.Max);
                    List<int> withinOctave = MusicCreator2.GetNotesWithinInterval(lastNote, 7, MelodyNotes.Min, MelodyNotes.Max);
                    withinOctave.RemoveAll(n => withinTierceN.Contains(n));
                    note = withinOctave[RandomGenerator.Next(0, withinOctave.Count)];
                    break;
                case repeat:
                    note = lastNote;
                    break;
                default:
                    Debug.LogError("Problem with the choice : " + choice);
                    break;
            }
            melody.Add(note);
        }

        // TODO add silences and continue by avoiding making too large silence or too large notes

        // Add some silences randomly by replacing notes
        int numberSilence = RandomGenerator.Next(NumberSilenceInMelody.Min, NumberSilenceInMelody.Max);
        PutNoteRandomlyIn(MusicPlayer.SILENCE, melody, numberSilence);

        // Add some continue randomly by replacing notes
        int numberContinues = RandomGenerator.Next(NumberContinueInMelody.Min, NumberContinueInMelody.Max);
        PutNoteRandomlyIn(MusicPlayer.CONTINUE, melody, numberContinues);

        return melody;
    }

    /// <summary>
    /// Generate a support for the given melody.
    /// Will take the most played note of the melody, and choose this note - 1 * octave chord as base chord.
    /// For each tab of the melody, will choose a different chord, based on the base chord. (see quintes cycles)
    /// https://composer-sa-musique.fr/comment-creer-une-musique-en-5-minutes-le-cycle-des-quintes/
    /// Notes of the chosen chord will be used to make de melody of the support.
    /// </summary>
    /// <param name="beatsPerTab"></param>
    /// <param name="melodyToSupport"></param>
    /// <returns></returns>
    private List<int> GenerateSupport(int beatsPerTab, List<int> melodyToSupport)
    {
        List<int> melodyToSupportCopy = new List<int>(melodyToSupport);
        melodyToSupportCopy.RemoveAll(note => note < 0);
        int mostPlayedNote = melodyToSupportCopy.GroupBy(note => note).OrderByDescending(x => x.Count()).First().Key;
        int checkInBounds = (mostPlayedNote - 7 > SupportNotes.Min) ? mostPlayedNote - 7 : mostPlayedNote;
        List <int> basedChord = MusicCreator2.GetChord(checkInBounds, SupportNotes.Max);
        List<List<int>> chordsToUse = new List<List<int>>();
        chordsToUse.Add(basedChord);
        int lastChord = mostPlayedNote;
        for (int i = 1; i < NumberTabMelody; i++)
        {
            List<int> goodChords = new List<int>();
            int minusChord = lastChord - 4;
            int plusChord = lastChord + 4;
            int minorChordLow = lastChord - 12;
            int minorChordHigh = lastChord + 12;
            goodChords.Add(checkInBounds);
            if (minusChord >= SupportNotes.Min) goodChords.Add(minusChord);
            if (plusChord + 4 <= SupportNotes.Max) goodChords.Add(plusChord);
            if (minorChordLow >= SupportNotes.Min) goodChords.Add(minorChordLow);
            if (minorChordHigh + 4 <=  SupportNotes.Max) goodChords.Add(minorChordHigh);
            int chosenChord = goodChords[RandomGenerator.Next(0, goodChords.Count)];
            Debug.Assert(chosenChord >= SupportNotes.Min && chosenChord <= SupportNotes.Max, "Out of bound note : " + minusChord + ", " + plusChord + ", " + minorChordLow + ", " + minorChordHigh + ", " + SupportNotes.Max + ", ");
            chordsToUse.Add(MusicCreator2.GetChord(chosenChord, SupportNotes.Max));
        }

        // Create the rythm pattern of the support
        List<int> supportRythmPattern = new List<int>();
        for (int i = 0; i < beatsPerTab; i++)
        {
            int randomChordIndex = RandomGenerator.Next(0, 3);
            supportRythmPattern.Add(randomChordIndex);
        }

        // Add some silences randomly by replacing notes
        int numberSilence = RandomGenerator.Next(NumberSilenceInSupportPattern.Min, NumberSilenceInSupportPattern.Max);
        PutNoteRandomlyIn(MusicPlayer.SILENCE, supportRythmPattern, numberSilence);

        // Add some continue randomly by replacing notes
        int numberContinues = RandomGenerator.Next(NumberContinueInSupportPattern.Min, NumberContinueInSupportPattern.Max);
        PutNoteRandomlyIn(MusicPlayer.CONTINUE, supportRythmPattern, numberContinues);

        List<int> supportMelody = new List<int>();
        // Create each tab using the pattern
        for (int i = 0; i < melodyToSupport.Count / beatsPerTab; i++)
        {
            List<int> chordToUse = chordsToUse[i];
            for (int j = 0; j < beatsPerTab; j++)
            {
               if (supportRythmPattern[j] != MusicPlayer.SILENCE && supportRythmPattern[j] != MusicPlayer.CONTINUE)
                {
                    supportMelody.Add(chordToUse[supportRythmPattern[j]]);
                }
               else
                {
                    supportMelody.Add(supportRythmPattern[j]);
                }
            }
        }
        Debug.Assert(supportMelody.Count == melodyToSupport.Count, supportMelody.Count + " but" + melodyToSupport.Count);
        return supportMelody;
    }

    private void PutNoteRandomlyIn(int note, List<int> melody, int count)
    {
        for (int i = 0; i < count; i++)
        {
            melody[RandomGenerator.Next(0, melody.Count)] = note;
        }
    }

    private List<int> GetModifiedMelody(List<int> melody, Interval notes)
    {
        List<int> modifiedMelody = new List<int>(melody);
        int numberNotesModified = RandomGenerator.Next(NumberNotesModifiedInMelody.Min, NumberNotesModifiedInMelody.Max);
        for (int i = 0; i < numberNotesModified; i++)
        {
            int index = RandomGenerator.Next(0, melody.Count);
            List<int> jointNotes =  MusicCreator2.GetJointNotes(modifiedMelody[index], notes.Min, notes.Max);
            modifiedMelody[index] = RandomGenerator.Next(0, jointNotes.Count);
        }
        return modifiedMelody;
    }
}
