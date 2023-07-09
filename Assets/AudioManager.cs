using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    public AudioClip def;
    public float defPitchRange;
    
    public void Play(AudioClip audio, bool alterPitch){
        
        AudioSource source = GetComponent<AudioSource>();
        if(alterPitch){
            source.pitch =  source.pitch + defPitchRange;
        }
        source.clip = audio;
        source.Play();
    }
    public void Play(AudioClip audio){
        AudioSource source = GetComponent<AudioSource>();
        source.clip = audio;
        source.Play();
    }
    public void Play(bool alterPitch){
        Play(def,alterPitch);
    }


    


}
