using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour {

    public const int SILENCE = -1;
    public const int CONTINUE = -2;

    public bool Loop = true;

    public void PlayMusic(Music music, double delay = 0)
    {
        currentMusic = music;
        CreateNewAudioSources(music.VoiceVolumes);
        isPlaying = true;
        nextTimeToPlay = AudioSettings.dspTime + delay;
        beatTime = (60.0 / currentMusic.BPM);
        musicUnitBeatTime = beatTime * currentMusic.BeatUnit;
        currentMusicBeat = 0;
    }

    private List<AudioSource> audioSources;

    private Music currentMusic;
    private bool isPlaying = false;

    private double nextTimeToPlay;
    private int currentMusicBeat = 0;

    private double beatTime;
    private double musicUnitBeatTime;

    private void Awake()
    {
        audioSources = new List<AudioSource>();
    }

    public MusicCreator2 mc;

    private void Start()
    {
        PlayMusic(mc.GenerateMusic());
    }

    private void Update()
    {
        if (!isPlaying || !(AudioSettings.dspTime >= nextTimeToPlay)) return;

        // Play the current note for each voices
        for (int i = 0; i < currentMusic.NumberVoices; i++)
        {         
            int note = currentMusic.Partition[i][currentMusicBeat];
            if (note == SILENCE) audioSources[i].Stop();
            else if (note == CONTINUE) continue;
            else
            {
                audioSources[i].clip = currentMusic.NotesSound[note];
                audioSources[i].PlayScheduled(AudioSettings.dspTime);
            }     
        }

        // Compute for next time to play
        if (++currentMusicBeat >= currentMusic.NumberMusicBeats)
        {
            isPlaying = false;
            if (Loop) PlayMusic(mc.GenerateMusic(), 1.0);
        }
        nextTimeToPlay = AudioSettings.dspTime +  musicUnitBeatTime;
    }

    private void CreateNewAudioSources(float[] volumes)
    {
        foreach (AudioSource audioSource in audioSources)
        {
            Destroy(audioSource);
        }
        audioSources.Clear();
        for (int i = 0; i < volumes.Length; i++)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
            audioSources[i].volume = volumes[i];
        }
    }
}
