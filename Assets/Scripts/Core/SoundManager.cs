using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class SoundManager : DontDestroy<SoundManager>
{
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceSound;

    public void StartState()
    {
        GameData.GetBool(GameData.StringHelper.StateMusic, true);
        GameData.GetBool(GameData.StringHelper.StateSound, true);
        GameData.GetBool(GameData.StringHelper.StateVibration, true);
    }

    public void PlayBGM()
    {
        
    }

    public void StopBGM()
    {
        
    }
    public void PlaySound(AudioClip clip)
    {
        if (!GameData.StateSound) return;
        audioSourceSound.PlayOneShot(clip);
    }
}