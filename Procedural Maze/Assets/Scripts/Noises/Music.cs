using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music
{
    /// <summary>
    /// Each list contains notes to play at each half-beats for an instrument.
    /// </summary>
    public List<List<int>> Partition;
    /// <summary>
    /// Number of voices in the partition.
    /// Correspond to the number of list in the partition.
    /// </summary>
    public int NumberVoices { get { return Partition.Count; } }
    /// <summary>
    /// The nth int is the volume of the nth voice.
    /// </summary>
    public float[] VoiceVolumes;
    public int NumberMusicBeats { get { return Partition[0].Count; } }
    /// <summary>
    /// The audioclips used to play each notes.
    /// </summary>
    public List<AudioClip> NotesSound;
    /// <summary>
    /// Number of beats per minute. It's the speed of the music.
    /// </summary>
    public double BPM;
    /// <summary>
    /// How long should last a note.
    /// A value of 1/2 means a note of the partition lasts 1/2 beat.
    /// </summary>
    public double BeatUnit;

    public Music()
    {
        Partition = new List<List<int>>();
    }
}
