using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] sounds;
    GlobalAudioSettings audioSettings;

    void Start()
    {
        //if (instance == null && this.name == "Audiomanager")
        //    instance = this;
        //else
        //    Destroy(this.gameObject);



        foreach (Sound s in sounds)
        {
            s.source = this.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            print("gameobj: " + this.gameObject.name);
            s.source.spatialBlend = GlobalAudioSettings.instance.spatialBlend;
            s.source.rolloffMode = GlobalAudioSettings.instance.rolloffMode;
            s.source.maxDistance = GlobalAudioSettings.instance.maxDistance;
            if (GlobalAudioSettings.instance.rolloffMode == AudioRolloffMode.Custom)
                s.source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GlobalAudioSettings.instance.customSpacialCurve);
            else
                s.source.rolloffMode = GlobalAudioSettings.instance.rolloffMode;
        }
    }

    

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //    Stop("Stone");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            //print("is playing? " + s.source.isPlaying);
            if (!s.source.isPlaying)
                s.source.Play();
        }
        else
            Debug.LogError("Sound not found");
            
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Stop();
    }

    //public bool IsPlaying(string name)
    //{
    //    Sound s = Array.Find(sounds, sound => sound.name == name);
    //    if (s != null)
    //    {
    //        if (s.source.isPlaying)
    //            return true;
    //        else
    //            return false;
    //    }
    //}
}
