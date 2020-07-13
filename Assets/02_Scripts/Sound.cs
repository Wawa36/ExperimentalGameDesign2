using UnityEngine.Audio;
using UnityEngine;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
    //[Range(0,1f)]
    //public float spatialBlend = 1f;
    //public float maxDistance = 10f;
    //public AudioRolloffMode rolloffMode = AudioRolloffMode.Custom;

    [HideInInspector]
    public AudioSource source;

}
