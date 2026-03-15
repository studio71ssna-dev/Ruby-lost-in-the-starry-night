using UnityEngine;
using System.Collections;
using DG.Tweening;
public class Note : MonoBehaviour
{
    private float hitTime;
    private float spawnTime;
    private int noteFret;
    private int lane;

    private Vector3 spawnPosition;
    private Vector3 hitPosition;

    private MusicTimer musicTimer;
    private SpriteRenderer spriteRenderer;

    public float HitTime => hitTime;
    public int NoteFret => noteFret;
    public int Lane => lane;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        StartCoroutine(DestroyAfterUnscaledTime(5f));
    }

    public void Init(
        float noteHitTime,
        float noteSpawnTime,
        Vector3 spawnPos,
        Vector3 hitPos,
        MusicTimer timer,
        Color fretColor,
        int noteFretIndex,
        int laneIndex)
    {
        hitTime = noteHitTime;
        spawnTime = noteSpawnTime;

        spawnPosition = spawnPos;
        hitPosition = hitPos;

        musicTimer = timer;
        noteFret = noteFretIndex;
        lane = laneIndex;

        if (spriteRenderer != null)
            spriteRenderer.color = fretColor;
    }

    void Update()
    {
        float songTime = musicTimer.SongTime;
        float t = (songTime - spawnTime) / (hitTime - spawnTime);
        transform.position = Vector3.LerpUnclamped(spawnPosition, hitPosition, t);
    }
    IEnumerator DestroyAfterUnscaledTime(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Destroy(gameObject);
    }

}