using UnityEngine;
using UnityEngine.UI;

public class Note : MonoBehaviour
{
    private double hitTime;
    private double spawnTime;
    private Vector3 spawnPos;
    private Vector3 hitPos;
    private SongController songController;

    public int NoteFret { get; private set; }
    public int Lane { get; private set; }
    public double HitTime => hitTime;

    public void Init(
        double hitTime,
        double spawnTime,
        Vector3 spawnPos,
        Vector3 hitPos,
        SongController controller,
        int fret,
        int lane,
        Color noteColor)
    {
        this.hitTime = hitTime;
        this.spawnTime = spawnTime;
        this.spawnPos = spawnPos;
        this.hitPos = hitPos;
        this.songController = controller;
        this.NoteFret = fret;
        this.Lane = lane;

        transform.position = spawnPos;

        // Search children too — handles prefabs where renderer is on a child object
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = noteColor;
            return;
        }

        Image img = GetComponentInChildren<Image>();
        if (img != null)
            img.color = noteColor;
    }

    private void Update()
    {
        if (songController == null) return;

        double songTime = songController.SongTime;
        float t = (float)((songTime - spawnTime) / (hitTime - spawnTime));
        transform.position = Vector3.LerpUnclamped(spawnPos, hitPos, t);
    }
}