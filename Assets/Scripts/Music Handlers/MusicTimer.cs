using UnityEngine;

public class MusicTimer : MonoBehaviour
{
    private AudioSource music;

    public float SongTime => music.time;

    private void Awake()
    {
        music = GetComponent<AudioSource>();
    }
    public void StartSong(SongData song)
    {
        music.clip = song.music;
        music.time = 0f;
        music.Play();
    }
}