using SingletonManagers;
using UnityEngine;

public class PlayerMusic : MonoBehaviour
{
    [SerializeField] private InputJudge judge;
    [SerializeField] private NoteManager noteManager;
    [SerializeField] private WolfPressureManager wolfPressureManager;

    // 0=white(neutral) 1=brown(left) 2=green(down) 3=purple(right)
    private int activeColor = 0;

    private void OnEnable()
    {
        if (InputHandler.Instance == null) return;

        judge.OnHit += HandleHit;
        noteManager.OnMiss += HandleMiss;


        // Lane keys — press 1/2/3 to hit notes in that column
        InputHandler.Instance.OnLane1 += () => HandleLane(0);
        InputHandler.Instance.OnLane2 += () => HandleLane(1);
        InputHandler.Instance.OnLane3 += () => HandleLane(2);

        // Arrow keys held — set active color
        // Fret1 = left arrow (brown), Fret2 = down arrow (green), Fret3 = right arrow (purple)
        InputHandler.Instance.OnFret1 += (held) => HandleColorKey(1, held);
        InputHandler.Instance.OnFret2 += (held) => HandleColorKey(2, held);
        InputHandler.Instance.OnFret3 += (held) => HandleColorKey(3, held);
    }

    private void OnDisable()
    {
        judge.OnHit -= HandleHit;
        noteManager.OnMiss -= HandleMiss;
        activeColor = 0;
    }

    private void Start()
    {
        judge.OnHit += HandleHit;
        noteManager.OnMiss += HandleMiss;
    }

    private void HandleLane(int lane)
    {
        judge.Judge(lane, activeColor);
    }

    private void HandleColorKey(int color, bool held)
    {
        if (held)
        {
            // Holding this arrow — switch to its color
            activeColor = color;
        }
        else
        {
            // Released — if this was the active color, go back to neutral (white)
            if (activeColor == color)
                activeColor = 0;
        }
    }

    private void HandleHit(HitResult result, Vector3 pos)
    {
        Debug.Log($"HandleHit fired: {result} | scoreManager null? {wolfPressureManager == null}");
        switch (result)
        {
            case HitResult.Perfect:
                wolfPressureManager?.Perfect();
                ParticleManager.Instance?.PlayParticle("NoteHit", pos);
                AudioManager.Instance?.Play("NoteHit", pos);
                break;

            case HitResult.Good:
                wolfPressureManager?.Good();
                ParticleManager.Instance?.PlayParticle("NoteHit", pos);
                AudioManager.Instance?.Play("NoteHit", pos);
                break;
        }
    }

    private void HandleMiss()
    {
        // Bar fills automatically — miss just means no reduction
        AudioManager.Instance?.Play("NoteMiss", transform.position);
    }
}