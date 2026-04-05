using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private AudioSource source;
    private double startDspTime;

    public double CurrentDSPTime => AudioSettings.dspTime - startDspTime;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
        startDspTime = AudioSettings.dspTime;
    }

    public void Stop()
    {
        source.Stop();
    }

    public bool IsPlaying => source.isPlaying;
    public float ClipLength => source.clip != null ? source.clip.length : 0f;
}