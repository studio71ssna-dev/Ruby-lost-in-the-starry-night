using UnityEngine;

public class MusicTimer : MonoBehaviour
{
    [SerializeField] private AudioSource music;

    public float SongTime => music.time;

    public void StartSong(SongData song)
    {
        music.clip = song.music;
        music.time = 0f;
        music.Play();
    }
}