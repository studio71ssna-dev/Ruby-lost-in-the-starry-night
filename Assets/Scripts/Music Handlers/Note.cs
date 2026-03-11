using UnityEngine;

public class Note : MonoBehaviour
{
    private float hitTime;
    private float spawnTime;

    private Vector3 spawnPosition;
    private Vector3 hitPosition;

    private MusicTimer musicTimer;

    public void Init(
        float noteHitTime,
        float noteSpawnTime,
        Vector3 spawnPos,
        Vector3 hitPos,
        MusicTimer timer)
    {
        hitTime = noteHitTime;
        spawnTime = noteSpawnTime;

        spawnPosition = spawnPos;
        hitPosition = hitPos;

        musicTimer = timer;
    }

    void Update()
    {
        float songTime = musicTimer.SongTime;

        float t = (songTime - spawnTime) / (hitTime - spawnTime);

        transform.position = Vector3.Lerp(spawnPosition, hitPosition, t);
    }
}