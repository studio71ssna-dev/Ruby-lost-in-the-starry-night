using UnityEngine;
using UnityEngine.UI; // Required in case your notes are UI Images

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
        Color noteColor) // The new color parameter
    {
        this.hitTime = hitTime;
        this.spawnTime = spawnTime;
        this.spawnPos = spawnPos;
        this.hitPos = hitPos;
        this.songController = controller;

        this.NoteFret = fret;
        this.Lane = lane;
        transform.position = spawnPos;

        // Apply the color to either a SpriteRenderer or a UI Image
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = noteColor;
        }
        else
        {
            Image img = GetComponent<Image>();
            if (img != null) img.color = noteColor;
        }
    }

    private void Update()
    {
        if (songController == null) return;

        double songTime = songController.SongTime;
        float t = (float)((songTime - spawnTime) / (hitTime - spawnTime));

        transform.position = Vector3.LerpUnclamped(spawnPos, hitPos, t);
    }
}