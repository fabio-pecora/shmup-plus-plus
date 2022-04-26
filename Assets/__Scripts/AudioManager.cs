using UnityEngine;
using System;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    // Array of sounds
    public Sound[] sounds;

    void Start()
    {
        foreach (Sound s in sounds)
        {
            // In order to set clip, volume, pitch and loop
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    //We'll call this method to play the sound with the specific name
    public void Play(string name)
    {
        //We are looping through the array of sounds
        Sound s = Array.Find(sounds, sound => sound.name == name);
        //If the sound with that name is not found we throw an error message
        if (s == null)
        {
            Debug.LogWarning("Sound " + name + " not found!");
        }
        //If it is found then we play it
        s.source.Play();
    }
}