using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMusicCreator : MonoBehaviour {

    public MusicCreator2 mc;
    public MusicPlayer mp;

    private void Start()
    {
        mp.PlayMusic(mc.GenerateMusic());
    }
}
