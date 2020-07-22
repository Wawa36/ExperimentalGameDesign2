using UnityEngine.Audio;
using UnityEngine;

public class GlobalAudioSettings : MonoBehaviour
{
    public static GlobalAudioSettings instance;

    [Header("Spatial Audio")]
    [Range(0, 1f)]
    public float spatialBlend = 1f;
    public float maxDistance = 10f;
    public AudioRolloffMode rolloffMode = AudioRolloffMode.Custom;
    public AnimationCurve customSpacialCurve;
    public AudioMixerGroup[] audioMixerGroups;


    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }
}
