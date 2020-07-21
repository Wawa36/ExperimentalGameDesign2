using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
            if (s.name != "Teleport")
                s.source = this.gameObject.AddComponent<AudioSource>();
            else
            {
                // add new GameObj for teleport spatial animation sound
                GameObject soundObj = new GameObject();
                soundObj.transform.parent = this.transform;
                soundObj.name = "TeleportSound";
                soundObj.transform.position = this.transform.position;
                s.source = soundObj.AddComponent<AudioSource>();
            }
            
            // individual settings
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            // global audio settings
            s.source.outputAudioMixerGroup = GlobalAudioSettings.instance.audioMixerGroups[0];
            s.source.spatialBlend = GlobalAudioSettings.instance.spatialBlend;
            s.source.maxDistance = GlobalAudioSettings.instance.maxDistance;
            s.source.rolloffMode = GlobalAudioSettings.instance.rolloffMode;
            if (GlobalAudioSettings.instance.rolloffMode == AudioRolloffMode.Custom)
                s.source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GlobalAudioSettings.instance.customSpacialCurve);

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
    
    public IEnumerator PlayFromAToB(string name, float maxTime, GameObject obj, Vector3 startPos, Vector3 goalPos)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        float timer = 0;
        if (s != null)
        {
            // start playing
            //if (!s.source.isPlaying)
                s.source.Play();

            // lerp position
            while (timer < maxTime)
            {
                timer += Time.deltaTime;
                obj.transform.position = Vector3.Lerp(startPos, goalPos, timer / maxTime);
                print("sound coroutine");
                Debug.DrawLine(startPos, goalPos, Color.magenta);
                yield return null;
            }
            print("ende coroutine");
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
